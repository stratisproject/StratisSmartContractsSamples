using FluentValidation;
using Ticketbooth.Api.Requests;

namespace Ticketbooth.Api.Validation
{
    public class EndSaleRequestValidator : AbstractValidator<EndSaleRequest>
    {
        public EndSaleRequestValidator(IValidator<SmartContractTransactionRequest> smartContractTransactionRequestValidator)
        {
            RuleFor(request => request).SetValidator(smartContractTransactionRequestValidator);
        }
    }
}
