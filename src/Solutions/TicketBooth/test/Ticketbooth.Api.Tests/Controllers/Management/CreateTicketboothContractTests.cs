using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NBitcoin;
using NUnit.Framework;
using System.Threading.Tasks;
using Ticketbooth.Api.Requests;
using static TicketContract;

namespace Ticketbooth.Api.Tests.Controllers
{
    public partial class ManagementControllerTests
    {
        private TicketContractCreateRequest TicketContractCreateRequest => new TicketContractCreateRequest()
        {
            AccountName = "account 0",
            GasPrice = 100,
            Password = "hunter2",
            Seats = new Seat[]
                {
                    new Seat { Number = 1, Letter = 'A' },
                    new Seat { Number = 1, Letter = 'B' },
                    new Seat { Number = 1, Letter = 'C' }
                },
            Sender = "tNuiVJiEhvbQgXu4P32S4TnAVEG3kgLnu8",
            Venue = "Manchester Arena",
            WalletName = "Wallet One"
        };

        [Test]
        public async Task Create_NoConnections_TransactionNotBroadcastStatusCode403()
        {
            // Arrange
            SetupZeroConnections();

            var requestBody = TicketContractCreateRequest;

            // Act
            var result = await _managementController.Create(requestBody) as ObjectResult;

            // Assert
            _broadcasterManager.Verify(callTo => callTo.BroadcastTransactionAsync(It.IsAny<Transaction>()), Times.Never);
            Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status403Forbidden));
        }

        [Test]
        public async Task Create_BuildCreateTransactionUnsuccessful_TransactionNotBroadcastStatusCode400()
        {
            // Arrange
            SetupOneConnection();
            SetupFailedBuildCreateContractTransactionRequest();

            var requestBody = TicketContractCreateRequest;

            // Act
            var result = await _managementController.Create(requestBody) as ObjectResult;

            // Assert
            _broadcasterManager.Verify(callTo => callTo.BroadcastTransactionAsync(It.IsAny<Transaction>()), Times.Never);
            Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status400BadRequest));
        }

        [Test]
        public async Task Create_BuildCreateTransactionSuccessful_TransactionBroadcast()
        {
            // Arrange
            SetupOneConnection();
            var buildCreateTransactionResponse = SetupSuccessfulBuildCreateContractTransactionRequest();

            var requestBody = TicketContractCreateRequest;

            // Act
            var result = await _managementController.Create(requestBody) as ObjectResult;

            // Assert
            _broadcasterManager.Verify(callTo => callTo.BroadcastTransactionAsync(It.Is<Transaction>(transaction => transaction.ToHex() == buildCreateTransactionResponse.Hex)), Times.Once);
        }

        [Test]
        public async Task Create_CannotBroadcastTransaction_StatusCode500()
        {
            // Arrange
            SetupOneConnection();
            SetupSuccessfulBuildCreateContractTransactionRequest();
            SetupCannotBroadcastTransaction();

            var requestBody = TicketContractCreateRequest;

            // Act
            var result = await _managementController.Create(requestBody) as ObjectResult;

            // Assert
            Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status500InternalServerError));
        }

        [Test]
        public async Task Create_BroadcastTransaction_StatusCode201()
        {
            // Arrange
            SetupOneConnection();
            SetupSuccessfulBuildCreateContractTransactionRequest();
            SetupTransactionWasBroadcast();

            var requestBody = TicketContractCreateRequest;

            // Act
            var result = await _managementController.Create(requestBody) as ObjectResult;

            // Assert
            Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status201Created));
        }
    }
}
