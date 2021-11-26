using FluentValidation.TestHelper;
using Moq;
using NBitcoin;
using NUnit.Framework;
using System;
using Ticketbooth.Api.Validation;
using static TicketContract;

namespace Ticketbooth.Api.Tests.Validation
{
    public class TicketContractCreateRequestValidatorTests
    {
        private TicketContractCreateRequestValidator _validator;

        [SetUp]
        public void SetUp()
        {
            _validator = new TicketContractCreateRequestValidator(new SmartContractTransactionRequestValidator(Mock.Of<Network>()));
        }

        [Test]
        public void Validate_Transaction()
        {
            _validator.ShouldHaveChildValidator(validator => validator, typeof(SmartContractTransactionRequestValidator));
        }

        [Test]
        public void Validation_Seats()
        {
            // Invalid
            _validator.ShouldHaveValidationErrorFor(validator => validator.Seats, Array.Empty<Seat>());
            _validator.ShouldHaveValidationErrorFor(
                validator => validator.Seats,
                new Seat[]
                {
                    new Seat { Number = 5 },
                    new Seat { Number = 5 },
                });
            _validator.ShouldHaveValidationErrorFor(
                validator => validator.Seats,
                new Seat[]
                {
                    new Seat { Letter = 'C' },
                    new Seat { Letter = 'C' },
                });
            _validator.ShouldHaveValidationErrorFor(
                validator => validator.Seats,
                new Seat[]
                {
                    new Seat { Number = 1, Letter = 'A' },
                    new Seat { Number = 1, Letter = 'A' },
                });

            var tooManySeats = new Seat[66];
            for (int i = 0; i < tooManySeats.Length; i++)
            {
                tooManySeats[i] = new Seat { Number = i + 1 };
            }

            _validator.ShouldHaveValidationErrorFor(validator => validator.Seats, tooManySeats);

            // Valid
            _validator.ShouldNotHaveValidationErrorFor(
                validator => validator.Seats,
                new Seat[]
                {
                    new Seat { Number = 1, Letter = 'A' },
                    new Seat { Number = 1, Letter = 'B' },
                });
        }

        [Test]
        public void Validation_Venue()
        {
            // Invalid
            _validator.ShouldHaveValidationErrorFor(validator => validator.Venue, null as string);
            _validator.ShouldHaveValidationErrorFor(validator => validator.Venue, string.Empty);
            _validator.ShouldHaveValidationErrorFor(validator => validator.Venue, "    ");

            // Valid
            _validator.ShouldNotHaveValidationErrorFor(validator => validator.Venue, "Hello world");
        }
    }
}
