using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NBitcoin;
using NUnit.Framework;
using Stratis.SmartContracts;
using Stratis.SmartContracts.CLR;
using System.Threading.Tasks;
using Ticketbooth.Api.Requests;
using Ticketbooth.Api.Responses;
using static TicketContract;

namespace Ticketbooth.Api.Tests.Controllers
{
    public partial class PublicControllerTests
    {
        private ReserveTicketRequest ReserveTicketRequestWithoutCustomerName => new ReserveTicketRequest
        {
            AccountName = "account 0",
            GasPrice = 100,
            Password = "hunter2",
            Seat = new Seat { Number = 1, Letter = 'A' },
            Sender = "tNuiVJiEhvbQgXu4P32S4TnAVEG3kgLnu8",
            WalletName = "Wallet One"
        };

        private ReserveTicketRequest ReserveTicketRequestWithCustomerName => new ReserveTicketRequest
        {
            AccountName = "account 0",
            CustomerName = "Benjamin Swift",
            GasPrice = 100,
            Password = "hunter2",
            Seat = new Seat { Number = 1, Letter = 'A' },
            Sender = "tNuiVJiEhvbQgXu4P32S4TnAVEG3kgLnu8",
            WalletName = "Wallet One"
        };

        private Ticket[] TicketsWithoutRequestedReserveTicket => new Ticket[]
        {
            new Ticket
            {
                Address = Address.Zero,
                CustomerIdentifier = null,
                Price = 100_000_000,
                Seat = new Seat { Number = 1, Letter = 'B' },
                Secret = null
            }
        };

        private Ticket[] TicketsContainingValidRequestedReserveTicket => new Ticket[]
        {
            new Ticket
            {
                Address = Address.Zero,
                CustomerIdentifier = null,
                Price = 100_000_000,
                Seat = new Seat { Number = 1, Letter = 'A' },
                Secret = null
            }
        };

        private Ticket[] TicketsContainingAlreadyReservedRequestedReserveTicket => new Ticket[]
        {
            new Ticket
            {
                Address = "tNuiVJiEhvbQgXu4P32S4TnAVEG3kgLnu8".ToAddress(_network),
                CustomerIdentifier = null,
                Price = 100_000_000,
                Seat = new Seat { Number = 1, Letter = 'A' },
                Secret = null
            }
        };

        private string PresetSecret => "k9xiZFLi7n38Bhw";

