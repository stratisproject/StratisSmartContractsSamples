using Swashbuckle.AspNetCore.Examples;
using static TicketContract;

namespace Ticketbooth.Api.Requests.Examples
{
    public class TicketContractCreateRequestExample : IExamplesProvider
    {
        private readonly ExampleGenerator _exampleGenerator;

        public TicketContractCreateRequestExample(ExampleGenerator exampleGenerator)
        {
            _exampleGenerator = exampleGenerator;
        }

        public object GetExamples()
        {
            return new TicketContractCreateRequest
            {
                AccountName = "account 0",
                GasPrice = 100,
                Password = "Hunter2",
                Seats = new Seat[]
                {
                    new Seat
                    {
                        Number = 1,
                        Letter = 'A'
                    },
                    new Seat
                    {
                        Number = 2,
                        Letter = 'A'
                    },
                    new Seat
                    {
                        Number = 3,
                        Letter = 'A'
                    },
                    new Seat
                    {
                        Number = 4,
                        Letter = 'A'
                    },
                    new Seat
                    {
                        Number = 1,
                        Letter = 'B'
                    },
                    new Seat
                    {
                        Number = 2,
                        Letter = 'B'
                    },
                    new Seat
                    {
                        Number = 3,
                        Letter = 'B'
                    },
                    new Seat
                    {
                        Number = 4,
                        Letter = 'B'
                    },
                },
                Sender = _exampleGenerator.ValidAddress(),
                Venue = "Manchester Arena",
                WalletName = "Wallet One"
            };
        }
    }
}
