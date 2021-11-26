using Swashbuckle.AspNetCore.Examples;

namespace Ticketbooth.Api.Requests.Examples
{
    public class EndSaleRequestExample : IExamplesProvider
    {
        private readonly ExampleGenerator _exampleGenerator;

        public EndSaleRequestExample(ExampleGenerator exampleGenerator)
        {
            _exampleGenerator = exampleGenerator;
        }

        public object GetExamples()
        {
            return new EndSaleRequest
            {
                AccountName = "account 0",
                GasPrice = 100,
                Password = "Hunter2",
                Sender = _exampleGenerator.ValidAddress(),
                WalletName = "Wallet One"
            };
        }
    }
}