        [Test]
        public async Task ReserveTicket_InvalidAddress_TransactionNotBroadcastStatusCode400()
        {
            // Arrange
            var address = InvalidAddress;
            var requestBody = ReserveTicketRequestWithCustomerName;

            // Act
            var result = await _publicController.ReserveTicket(address, requestBody) as ObjectResult;

            // Assert
            _broadcasterManager.Verify(callTo => callTo.BroadcastTransactionAsync(It.IsAny<Transaction>()), Times.Never);
            Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status400BadRequest));
        }

        [Test]
        public async Task ReserveTicket_ValidAddressContractNotFound_TransactionNotBroadcastStatusCode404()
        {
            // Arrange
            var address = ValidAddress;
            var requestBody = ReserveTicketRequestWithCustomerName;

            SetupContractDoesNotExist(address);

            // Act
            var result = await _publicController.ReserveTicket(address, requestBody) as ObjectResult;

            // Assert
            _broadcasterManager.Verify(callTo => callTo.BroadcastTransactionAsync(It.IsAny<Transaction>()), Times.Never);
            Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status404NotFound));
        }

        [Test]
        public async Task ReserveTicket_TicketNotFound_TransactionNotBroadcastStatusCode400()
        {
            // Arrange
            var address = ValidAddress;
            var requestBody = ReserveTicketRequestWithCustomerName;

            SetupContractExists(address);
            SetTicketsTo(TicketsWithoutRequestedReserveTicket);

            // Act
            var result = await _publicController.ReserveTicket(address, requestBody) as ObjectResult;

            // Assert
            _broadcasterManager.Verify(callTo => callTo.BroadcastTransactionAsync(It.IsAny<Transaction>()), Times.Never);
            Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status400BadRequest));
        }

        [Test]
        public async Task ReserveTicket_SaleNotActive_TransactionNotBroadcastStatusCode409()
        {
            // Arrange
            var address = ValidAddress;
            var requestBody = ReserveTicketRequestWithCustomerName;

            SetupContractExists(address);
            SetTicketsTo(TicketsContainingValidRequestedReserveTicket);
            SetEndOfSaleToBlock(default);

            // Act
            var result = await _publicController.ReserveTicket(address, requestBody) as ObjectResult;

            // Assert
            _broadcasterManager.Verify(callTo => callTo.BroadcastTransactionAsync(It.IsAny<Transaction>()), Times.Never);
            Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status409Conflict));
        }

        [Test]
        public async Task ReserveTicket_SaleHasEnded_TransactionNotBroadcastStatusCode409()
        {
            // Arrange
            var address = ValidAddress;
            var requestBody = ReserveTicketRequestWithCustomerName;

            SetupContractExists(address);
            SetTicketsTo(TicketsContainingValidRequestedReserveTicket);
            SetConsensusHeight(850_000);
            SetEndOfSaleToBlock(800_000);

            // Act
            var result = await _publicController.ReserveTicket(address, requestBody) as ObjectResult;

            // Assert
            _broadcasterManager.Verify(callTo => callTo.BroadcastTransactionAsync(It.IsAny<Transaction>()), Times.Never);
            Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status409Conflict));
        }

        [Test]
        public async Task ReserveTicket_TicketAlreadyReserved_TransactionNotBroadcastStatusCode400()
        {
            // Arrange
            var address = ValidAddress;
            var requestBody = ReserveTicketRequestWithCustomerName;

            SetupContractExists(address);
            SetTicketsTo(TicketsContainingAlreadyReservedRequestedReserveTicket);
            SetConsensusHeight(750_000);
            SetEndOfSaleToBlock(800_000);

            // Act
            var result = await _publicController.ReserveTicket(address, requestBody) as ObjectResult;

            // Assert
            _broadcasterManager.Verify(callTo => callTo.BroadcastTransactionAsync(It.IsAny<Transaction>()), Times.Never);
            Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status400BadRequest));
        }

        [Test]
        public async Task ReserveTicket_CustomerNameRequiredButNotSupplied_TransactionNotBroadcastStatusCode400()
        {
            // Arrange
            var address = ValidAddress;
            var requestBody = ReserveTicketRequestWithoutCustomerName;

            SetupContractExists(address);
            SetTicketsTo(TicketsContainingValidRequestedReserveTicket);
            SetConsensusHeight(750_000);
            SetEndOfSaleToBlock(800_000);
            SetIdentityVerificationPolicyTo(true);

            // Act
            var result = await _publicController.ReserveTicket(address, requestBody) as ObjectResult;

            // Assert
            _broadcasterManager.Verify(callTo => callTo.BroadcastTransactionAsync(It.IsAny<Transaction>()), Times.Never);
            Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status400BadRequest));
        }

        [Test]
        public async Task ReserveTicket_NoConnections_TransactionNotBroadcastStatusCode403()
        {
            // Arrange
            var address = ValidAddress;
            var requestBody = ReserveTicketRequestWithCustomerName;

            SetupContractExists(address);
            SetTicketsTo(TicketsContainingValidRequestedReserveTicket);
            SetConsensusHeight(750_000);
            SetEndOfSaleToBlock(800_000);
            SetIdentityVerificationPolicyTo(true);
            SetupZeroConnections();

            // Act
            var result = await _publicController.ReserveTicket(address, requestBody) as ObjectResult;

            // Assert
            _broadcasterManager.Verify(callTo => callTo.BroadcastTransactionAsync(It.IsAny<Transaction>()), Times.Never);
            Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status403Forbidden));
        }

        [Test]
        public async Task ReserveTicket_BuildCallTransactionUnsuccessful_TransactionNotBroadcastStatusCode400()
        {
            // Arrange
            var address = ValidAddress;
            var requestBody = ReserveTicketRequestWithCustomerName;

            SetupContractExists(address);
            SetTicketsTo(TicketsContainingValidRequestedReserveTicket);
            SetConsensusHeight(750_000);
            SetEndOfSaleToBlock(800_000);
            SetIdentityVerificationPolicyTo(true);
            SetupOneConnection();
            _stringGenerator.Setup(callTo => callTo.CreateUniqueString(15)).Returns(PresetSecret);
            SetupFailedBuildCallContractTransactionRequest();

            // Act
            var result = await _publicController.ReserveTicket(address, requestBody) as ObjectResult;

            // Assert
            _broadcasterManager.Verify(callTo => callTo.BroadcastTransactionAsync(It.IsAny<Transaction>()), Times.Never);
            Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status400BadRequest));
        }

        [Test]
        public async Task ReserveTicket_BuildCallTransactionSuccessful_TransactionBroadcast()
        {
            // Arrange
            var address = ValidAddress;
            var requestBody = ReserveTicketRequestWithCustomerName;

            SetupContractExists(address);
            SetTicketsTo(TicketsContainingValidRequestedReserveTicket);
            SetConsensusHeight(750_000);
            SetEndOfSaleToBlock(800_000);
            SetIdentityVerificationPolicyTo(true);
            SetupOneConnection();
            _stringGenerator.Setup(callTo => callTo.CreateUniqueString(15)).Returns(PresetSecret);
            var buildCallTransactionResponse = SetupSuccessfulBuildCallContractTransactionRequest();

            // Act
            var result = await _publicController.ReserveTicket(address, requestBody) as ObjectResult;

            // Assert
            _broadcasterManager.Verify(callTo => callTo.BroadcastTransactionAsync(It.Is<Transaction>(transaction => transaction.ToHex() == buildCallTransactionResponse.Hex)), Times.Once);
        }

        [Test]
        public async Task ReserveTicket_CannotBroadcastTransaction_StatusCode500()
        {
            // Arrange
            var address = ValidAddress;
            var requestBody = ReserveTicketRequestWithCustomerName;

            SetupContractExists(address);
            SetTicketsTo(TicketsContainingValidRequestedReserveTicket);
            SetConsensusHeight(750_000);
            SetEndOfSaleToBlock(800_000);
            SetIdentityVerificationPolicyTo(true);
            SetupOneConnection();
            _stringGenerator.Setup(callTo => callTo.CreateUniqueString(15)).Returns(PresetSecret);
            SetupSuccessfulBuildCallContractTransactionRequest();
            SetupCannotBroadcastTransaction();

            // Act
            var result = await _publicController.ReserveTicket(address, requestBody) as ObjectResult;

            // Assert
            Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status500InternalServerError));
        }

        [Test]
        public async Task ReserveTicket_TransactionBroadcasted_StatusCode201()
        {
            // Arrange
            var address = ValidAddress;
            var requestBody = ReserveTicketRequestWithCustomerName;

            SetupContractExists(address);
            SetTicketsTo(TicketsContainingValidRequestedReserveTicket);
            SetConsensusHeight(750_000);
            SetEndOfSaleToBlock(800_000);
            SetIdentityVerificationPolicyTo(true);
            SetupOneConnection();
            _stringGenerator.Setup(callTo => callTo.CreateUniqueString(15)).Returns(PresetSecret);
            SetupSuccessfulBuildCallContractTransactionRequest();
            SetupTransactionWasBroadcast();

            // Act
            var result = await _publicController.ReserveTicket(address, requestBody) as ObjectResult;

            // Assert
            Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status201Created));
        }

        [Test]
        public async Task ReserveTicket_RequiresIdentityVerification_ReturnsCustomerCbcResult()
        {
            // Arrange
            var address = ValidAddress;
            var requestBody = ReserveTicketRequestWithCustomerName;

            SetupContractExists(address);
            SetTicketsTo(TicketsContainingValidRequestedReserveTicket);
            SetConsensusHeight(750_000);
            SetEndOfSaleToBlock(800_000);
            SetIdentityVerificationPolicyTo(true);
            SetupOneConnection();
            _stringGenerator.Setup(callTo => callTo.CreateUniqueString(15)).Returns(PresetSecret);
            SetupSuccessfulBuildCallContractTransactionRequest();
            SetupTransactionWasBroadcast();

            // Act
            var result = await _publicController.ReserveTicket(address, requestBody) as ObjectResult;

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(((TicketReservationDetailsResponse)result.Value).Secret, Is.Not.Null);
                Assert.That(((TicketReservationDetailsResponse)result.Value).CustomerName, Is.Not.Null);
            });
        }

        [Test]
        public async Task ReserveTicket_RequiresIdentityVerification_ReturnsCustomerNull()
        {
            // Arrange
            var address = ValidAddress;
            var requestBody = ReserveTicketRequestWithCustomerName;

            SetupContractExists(address);
            SetTicketsTo(TicketsContainingValidRequestedReserveTicket);
            SetConsensusHeight(750_000);
            SetEndOfSaleToBlock(800_000);
            SetIdentityVerificationPolicyTo(false);
            SetupOneConnection();
            _stringGenerator.Setup(callTo => callTo.CreateUniqueString(15)).Returns(PresetSecret);
            SetupSuccessfulBuildCallContractTransactionRequest();
            SetupTransactionWasBroadcast();

            // Act
            var result = await _publicController.ReserveTicket(address, requestBody) as ObjectResult;

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(((TicketReservationDetailsResponse)result.Value).Secret, Is.Not.Null);
                Assert.That(((TicketReservationDetailsResponse)result.Value).CustomerName, Is.Null);
            });
        }
    }
}
