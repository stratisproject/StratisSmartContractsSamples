using Swashbuckle.AspNetCore.Examples;
using static TicketContract;

namespace Ticketbooth.Api.Requests.Examples
{
    public class BeginSaleRequestExample : IExamplesProvider
    {
        private readonly ExampleGenerator _exampleGenerator;

        public BeginSaleRequestExample(ExampleGenerator exampleGenerator)
        {
            _exampleGenerator = exampleGenerator;
        }

        public object GetExamples()
        {
            return new BeginSaleRequest
            {
                AccountName = "account 0",
                Details = new Show
                {
                    Name = "Greatest Hits Tour",
                    Organiser = "Rick Astley",
                    Time = 1895075621,
                    EndOfSale = 100000
                },
                GasPrice = 100,
                Password = "Hunter2",
                SeatPrices = new SeatPricing[]
                {
                    new SeatPricing
                    {
                        Seat = new Seat
                        {
                            Number = 1,
                            Letter = 'A'
                        },
                        Price = 25000000
                    },
                    new SeatPricing
                    {
                        Seat = new Seat
                        {
                            Number = 2,
                            Letter = 'A'
                        },
                        Price = 23000000
                    },
                    new SeatPricing
                    {
                        Seat = new Seat
                        {
                            Number = 3,
                            Letter = 'A'
                        },
                        Price = 25000000
                    },
                    new SeatPricing
                    {
                        Seat = new Seat
                        {
                            Number = 4,
                            Letter = 'A'
                        },
                        Price = 25000000
                    },
                    new SeatPricing
                    {
                        Seat = new Seat
                        {
                            Number = 1,
                            Letter = 'B'
                        },
                        Price = 30000000
                    },
                    new SeatPricing
                    {
                        Seat = new Seat
                        {
                            Number = 2,
                            Letter = 'B'
                        },
                        Price = 37000000
                    },
                    new SeatPricing
                    {
                        Seat = new Seat
                        {
                            Number = 3,
                            Letter = 'B'
                        },
                        Price = 35000000
                    },
                    new SeatPricing
                    {
                        Seat = new Seat
                        {
                            Number = 4,
                            Letter = 'B'
                        },
                        Price = 32000000
                    }
                },
                Sender = _exampleGenerator.ValidAddress(),
                WalletName = "Wallet One",
            };
        }
    }
}
