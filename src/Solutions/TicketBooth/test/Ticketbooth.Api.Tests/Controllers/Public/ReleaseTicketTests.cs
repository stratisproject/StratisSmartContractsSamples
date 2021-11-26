using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NBitcoin;
using NUnit.Framework;
using Stratis.SmartContracts;
using Stratis.SmartContracts.CLR;
using System.Threading.Tasks;
using Ticketbooth.Api.Requests;
using static TicketContract;

namespace Ticketbooth.Api.Tests.Controllers
{
    public partial class PublicControllerTests
    {
        private ReleaseTicketRequest ReleaseTicketRequest => new ReleaseTicketRequest
        {
            AccountName = "account 0",
            GasPrice = 100,
            Password = "hunter2",
            Seat = new Seat { Number = 1, Letter = 'A' },
            Sender = "tNuiVJiEhvbQgXu4P32S4TnAVEG3kgLnu8",
            WalletName = "Wallet One"
        };

        private Ticket[] TicketsWithoutRequestedReleaseTicket => new Ticket[]
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

        private Ticket[] TicketsContainingValidRequestedReleaseTicket => new Ticket[]
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

        private Ticket[] TicketsContainingNotOwnedBySenderRequestedReleaseTicket => new Ticket[]
        {
            new Ticket
            {
                Address = "tU9vpG8bfQxCQxGgHSjFYdb936jddYGrhm".ToAddress(_network),
                CustomerIdentifier = null,
                Price = 100_000_000,
                Seat = new Seat { Number = 1, Letter = 'A' },
                Secret = null
            }
        };

        private Ticket[] TicketsContainingUnownedRequestedReleaseTicket => new Ticket[]
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

