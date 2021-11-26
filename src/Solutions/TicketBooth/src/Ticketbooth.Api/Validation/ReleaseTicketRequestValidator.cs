using FluentValidation;
using Ticketbooth.Api.Requests;

namespace Ticketbooth.Api.Validation
{
    public class ReleaseTicketRequestValidator : AbstractValidator<ReleaseTicketRequest>
    {
        public ReleaseTicketRequestValidator(IValidator<SmartContractTransactionRequest> smartContractTransactionRequestValidator)
        {
            RuleFor(request => request).SetValidator(smartContractTransactionRequestValidator);
        }
    }
}
