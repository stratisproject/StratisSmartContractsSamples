using static TicketContract;

namespace Ticketbooth.Api.Requests
{
    /// <summary>
    /// Request body for request to purchase a ticket
    /// </summary>
    public class ReserveTicketRequest : SmartContractTransactionRequest
    {
        /// <summary>
        /// The seat of the ticket
        /// </summary>
        public Seat Seat { get; set; }

        /// <summary>
        /// The name of the customer, required if there is an identity verification policy
        /// </summary>
        public string CustomerName { get; set; }
    }
}
