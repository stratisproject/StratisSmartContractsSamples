using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;
using Stratis.SmartContracts;
using System.Threading.Tasks;
using static TicketContract;

namespace Ticketbooth.Api.Tests.Controllers
{
    public partial class PublicControllerTests
    {
        [Test]
        public async Task GetTickets_InvalidAddress_StatusCode400()
        {
            // Arrange
            var address = InvalidAddress;

            // Act
            var result = await _publicController.GetTickets(address) as ObjectResult;

            // Assert
            Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status400BadRequest));
        }

        [Test]
        public async Task GetTickets_ValidAddressContractNotFound_StatusCode404()
        {
            // Arrange
            var address = ValidAddress;

            SetupContractDoesNotExist(address);

            // Act
            var result = await _publicController.GetTickets(address) as ObjectResult;

            // Assert
            Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status404NotFound));
        }

        [Test]
        public async Task GetTickets_ContractFound_StatusCode200()
        {
            // Arrange
            var address = ValidAddress;

            SetupContractExists(address);

            // Act
            var result = await _publicController.GetTickets(address) as ObjectResult;

            // Assert
            Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status200OK));
        }

        [Test]
        public async Task GetTickets_ContractFound_ReturnsStorageValue()
        {
            // Arrange
            var address = ValidAddress;
            var tickets = new Ticket[]
            {
                new Ticket
                {
                    Address = Address.Zero,
                    CustomerIdentifier = new byte[] { 13, 28, 13, 82 },
                    Price = 100_000_000,
                    Seat = new Seat { Number = 1 },
                    Secret = new byte[] { 29, 182, 83, 7 }
                }
            };

            SetupContractExists(address);
            SetTicketsTo(tickets);

            // Act
            var result = await _publicController.GetTickets(address) as ObjectResult;

            // Assert
            Assert.That(result.Value, Is.EqualTo(tickets));
        }
    }
}
