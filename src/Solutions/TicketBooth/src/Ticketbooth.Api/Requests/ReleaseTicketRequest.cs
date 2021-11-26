using static TicketContract;

namespace Ticketbooth.Api.Requests
{
    /// <summary>
    /// Request body for request to release ticket to ticket contract
    /// </summary>
    public class ReleaseTicketRequest : SmartContractTransactionRequest
    {
        /// <summary>
        /// The seat of the ticket
        /// </summary>
        public Seat Seat { get; set; }
    }
}
