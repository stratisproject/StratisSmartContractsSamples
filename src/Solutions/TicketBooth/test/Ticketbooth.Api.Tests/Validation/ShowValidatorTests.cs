using FluentValidation.TestHelper;
using NUnit.Framework;
using System;
using Ticketbooth.Api.Validation;

namespace Ticketbooth.Api.Tests.Validation
{
    public class ShowValidatorTests
    {
        private ShowValidatior _validator;

        [SetUp]
        public void SetUp()
        {
            _validator = new ShowValidatior();
        }

        [Test]
        public void Validate_Name()
        {
            // Invalid
            _validator.ShouldHaveValidationErrorFor(validator => validator.Name, null as string);
            _validator.ShouldHaveValidationErrorFor(validator => validator.Name, string.Empty);
            _validator.ShouldHaveValidationErrorFor(validator => validator.Name, "   ");

            // Valid
            _validator.ShouldNotHaveValidationErrorFor(validator => validator.Name, "Hello world");
        }

        [Test]
        public void Validate_Organiser()
        {
            // Invalid
            _validator.ShouldHaveValidationErrorFor(validator => validator.Organiser, null as string);
            _validator.ShouldHaveValidationErrorFor(validator => validator.Organiser, string.Empty);
            _validator.ShouldHaveValidationErrorFor(validator => validator.Organiser, "   ");

            // Valid
            _validator.ShouldNotHaveValidationErrorFor(validator => validator.Organiser, "Rick Astley");
        }

        [Test]
        public void Validate_Time()
        {
            // Invalid
            _validator.ShouldHaveValidationErrorFor(validator => validator.Time, (ulong)0);
            _validator.ShouldHaveValidationErrorFor(validator => validator.Time, (ulong)DateTimeOffset.UtcNow.ToUnixTimeSeconds());

            // Valid
            _validator.ShouldNotHaveValidationErrorFor(validator => validator.Time, (ulong)DateTimeOffset.UtcNow.AddSeconds(10).ToUnixTimeSeconds());
        }
    }
}
