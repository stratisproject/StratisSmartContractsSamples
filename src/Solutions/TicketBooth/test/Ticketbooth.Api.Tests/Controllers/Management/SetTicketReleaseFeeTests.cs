using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NBitcoin;
using NUnit.Framework;
using Stratis.Bitcoin.Features.SmartContracts.Models;
using System.Threading.Tasks;
using Ticketbooth.Api.Requests;

namespace Ticketbooth.Api.Tests.Controllers
{
    public partial class ManagementControllerTests
    {
        private SetTicketReleaseFeeRequest SetTicketReleaseFeeRequest => new SetTicketReleaseFeeRequest
        {
            AccountName = "account 0",
            GasPrice = 100,
            Fee = 100_000_000,
            Password = "hunter2",
            Sender = "tNuiVJiEhvbQgXu4P32S4TnAVEG3kgLnu8",
            WalletName = "Wallet One"
        };

        [Test]
        public async Task SetTicketReleaseFee_InvalidAddress_TransactionNotBroadcastStatusCode400()
        {
            // Arrange
            var address = InvalidAddress;
            var requestBody = SetTicketReleaseFeeRequest;

            // Act
            var result = await _managementController.SetTicketReleaseFee(address, requestBody) as ObjectResult;

            // Assert
            _broadcasterManager.Verify(callTo => callTo.BroadcastTransactionAsync(It.IsAny<Transaction>()), Times.Never);
            Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status400BadRequest));
        }


        [Test]
        public async Task SetTicketReleaseFee_ValidAddressContractNotFound_TransactionNotBroadcastStatusCode404()
        {
            // Arrange
            var address = ValidAddress;
            var requestBody = SetTicketReleaseFeeRequest;

            SetupContractDoesNotExist(address);

            // Act
            var result = await _managementController.SetTicketReleaseFee(address, requestBody) as ObjectResult;

            // Assert
            _broadcasterManager.Verify(callTo => callTo.BroadcastTransactionAsync(It.IsAny<Transaction>()), Times.Never);
            Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status404NotFound));
        }

        [Test]
        public async Task SetTicketReleaseFee_SaleIsActive_TransactionNotBroadcastStatusCode409()
        {
            // Arrange
            var address = ValidAddress;
            var requestBody = SetTicketReleaseFeeRequest;

            SetupContractExists(address);
            SetEndOfSaleToBlock(1_000_000);

            // Act
            var result = await _managementController.SetTicketReleaseFee(address, requestBody) as ObjectResult;

            // Assert
            _broadcasterManager.Verify(callTo => callTo.BroadcastTransactionAsync(It.IsAny<Transaction>()), Times.Never);
            Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status409Conflict));
        }

        [Test]
        public async Task SetTicketReleaseFee_NoConnections_TransactionNotBroadcastStatusCode403()
        {
            // Arrange
            var address = ValidAddress;
            var requestBody = SetTicketReleaseFeeRequest;

            SetupContractExists(address);
            SetEndOfSaleToBlock(default);
            SetupZeroConnections();

            // Act
            var result = await _managementController.SetTicketReleaseFee(address, requestBody) as ObjectResult;

            // Assert
            _broadcasterManager.Verify(callTo => callTo.BroadcastTransactionAsync(It.IsAny<Transaction>()), Times.Never);
            Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status403Forbidden));
        }

        [Test]
        public async Task SetTicketReleaseFee_BuildCallTransactionUnsuccessful_TransactionNotBroadcastStatusCode400()
        {
            // Arrange
            var address = ValidAddress;
            var requestBody = SetTicketReleaseFeeRequest;
            var buildCallTransactionResponse = BuildCallContractTransactionResponse.Failed("Testing unsuccessful transaction");

            SetupContractExists(address);
            SetEndOfSaleToBlock(default);
            SetupOneConnection();
            SetupFailedBuildCallContractTransactionRequest();

            // Act
            var result = await _managementController.SetTicketReleaseFee(address, requestBody) as ObjectResult;

            // Assert
            _broadcasterManager.Verify(callTo => callTo.BroadcastTransactionAsync(It.IsAny<Transaction>()), Times.Never);
            Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status400BadRequest));
        }

        [Test]
        public async Task SetTicketReleaseFee_BuildCallTransactionSuccessful_TransactionBroadcast()
        {
            // Arrange
            var address = ValidAddress;
            var requestBody = SetTicketReleaseFeeRequest;

            SetupContractExists(address);
            SetEndOfSaleToBlock(default);
            SetupOneConnection();
            var buildCallTransactionResponse = SetupSuccessfulBuildCallContractTransactionRequest();

            // Act
            var result = await _managementController.SetTicketReleaseFee(address, requestBody) as ObjectResult;

            // Assert
            _broadcasterManager.Verify(callTo => callTo.BroadcastTransactionAsync(It.Is<Transaction>(transaction => transaction.ToHex() == buildCallTransactionResponse.Hex)), Times.Once);
        }

        [Test]
        public async Task SetTicketReleaseFee_CannotBroadcastTransaction_StatusCode500()
        {
            // Arrange
            var address = ValidAddress;
            var requestBody = SetTicketReleaseFeeRequest;

            SetupContractExists(address);
            SetEndOfSaleToBlock(default);
            SetupOneConnection();
            SetupSuccessfulBuildCallContractTransactionRequest();
            SetupCannotBroadcastTransaction();

            // Act
            var result = await _managementController.SetTicketReleaseFee(address, requestBody) as ObjectResult;

            // Assert
            Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status500InternalServerError));
        }

        [Test]
        public async Task SetTicketReleaseFee_TransactionBroadcasted_StatusCode201()
        {
            // Arrange
            var address = ValidAddress;
            var requestBody = SetTicketReleaseFeeRequest;

            SetupContractExists(address);
            SetEndOfSaleToBlock(default);
            SetupOneConnection();
            SetupSuccessfulBuildCallContractTransactionRequest();
            SetupTransactionWasBroadcast();

            // Act
            var result = await _managementController.SetTicketReleaseFee(address, requestBody) as ObjectResult;

            // Assert
            Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status201Created));
        }
    }
}
