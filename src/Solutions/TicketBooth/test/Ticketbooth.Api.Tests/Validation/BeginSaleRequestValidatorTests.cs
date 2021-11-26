using FluentValidation.TestHelper;
using Moq;
using NBitcoin;
using NUnit.Framework;
using Ticketbooth.Api.Requests;
using Ticketbooth.Api.Validation;
using static TicketContract;

namespace Ticketbooth.Api.Tests.Validation
{
    public class BeginSaleRequestValidatorTests
    {
        private BeginSaleRequestValidator _validator;

        [SetUp]
        public void SetUp()
        {
            _validator = new BeginSaleRequestValidator(new ShowValidatior(), new SmartContractTransactionRequestValidator(Mock.Of<Network>()));
        }

        [Test]
        public void Validate_Transaction()
        {
            _validator.ShouldHaveChildValidator(validator => validator, typeof(SmartContractTransactionRequestValidator));
        }

        [Test]
        public void Validate_SeatPrices()
        {
            // Invalid
            _validator.ShouldHaveValidationErrorFor(validator => validator.SeatPrices, null as SeatPricing[]);
            _validator.ShouldHaveValidationErrorFor(validator => validator.SeatPrices, new SeatPricing[0]);
            _validator.ShouldHaveValidationErrorFor(validator => validator.SeatPrices,
                new SeatPricing[]
                {
                    new SeatPricing {
                        Seat = new Seat { Number = 1 },
                        Price = 500000000
                    },
                    new SeatPricing {
                        Seat = new Seat { Number = 1 },
                        Price = 450000000
                    },
                });

            // Valid
            _validator.ShouldNotHaveValidationErrorFor(validator => validator.SeatPrices,
                new SeatPricing[]
                {
                    new SeatPricing {
                        Seat = new Seat { Number = 1 },
                        Price = 500000000
                    },
                    new SeatPricing {
                        Seat = new Seat { Number = 2 },
                        Price = 800000000
                    },
                });
        }

        [Test]
        public void Validate_Details()
        {
            _validator.ShouldHaveChildValidator(validator => validator.Details, typeof(ShowValidatior));
        }
    }
}
