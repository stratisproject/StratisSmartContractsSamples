using FluentValidation;
using System.Linq;
using Ticketbooth.Api.Requests;

namespace Ticketbooth.Api.Validation
{
    public class TicketContractCreateRequestValidator : AbstractValidator<TicketContractCreateRequest>
    {
        public TicketContractCreateRequestValidator(IValidator<SmartContractTransactionRequest> smartContractTransactionRequestValidator)
        {
            RuleFor(request => request).SetValidator(smartContractTransactionRequestValidator);
            RuleFor(request => request.Seats).NotEmpty()
                .DependentRules(() =>
                {
                    RuleFor(request => request.Seats).Must(seats =>
                    {
                        if (seats.Length > TicketContract.MAX_SEATS)
                        {
                            return false;
                        }

                        // checks if seats are unique
                        return seats.Distinct().Count() == seats.Length;
                    }).WithMessage($"Must specify a maximum of {TicketContract.MAX_SEATS} unique seats");
                });
            RuleFor(request => request.Venue).NotEmpty();
        }
    }
}