        [Test]
        public async Task ReleaseTicket_InvalidAddress_TransactionNotBroadcastStatusCode400()
        {
            // Arrange
            var address = InvalidAddress;
            var requestBody = ReleaseTicketRequest;

            // Act
            var result = await _publicController.ReleaseTicket(address, requestBody) as ObjectResult;

            // Assert
            _broadcasterManager.Verify(callTo => callTo.BroadcastTransactionAsync(It.IsAny<Transaction>()), Times.Never);
            Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status400BadRequest));
        }

        [Test]
        public async Task ReleaseTicket_ValidAddressContractNotFound_TransactionNotBroadcastStatusCode404()
        {
            // Arrange
            var address = ValidAddress;
            var requestBody = ReleaseTicketRequest;

            SetupContractDoesNotExist(address);

            // Act
            var result = await _publicController.ReleaseTicket(address, requestBody) as ObjectResult;

            // Assert
            _broadcasterManager.Verify(callTo => callTo.BroadcastTransactionAsync(It.IsAny<Transaction>()), Times.Never);
            Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status404NotFound));
        }

        [Test]
        public async Task ReleaseTicket_TicketNotFound_TransactionNotBroadcastStatusCode400()
        {
            // Arrange
            var address = ValidAddress;
            var requestBody = ReleaseTicketRequest;

            SetupContractExists(address);
            SetTicketsTo(TicketsWithoutRequestedReleaseTicket);

            // Act
            var result = await _publicController.ReleaseTicket(address, requestBody) as ObjectResult;

            // Assert
            _broadcasterManager.Verify(callTo => callTo.BroadcastTransactionAsync(It.IsAny<Transaction>()), Times.Never);
            Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status400BadRequest));
        }

        [Test]
        public async Task ReleaseTicket_SaleNotActive_TransactionNotBroadcastStatusCode409()
        {
            // Arrange
            var address = ValidAddress;
            var requestBody = ReleaseTicketRequest;

            SetupContractExists(address);
            SetTicketsTo(TicketsContainingValidRequestedReleaseTicket);
            SetEndOfSaleToBlock(default);

            // Act
            var result = await _publicController.ReleaseTicket(address, requestBody) as ObjectResult;

            // Assert
            _broadcasterManager.Verify(callTo => callTo.BroadcastTransactionAsync(It.IsAny<Transaction>()), Times.Never);
            Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status409Conflict));
        }

        [Test]
        public async Task ReleaseTicket_SaleHasEnded_TransactionNotBroadcastStatusCode409()
        {
            // Arrange
            var address = ValidAddress;
            var requestBody = ReleaseTicketRequest;

            SetupContractExists(address);
            SetTicketsTo(TicketsContainingValidRequestedReleaseTicket);
            SetConsensusHeight(850_000);
            SetEndOfSaleToBlock(800_000);

            // Act
            var result = await _publicController.ReleaseTicket(address, requestBody) as ObjectResult;

            // Assert
            _broadcasterManager.Verify(callTo => callTo.BroadcastTransactionAsync(It.IsAny<Transaction>()), Times.Never);
            Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status409Conflict));
        }

        [Test]
        public async Task ReleaseTicket_TicketNotOwned_TransactionNotBroadcastStatusCode400()
        {
            // Arrange
            var address = ValidAddress;
            var requestBody = ReleaseTicketRequest;

            SetupContractExists(address);
            SetTicketsTo(TicketsContainingUnownedRequestedReleaseTicket);
            SetConsensusHeight(750_000);
            SetNoReleaseBlocksTo(49_999);
            SetEndOfSaleToBlock(800_000);

            // Act
            var result = await _publicController.ReleaseTicket(address, requestBody) as ObjectResult;

            // Assert
            _broadcasterManager.Verify(callTo => callTo.BroadcastTransactionAsync(It.IsAny<Transaction>()), Times.Never);
            Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status400BadRequest));
        }

        [Test]
        public async Task ReleaseTicket_TicketNotOwnedBySender_TransactionNotBroadcastStatusCode400()
        {
            // Arrange
            var address = ValidAddress;
            var requestBody = ReleaseTicketRequest;

            SetupContractExists(address);
            SetTicketsTo(TicketsContainingNotOwnedBySenderRequestedReleaseTicket);
            SetConsensusHeight(750_000);
            SetNoReleaseBlocksTo(49_999);
            SetEndOfSaleToBlock(800_000);

            // Act
            var result = await _publicController.ReleaseTicket(address, requestBody) as ObjectResult;

            // Assert
            _broadcasterManager.Verify(callTo => callTo.BroadcastTransactionAsync(It.IsAny<Transaction>()), Times.Never);
            Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status400BadRequest));
        }

        [Test]
        public async Task ReleaseTicket_NoRefundsAllowed_TransactionNotBroadcastStatusCode409()
        {
            // Arrange
            var address = ValidAddress;
            var requestBody = ReleaseTicketRequest;

            SetupContractExists(address);
            SetTicketsTo(TicketsContainingValidRequestedReleaseTicket);
            SetConsensusHeight(750_000);
            SetNoReleaseBlocksTo(50_000);
            SetEndOfSaleToBlock(800_000);

            // Act
            var result = await _publicController.ReleaseTicket(address, requestBody) as ObjectResult;

            // Assert
            _broadcasterManager.Verify(callTo => callTo.BroadcastTransactionAsync(It.IsAny<Transaction>()), Times.Never);
            Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status409Conflict));
        }

        [Test]
        public async Task ReleaseTicket_NoConnections_TransactionNotBroadcastStatusCode403()
        {
            // Arrange
            var address = ValidAddress;
            var requestBody = ReleaseTicketRequest;

            SetupContractExists(address);
            SetTicketsTo(TicketsContainingValidRequestedReleaseTicket);
            SetConsensusHeight(750_000);
            SetNoReleaseBlocksTo(49_999);
            SetEndOfSaleToBlock(800_000);
            SetupZeroConnections();

            // Act
            var result = await _publicController.ReleaseTicket(address, requestBody) as ObjectResult;

            // Assert
            _broadcasterManager.Verify(callTo => callTo.BroadcastTransactionAsync(It.IsAny<Transaction>()), Times.Never);
            Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status403Forbidden));
        }

        [Test]
        public async Task ReleaseTicket_BuildCallTransactionUnsuccessful_TransactionNotBroadcastStatusCode400()
        {
            // Arrange
            var address = ValidAddress;
            var requestBody = ReleaseTicketRequest;

            SetupContractExists(address);
            SetTicketsTo(TicketsContainingValidRequestedReleaseTicket);
            SetConsensusHeight(750_000);
            SetNoReleaseBlocksTo(49_999);
            SetEndOfSaleToBlock(800_000);
            SetupOneConnection();
            SetupFailedBuildCallContractTransactionRequest();

            // Act
            var result = await _publicController.ReleaseTicket(address, requestBody) as ObjectResult;

            // Assert
            _broadcasterManager.Verify(callTo => callTo.BroadcastTransactionAsync(It.IsAny<Transaction>()), Times.Never);
            Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status400BadRequest));
        }

        [Test]
        public async Task ReleaseTicket_BuildCallTransactionSuccessful_TransactionBroadcast()
        {
            // Arrange
            var address = ValidAddress;
            var requestBody = ReleaseTicketRequest;

            SetupContractExists(address);
            SetTicketsTo(TicketsContainingValidRequestedReleaseTicket);
            SetConsensusHeight(750_000);
            SetNoReleaseBlocksTo(49_999);
            SetEndOfSaleToBlock(800_000);
            SetupOneConnection();
            var buildCallTransactionResponse = SetupSuccessfulBuildCallContractTransactionRequest();

            // Act
            var result = await _publicController.ReleaseTicket(address, requestBody) as ObjectResult;

            // Assert
            _broadcasterManager.Verify(callTo => callTo.BroadcastTransactionAsync(It.Is<Transaction>(transaction => transaction.ToHex() == buildCallTransactionResponse.Hex)), Times.Once);
        }

        [Test]
        public async Task ReleaseTicket_CannotBroadcastTransaction_StatusCode500()
        {
            // Arrange
            var address = ValidAddress;
            var requestBody = ReleaseTicketRequest;

            SetupContractExists(address);
            SetTicketsTo(TicketsContainingValidRequestedReleaseTicket);
            SetConsensusHeight(750_000);
            SetNoReleaseBlocksTo(49_999);
            SetEndOfSaleToBlock(800_000);
            SetupOneConnection();
            SetupSuccessfulBuildCallContractTransactionRequest();
            SetupCannotBroadcastTransaction();

            // Act
            var result = await _publicController.ReleaseTicket(address, requestBody) as ObjectResult;

            // Assert
            Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status500InternalServerError));
        }

        [Test]
        public async Task ReleaseTicket_TransactionBroadcasted_StatusCode201()
        {
            // Arrange
            var address = ValidAddress;
            var requestBody = ReleaseTicketRequest;

            SetupContractExists(address);
            SetTicketsTo(TicketsContainingValidRequestedReleaseTicket);
            SetConsensusHeight(750_000);
            SetNoReleaseBlocksTo(49_999);
            SetEndOfSaleToBlock(800_000);
            SetupOneConnection();
            SetupSuccessfulBuildCallContractTransactionRequest();
            SetupTransactionWasBroadcast();

            // Act
            var result = await _publicController.ReleaseTicket(address, requestBody) as ObjectResult;

            // Assert
            Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status201Created));
        }
    }
}
