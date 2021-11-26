using FluentValidation;
using Ticketbooth.Api.Requests;

namespace Ticketbooth.Api.Validation
{
    public class SetTicketReleaseFeeRequestValidator : AbstractValidator<SetTicketReleaseFeeRequest>
    {
        public SetTicketReleaseFeeRequestValidator(IValidator<SmartContractTransactionRequest> smartContractTransactionRequestValidator)
        {
            RuleFor(request => request).SetValidator(smartContractTransactionRequestValidator);
        }
    }
}
