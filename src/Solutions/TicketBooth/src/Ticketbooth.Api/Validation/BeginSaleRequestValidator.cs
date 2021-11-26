using FluentValidation;
using System.Linq;
using Ticketbooth.Api.Requests;

namespace Ticketbooth.Api.Validation
{
    public class BeginSaleRequestValidator : AbstractValidator<BeginSaleRequest>
    {
        public BeginSaleRequestValidator(IValidator<Show> showValidator, IValidator<SmartContractTransactionRequest> smartContractTransactionValidator)
        {
            RuleFor(request => request).SetValidator(smartContractTransactionValidator);
            RuleFor(request => request.SeatPrices).NotEmpty()
                .DependentRules(() =>
                {
                    RuleFor(request => request.SeatPrices).Must(seatPrices =>
                    {
                        // all seats must be unique
                        return seatPrices.Length == seatPrices.Select(seatPrice => seatPrice.Seat).Distinct().Count();
                    }).WithMessage("Cannot price the same seat more than once");
                });
            RuleFor(request => request.Details).SetValidator(showValidator);
        }
    }
}
