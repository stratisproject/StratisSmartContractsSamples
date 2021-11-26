using FluentValidation.TestHelper;
using Moq;
using NBitcoin;
using NUnit.Framework;
using Ticketbooth.Api.Validation;

namespace Ticketbooth.Api.Tests.Validation
{
    public class SetTicketReleaseFeeRequestValidatorTests
    {
        private SetTicketReleaseFeeRequestValidator _validator;

        [SetUp]
        public void SetUp()
        {
            _validator = new SetTicketReleaseFeeRequestValidator(new SmartContractTransactionRequestValidator(Mock.Of<Network>()));
        }

        [Test]
        public void Validate_Transaction()
        {
            _validator.ShouldHaveChildValidator(validator => validator, typeof(SmartContractTransactionRequestValidator));
        }
    }
}
