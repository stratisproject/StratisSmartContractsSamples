using FluentValidation;
using System;
using Ticketbooth.Api.Requests;

namespace Ticketbooth.Api.Validation
{
    public class ShowValidatior : AbstractValidator<Show>
    {
        public ShowValidatior()
        {
            RuleFor(request => request.Name).NotEmpty();
            RuleFor(request => request.Organiser).NotEmpty();
            RuleFor(request => request.Time).GreaterThan((ulong)DateTimeOffset.UtcNow.ToUnixTimeSeconds()).WithMessage("Time must be in the future");
        }
    }
}
