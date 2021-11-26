using Swashbuckle.AspNetCore.Examples;
using static TicketContract;

namespace Ticketbooth.Api.Requests.Examples
{
    public class ReserveTicketRequestExample : IExamplesProvider
    {
        private readonly ExampleGenerator _exampleGenerator;

        public ReserveTicketRequestExample(ExampleGenerator exampleGenerator)
        {
            _exampleGenerator = exampleGenerator;
        }

        public object GetExamples()
        {
            return new ReserveTicketRequest
            {
                AccountName = "account 0",
                CustomerName = "Benjamin Swift",
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
