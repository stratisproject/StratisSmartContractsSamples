namespace Ticketbooth.Api.Requests
{
    /// <summary>
    /// Request body for request to set ticket release fee
    /// </summary>
    public class SetTicketReleaseFeeRequest : SmartContractTransactionRequest
    {
        /// <summary>
        /// The new ticket release fee
        /// </summary>
        public ulong Fee { get; set; }
    }
}
