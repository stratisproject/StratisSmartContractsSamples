using Swashbuckle.AspNetCore.Examples;

namespace Ticketbooth.Api.Requests.Examples
{
    public class SetTicketReleaseFeeRequestExample : IExamplesProvider
    {
        private readonly ExampleGenerator _exampleGenerator;

        public SetTicketReleaseFeeRequestExample(ExampleGenerator exampleGenerator)
        {
            _exampleGenerator = exampleGenerator;
        }

        public object GetExamples()
        {
            return new SetTicketReleaseFeeRequest
            {
                AccountName = "account 0",
                Fee = 100000000,
                GasPrice = 100,
                Password = "Hunter2",
                Sender = _exampleGenerator.ValidAddress(),
                WalletName = "Wallet One"
            };
        }
    }
}
