using NBitcoin;
using Stratis.SmartContracts;
using Stratis.SmartContracts.CLR;
using Swashbuckle.AspNetCore.Examples;
using static TicketContract;

namespace Ticketbooth.Api.Responses.Examples
{
    public class TicketsResponseExample : IExamplesProvider
    {
        private readonly Network _network;

        public TicketsResponseExample(Network network)
        {
            _network = network;
        }

        public object GetExamples()
        {
            return new Ticket[]
            {
                new Ticket
                {
                    Address = Address.Zero,
                    CustomerIdentifier = null,
                    Price = 230000000,
                    Seat = new Seat
                    {
                        Number = 1,
                        Letter = 'A'
                    },
                    Secret = null
                },
                new Ticket
                {
                    Address = "tU9vpG8bfQxCQxGgHSjFYdb936jddYGrhm".ToAddress(_network),
                    CustomerIdentifier = new byte[16] { 101, 124, 84, 12, 32, 54, 45, 164, 60, 2, 45, 234, 243, 65, 4, 3 },
                    Price = 250000000,
                    Seat = new Seat
                    {
                        Number = 2,
                        Letter = 'A'
                    },
                    Secret = new byte[16] { 25, 36, 3, 255, 225, 81, 78, 37, 37, 35, 211, 64, 23, 8, 53, 35 }
                },
                new Ticket
                {
                    Address = "tU9vpG8bfQxCQxGgHSjFYdb936jddYGrhm".ToAddress(_network),
                    CustomerIdentifier = new byte[16] { 93, 84, 2, 2, 72, 71, 84, 21, 128, 199, 2, 84, 228, 17, 72, 74 },
                    Price = 250000000,
                    Seat = new Seat
                    {
                        Number = 3,
                        Letter = 'A'
                    },
                    Secret = new byte[16] { 125, 47, 83, 111, 112, 114, 10, 92, 110, 28, 47, 82, 82, 9, 2, 24 }
                },
                new Ticket
                {
                    Address = Address.Zero,
                    CustomerIdentifier = null,
                    Price = 230000000,
                    Seat = new Seat
                    {
                        Number = 4,
                        Letter = 'A'
                    },
                    Secret = null
                },
                new Ticket
                {
                    Address = "tU9vpG8bfQxCQxGgHSjFYdb936jddYGrhm".ToAddress(_network),
                    CustomerIdentifier = new byte[16] { 98, 28, 71, 74, 128, 102, 227, 221, 28, 72, 64, 62, 18, 9, 1, 2 },
                    Price = 190000000,
                    Seat = new Seat
                    {
                        Number = 1,
                        Letter = 'B'
                    },
                    Secret = new byte[16] { 192, 92, 83, 71, 74, 2, 77, 4, 18, 82, 71, 142, 172, 172, 73, 17 }
                },
                new Ticket
                {
                    Address = "tU9vpG8bfQxCQxGgHSjFYdb936jddYGrhm".ToAddress(_network),
                    CustomerIdentifier = new byte[16] { 99, 82, 47, 73, 72, 11, 38, 32, 111, 8, 129, 118, 82, 74, 71, 8},
                    Price = 270000000,
                    Seat = new Seat
                    {
                        Number = 2,
                        Letter = 'B'
                    },
                    Secret = new byte[16] { 35, 32, 77, 100, 101, 28, 74, 17, 229, 27, 21, 71, 4, 82, 7, 47 }
                },
                new Ticket
                {
                    Address = "tU9vpG8bfQxCQxGgHSjFYdb936jddYGrhm".ToAddress(_network),
                    CustomerIdentifier = new byte[16] { 143, 34, 111, 13, 22, 55, 34, 41, 111, 52, 89, 33, 58, 28, 119, 9 },
                    Price = 260000000,
                    Seat = new Seat
                    {
                        Number = 3,
                        Letter = 'B'
                    },
                    Secret = new byte[16] { 55, 38, 27, 183, 38, 32, 37, 23, 221, 38, 38, 9, 3, 231, 24, 31 }
                },
                new Ticket
                {
                    Address = "tU9vpG8bfQxCQxGgHSjFYdb936jddYGrhm".ToAddress(_network),
                    CustomerIdentifier = new byte[16] { 81, 83, 47, 121, 32, 12, 32, 221, 24, 29, 18, 200, 29, 93, 8, 2 },
                    Price = 200000000,
                    Seat = new Seat
                    {
                        Number = 4,
                        Letter = 'B'
                    },
                    Secret = new byte[16] { 77, 83, 32, 11, 38, 192, 82, 74, 74, 82, 24, 218, 245, 21, 74, 7 }
                },
            };
        }
    }
}
