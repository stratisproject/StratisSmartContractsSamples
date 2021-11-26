using static TicketContract;

namespace Ticketbooth.Api.Requests
{
    /// <summary>
    /// Request body for ticket contract creation
    /// </summary>
    public class TicketContractCreateRequest : SmartContractTransactionRequest
    {
        /// <summary>
        /// A unique array of seats that you want to sell in the contract.
        /// </summary>
        public Seat[] Seats { get; set; }

        /// <summary>
        /// Name of the venue. This is used to identify the group of seats.
        /// </summary>
        public string Venue { get; set; }
    }
}
