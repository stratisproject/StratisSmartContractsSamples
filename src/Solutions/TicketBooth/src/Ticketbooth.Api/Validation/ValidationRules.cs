using FluentValidation;
using NBitcoin;
using System;

namespace Ticketbooth.Api.Validation
{
    public static class ValidationRules
    {
        public static IRuleBuilderOptions<T, string> IsAddress<T>(this IRuleBuilder<T, string> ruleBuilder, Network network)
        {
            return ruleBuilder.Must(value =>
            {
                if (value is null)
                {
                    return false;
                }

                try
                {
                    BitcoinAddress.Create(value, network);
                    return true;
                }
                catch (Exception)
                {
                    return false;
                }
            }).WithMessage($"Address must be valid for {network.Name}");
        }
    }
}
