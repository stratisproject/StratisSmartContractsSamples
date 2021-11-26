using Swashbuckle.AspNetCore.Examples;

namespace Ticketbooth.Api.Responses.Examples
{
    public class TicketReservationDetailsResponseExample : IExamplesProvider
    {
        public object GetExamples()
        {
            return new TicketReservationDetailsResponse
            {
                TransactionHash = "c80b5075fcd6d0d58e235b86556834d7dc71be48e8ee5940071431963b9f47d6",
                Secret = new CbcSecret
                {
                    Plaintext = "h8pqqc2_r90je7v",
                    Key = new byte[32] { 98, 182, 82, 74, 8, 2, 43, 29, 28, 109, 248, 175, 184, 72, 71, 72, 90, 211, 24, 129, 182, 174, 7, 88, 98, 104, 19, 28, 82, 72, 33, 190 },
                    IV = new byte[16] { 82, 74, 27, 110, 129, 18, 231, 248, 27, 90, 8, 22, 123, 8, 7, 21 }
                },
                CustomerName = new CbcValues
                {
                    Key = new byte[32] { 8, 4, 54, 28, 222, 198, 82, 72, 58, 72, 118, 173, 183, 185, 109, 93, 3, 32, 4, 37, 193, 181, 188, 93, 7, 1, 39, 37, 242, 255, 2, 12 },
                    IV = new byte[16] { 73, 71, 8, 191, 38, 32, 235, 28, 2, 10, 13, 39, 112, 32, 31, 9 }
                }
            };
        }
    }
}
