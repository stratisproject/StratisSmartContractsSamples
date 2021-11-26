using FluentValidation;
using Ticketbooth.Api.Requests;

namespace Ticketbooth.Api.Validation
{
    public class SetNoReleaseBlocksRequestValidator : AbstractValidator<SetNoReleaseBlocksRequest>
    {
        public SetNoReleaseBlocksRequestValidator(IValidator<SmartContractTransactionRequest> smartContractTransactionRequestValidator)
        {
            RuleFor(request => request).SetValidator(smartContractTransactionRequestValidator);
        }
    }
}
