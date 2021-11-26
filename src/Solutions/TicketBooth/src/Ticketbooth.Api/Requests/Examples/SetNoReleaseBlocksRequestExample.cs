using Swashbuckle.AspNetCore.Examples;

namespace Ticketbooth.Api.Requests.Examples
{
    public class SetNoReleaseBlocksRequestExample : IExamplesProvider
    {
        private readonly ExampleGenerator _exampleGenerator;

        public SetNoReleaseBlocksRequestExample(ExampleGenerator exampleGenerator)
        {
            _exampleGenerator = exampleGenerator;
        }

        public object GetExamples()
        {
            return new SetNoReleaseBlocksRequest
            {
                AccountName = "account 0",
                Count = 5000,
                GasPrice = 100,
                Password = "Hunter2",
                Sender = _exampleGenerator.ValidAddress(),
                WalletName = "Wallet One"
            };
        }
    }
}
