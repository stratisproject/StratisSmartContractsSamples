using FluentValidation;
using Ticketbooth.Api.Requests;

namespace Ticketbooth.Api.Validation
{
    public class SetIdentityVerificationPolicyRequestValidator : AbstractValidator<SetIdentityVerificationPolicyRequest>
    {
        public SetIdentityVerificationPolicyRequestValidator(IValidator<SmartContractTransactionRequest> smartContractTransactionRequestValidator)
        {
            RuleFor(request => request).SetValidator(smartContractTransactionRequestValidator);
        }
    }
}
