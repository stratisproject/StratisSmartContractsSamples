using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;
using System.Threading.Tasks;

namespace Ticketbooth.Api.Tests.Controllers
{
    public partial class PublicControllerTests
    {
        [Test]
        public async Task GetIdentityVerificationPolicy_InvalidAddress_StatusCode400()
        {
            // Arrange
            var address = InvalidAddress;

            // Act
            var result = await _publicController.GetIdentityVerificationPolicy(address) as ObjectResult;

            // Assert
            Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status400BadRequest));
        }

        [Test]
        public async Task GetIdentityVerificationPolicy_ValidAddressContractNotFound_StatusCode404()
        {
            // Arrange
            var address = ValidAddress;

            SetupContractDoesNotExist(address);

            // Act
            var result = await _publicController.GetIdentityVerificationPolicy(address) as ObjectResult;

            // Assert
            Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status404NotFound));
        }

        [Test]
        public async Task GetIdentityVerificationPolicy_ContractFound_StatusCode200()
        {
            // Arrange
            var address = ValidAddress;

            SetupContractExists(address);

            // Act
            var result = await _publicController.GetIdentityVerificationPolicy(address) as ObjectResult;

            // Assert
            Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status200OK));
        }

        [Test]
        public async Task GetIdentityVerificationPolicy_ContractFound_ReturnsStorageValue()
        {
            // Arrange
            var address = ValidAddress;

            SetupContractExists(address);
            SetIdentityVerificationPolicyTo(true);

            // Act
            var result = await _publicController.GetIdentityVerificationPolicy(address) as ObjectResult;

            // Assert
            Assert.That(result.Value, Is.EqualTo(true));
        }
    }
}
