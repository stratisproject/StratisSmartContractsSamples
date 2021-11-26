using static TicketContract;

namespace Ticketbooth.Api.Tools
{
    public static class SeatExtensions
    {
        public static string ToDisplayString(this Seat seat)
        {
            if (seat.Number == default && seat.Letter == default)
            {
                return "UNDEFINED";
            }

            if (seat.Number == default)
            {
                return seat.Letter.ToString();
            }

            if (seat.Letter == default)
            {
                return seat.Number.ToString();
            }

            return $"{seat.Number}{seat.Letter}";
        }
    }
}
