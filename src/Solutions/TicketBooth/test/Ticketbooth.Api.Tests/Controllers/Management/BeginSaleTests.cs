using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NBitcoin;
using NUnit.Framework;
using Stratis.SmartContracts;
using System.Threading.Tasks;
using Ticketbooth.Api.Requests;
using static TicketContract;

namespace Ticketbooth.Api.Tests.Controllers
{
    public partial class ManagementControllerTests
    {
        private BeginSaleRequest BeginSaleRequest => new BeginSaleRequest
        {
            AccountName = "account 0",
            Details = new Requests.Show
            {
                EndOfSale = 1_000_000,
                Name = "Greatest Hits Tour",
                Organiser = "Rick Astley",
                Time = 1895075621
            },
            GasPrice = 100,
            Password = "hunter2",
            SeatPrices = new SeatPricing[]
            {
                new SeatPricing { Seat = new Seat { Number = 1, Letter = 'A' }, Price = 1_000_000_000 }
            },
            Sender = "tNuiVJiEhvbQgXu4P32S4TnAVEG3kgLnu8",
            WalletName = "Wallet One"
        };

        [Test]
        public async Task BeginSale_InvalidAddress_TransactionNotBroadcastStatusCode400()
        {
            // Arrange
            var address = InvalidAddress;
            var requestBody = BeginSaleRequest;

            // Act
            var result = await _managementController.BeginSale(address, requestBody) as ObjectResult;

            // Assert
            _broadcasterManager.Verify(callTo => callTo.BroadcastTransactionAsync(It.IsAny<Transaction>()), Times.Never);
            Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status400BadRequest));
        }

        [Test]
        public async Task BeginSale_EndOfSaleNotInFuture_TransactionNotBroadcastStatusCode400()
        {
            // Arrange
            var address = ValidAddress;
            var requestBody = BeginSaleRequest;

            SetConsensusHeight(1_000_000);

            // Act
            var result = await _managementController.BeginSale(address, requestBody) as ObjectResult;

            // Assert
            _broadcasterManager.Verify(callTo => callTo.BroadcastTransactionAsync(It.IsAny<Transaction>()), Times.Never);
            Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status400BadRequest));
        }

        [Test]
        public async Task BeginSale_ValidAddressContractNotFound_TransactionNotBroadcastStatusCode404()
        {
            // Arrange
            var address = ValidAddress;
            var requestBody = BeginSaleRequest;

            SetConsensusHeight(999_000);
            SetupContractDoesNotExist(address);

            // Act
            var result = await _managementController.BeginSale(address, requestBody) as ObjectResult;

            // Assert
            _broadcasterManager.Verify(callTo => callTo.BroadcastTransactionAsync(It.IsAny<Transaction>()), Times.Never);
            Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status404NotFound));
        }

        [Test]
        public async Task BeginSale_SaleIsActive_TransactionNotBroadcastStatusCode409()
        {
            // Arrange
            var address = ValidAddress;
            var requestBody = BeginSaleRequest;

            SetConsensusHeight(999_999);
            SetupContractExists(address);
            SetEndOfSaleToBlock(800_000);

            // Act
            var result = await _managementController.BeginSale(address, requestBody) as ObjectResult;

            // Assert
            _broadcasterManager.Verify(callTo => callTo.BroadcastTransactionAsync(It.IsAny<Transaction>()), Times.Never);
            Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status409Conflict));
        }

        [Test]
        public async Task BeginSale_NoConnections_TransactionNotBroadcastStatusCode403()
        {
            // Arrange
            var address = ValidAddress;
            var requestBody = BeginSaleRequest;

            SetConsensusHeight(999_999);
            SetupContractExists(address);
            SetEndOfSaleToBlock(default);
            SetupZeroConnections();

            // Act
            var result = await _managementController.BeginSale(address, requestBody) as ObjectResult;

            // Assert
            _broadcasterManager.Verify(callTo => callTo.BroadcastTransactionAsync(It.IsAny<Transaction>()), Times.Never);
            Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status403Forbidden));
        }

        [Test]
        public async Task BeginSale_NotAllSeatsAreSupplied_TransactionNotBroadcastStatusCode400()
        {
            // Arrange
            var address = ValidAddress;
            var requestBody = BeginSaleRequest;

            SetConsensusHeight(999_999);
            SetupContractExists(address);
            SetEndOfSaleToBlock(default);
            SetupOneConnection();
            SetTicketsTo(new Ticket[]
            {
                new Ticket
                {
                    Address = Address.Zero,
                    CustomerIdentifier = null,
                    Price = 100_000_000,
                    Seat = new Seat { Number = 1, Letter = 'A' },
                    Secret = null
                },
                new Ticket
                {
                    Address = Address.Zero,
                    CustomerIdentifier = null,
                    Price = 100_000_000,
                    Seat = new Seat { Number = 1, Letter = 'B' },
                    Secret = null
                },
            });

            // Act
            var result = await _managementController.BeginSale(address, requestBody) as ObjectResult;

            // Assert
            _broadcasterManager.Verify(callTo => callTo.BroadcastTransactionAsync(It.IsAny<Transaction>()), Times.Never);
            Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status400BadRequest));
        }

        [Test]
        public async Task BeginSale_BuildCallTransactionUnsuccessful_TransactionNotBroadcastStatusCode400()
        {
            // Arrange
            var address = ValidAddress;
            var requestBody = BeginSaleRequest;

            SetConsensusHeight(999_999);
            SetupContractExists(address);
            SetEndOfSaleToBlock(default);
            SetupOneConnection();
            SetTicketsTo(new Ticket[]
            {
                new Ticket
                {
                    Address = Address.Zero,
                    CustomerIdentifier = null,
                    Price = 100_000_000,
                    Seat = new Seat { Number = 1, Letter = 'A' },
                    Secret = null
                }
            });
            SetupFailedBuildCallContractTransactionRequest();

            // Act
            var result = await _managementController.BeginSale(address, requestBody) as ObjectResult;

            // Assert
            _broadcasterManager.Verify(callTo => callTo.BroadcastTransactionAsync(It.IsAny<Transaction>()), Times.Never);
            Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status400BadRequest));
        }

        [Test]
        public async Task BeginSale_BuildCallTransactionSuccessful_TransactionBroadcast()
        {
            // Arrange
            var address = ValidAddress;
            var requestBody = BeginSaleRequest;

            SetConsensusHeight(999_999);
            SetupContractExists(address);
            SetEndOfSaleToBlock(default);
            SetupOneConnection();
            SetTicketsTo(new Ticket[]
            {
                new Ticket
                {
                    Address = Address.Zero,
                    CustomerIdentifier = null,
                    Price = 100_000_000,
                    Seat = new Seat { Number = 1, Letter = 'A' },
                    Secret = null
                }
            });
            var buildCallTransactionResponse = SetupSuccessfulBuildCallContractTransactionRequest();

            // Act
            var result = await _managementController.BeginSale(address, requestBody) as ObjectResult;

            // Assert
            _broadcasterManager.Verify(callTo => callTo.BroadcastTransactionAsync(It.Is<Transaction>(transaction => transaction.ToHex() == buildCallTransactionResponse.Hex)), Times.Once);
        }

        [Test]
        public async Task BeginSale_CannotBroadcastTransaction_StatusCode500()
        {
            // Arrange
            var address = ValidAddress;
            var requestBody = BeginSaleRequest;

            SetConsensusHeight(999_999);
            SetupContractExists(address);
            SetEndOfSaleToBlock(default);
            SetupOneConnection();
            SetTicketsTo(new Ticket[]
            {
                new Ticket
                {
                    Address = Address.Zero,
                    CustomerIdentifier = null,
                    Price = 100_000_000,
                    Seat = new Seat { Number = 1, Letter = 'A' },
                    Secret = null
                }
            });
            SetupSuccessfulBuildCallContractTransactionRequest();
            SetupCannotBroadcastTransaction();

            // Act
            var result = await _managementController.BeginSale(address, requestBody) as ObjectResult;

            // Assert
            Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status500InternalServerError));
        }

        [Test]
        public async Task BeginSale_TransactionBroadcasted_StatusCode201()
        {
            // Arrange
            var address = ValidAddress;
            var requestBody = BeginSaleRequest;

            SetConsensusHeight(999_999);
            SetupContractExists(address);
            SetEndOfSaleToBlock(default);
            SetupOneConnection();
            SetTicketsTo(new Ticket[]
            {
                new Ticket
                {
                    Address = Address.Zero,
                    CustomerIdentifier = null,
                    Price = 100_000_000,
                    Seat = new Seat { Number = 1, Letter = 'A' },
                    Secret = null
                }
            });
            SetupSuccessfulBuildCallContractTransactionRequest();
            SetupTransactionWasBroadcast();

            // Act
            var result = await _managementController.BeginSale(address, requestBody) as ObjectResult;

            // Assert
            Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status201Created));
        }
    }
}
