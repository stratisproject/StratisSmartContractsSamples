using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NBitcoin;
using NUnit.Framework;
using System.Threading.Tasks;
using Ticketbooth.Api.Requests;

namespace Ticketbooth.Api.Tests.Controllers
{
    public partial class ManagementControllerTests
    {
        private EndSaleRequest EndSaleRequest => new EndSaleRequest
        {
            AccountName = "account 0",
            GasPrice = 100,
            Password = "hunter2",
            Sender = "tNuiVJiEhvbQgXu4P32S4TnAVEG3kgLnu8",
            WalletName = "Wallet One"
        };

        [Test]
        public async Task EndSale_InvalidAddress_TransactionNotBroadcastStatusCode400()
        {
            // Arrange
            var address = InvalidAddress;
            var requestBody = EndSaleRequest;

            // Act
            var result = await _managementController.EndSale(address, requestBody) as ObjectResult;

            // Assert
            _broadcasterManager.Verify(callTo => callTo.BroadcastTransactionAsync(It.IsAny<Transaction>()), Times.Never);
            Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status400BadRequest));
        }

        [Test]
        public async Task EndSale_ValidAddressContractNotFound_TransactionNotBroadcastStatusCode404()
        {
            // Arrange
            var address = ValidAddress;
            var requestBody = EndSaleRequest;

            SetupContractDoesNotExist(address);

            // Act
            var result = await _managementController.EndSale(address, requestBody) as ObjectResult;

            // Assert
            _broadcasterManager.Verify(callTo => callTo.BroadcastTransactionAsync(It.IsAny<Transaction>()), Times.Never);
            Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status404NotFound));
        }

        [Test]
        public async Task EndSale_SaleIsInactive_TransactionNotBroadcastStatusCode409()
        {
            // Arrange
            var address = ValidAddress;
            var requestBody = EndSaleRequest;

            SetupContractExists(address);
            SetEndOfSaleToBlock(default);

            // Act
            var result = await _managementController.EndSale(address, requestBody) as ObjectResult;

            // Assert
            _broadcasterManager.Verify(callTo => callTo.BroadcastTransactionAsync(It.IsAny<Transaction>()), Times.Never);
            Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status409Conflict));
        }

        [Test]
        public async Task EndSale_SaleNotEnded_TransactionNotBroadcastStatusCode409()
        {
            // Arrange
            var address = ValidAddress;
            var requestBody = EndSaleRequest;

            SetupContractExists(address);
            SetEndOfSaleToBlock(1_000_000);
            SetConsensusHeight(999_999);

            // Act
            var result = await _managementController.EndSale(address, requestBody) as ObjectResult;

            // Assert
            _broadcasterManager.Verify(callTo => callTo.BroadcastTransactionAsync(It.IsAny<Transaction>()), Times.Never);
            Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status409Conflict));
        }

        [Test]
        public async Task EndSale_NoConnections_TransactionNotBroadcastStatusCode403()
        {
            // Arrange
            var address = ValidAddress;
            var requestBody = EndSaleRequest;

            SetupContractExists(address);
            SetEndOfSaleToBlock(1_000_000);
            SetConsensusHeight(1_000_001);
            SetupZeroConnections();

            // Act
            var result = await _managementController.EndSale(address, requestBody) as ObjectResult;

            // Assert
            _broadcasterManager.Verify(callTo => callTo.BroadcastTransactionAsync(It.IsAny<Transaction>()), Times.Never);
            Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status403Forbidden));
        }

        [Test]
        public async Task EndSale_BuildCallTransactionUnsuccessful_TransactionNotBroadcastStatusCode400()
        {
            // Arrange
            var address = ValidAddress;
            var requestBody = EndSaleRequest;

            SetupContractExists(address);
            SetEndOfSaleToBlock(1_000_000);
            SetConsensusHeight(1_000_001);
            SetupOneConnection();
            SetupFailedBuildCallContractTransactionRequest();

            // Act
            var result = await _managementController.EndSale(address, requestBody) as ObjectResult;

            // Assert
            _broadcasterManager.Verify(callTo => callTo.BroadcastTransactionAsync(It.IsAny<Transaction>()), Times.Never);
            Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status400BadRequest));
        }

        [Test]
        public async Task EndSale_BuildCallTransactionSuccessful_TransactionBroadcast()
        {
            // Arrange
            var address = ValidAddress;
            var requestBody = EndSaleRequest;

            SetupContractExists(address);
            SetEndOfSaleToBlock(1_000_000);
            SetConsensusHeight(1_000_001);
            SetupOneConnection();
            var buildCallTransactionResponse = SetupSuccessfulBuildCallContractTransactionRequest();

            // Act
            var result = await _managementController.EndSale(address, requestBody) as ObjectResult;

            // Assert
            _broadcasterManager.Verify(callTo => callTo.BroadcastTransactionAsync(It.Is<Transaction>(transaction => transaction.ToHex() == buildCallTransactionResponse.Hex)), Times.Once);
        }

        [Test]
        public async Task EndSale_CannotBroadcastTransaction_StatusCode500()
        {
            // Arrange
            var address = ValidAddress;
            var requestBody = EndSaleRequest;

            SetupContractExists(address);
            SetEndOfSaleToBlock(1_000_000);
            SetConsensusHeight(1_000_001);
            SetupOneConnection();
            SetupSuccessfulBuildCallContractTransactionRequest();
            SetupCannotBroadcastTransaction();

            // Act
            var result = await _managementController.EndSale(address, requestBody) as ObjectResult;

            // Assert
            Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status500InternalServerError));
        }

        [Test]
        public async Task EndSale_TransactionBroadcasted_StatusCode201()
        {
            // Arrange
            var address = ValidAddress;
            var requestBody = EndSaleRequest;

            SetupContractExists(address);
            SetEndOfSaleToBlock(1_000_000);
            SetConsensusHeight(1_000_001);
            SetupOneConnection();
            SetupSuccessfulBuildCallContractTransactionRequest();
            SetupTransactionWasBroadcast();

            // Act
            var result = await _managementController.EndSale(address, requestBody) as ObjectResult;

            // Assert
            Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status201Created));
        }
    }
}
