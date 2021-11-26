using Swashbuckle.AspNetCore.Examples;
using static TicketContract;

namespace Ticketbooth.Api.Requests.Examples
{
    public class ReleaseTicketRequestExample : IExamplesProvider
    {
        private readonly ExampleGenerator _exampleGenerator;

        public ReleaseTicketRequestExample(ExampleGenerator exampleGenerator)
        {
            _exampleGenerator = exampleGenerator;
        }

        public object GetExamples()
        {
            return new ReleaseTicketRequest
            {
                AccountName = "account 0",
                GasPrice = 100,
                Password = "Hunter2",
                Seat = new Seat
                {
                    Number = 3,
                    Letter = 'D'
                },
                Sender = _exampleGenerator.ValidAddress(),
                WalletName = "Wallet One"
            };
        }
    }
}
