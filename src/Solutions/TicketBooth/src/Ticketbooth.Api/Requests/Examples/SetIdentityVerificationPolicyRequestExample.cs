using Swashbuckle.AspNetCore.Examples;

namespace Ticketbooth.Api.Requests.Examples
{
    public class SetIdentityVerificationPolicyRequestExample : IExamplesProvider
    {
        private readonly ExampleGenerator _exampleGenerator;

        public SetIdentityVerificationPolicyRequestExample(ExampleGenerator exampleGenerator)
        {
            _exampleGenerator = exampleGenerator;
        }

        public object GetExamples()
        {
            return new SetIdentityVerificationPolicyRequest
            {
                AccountName = "account 0",
                GasPrice = 100,
                Password = "Hunter2",
                RequireIdentityVerification = true,
                Sender = _exampleGenerator.ValidAddress(),
                WalletName = "Wallet One"
            };
        }
    }
}
