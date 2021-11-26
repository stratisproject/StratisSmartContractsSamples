namespace Ticketbooth.Api.Requests
{
    /// <summary>
    /// Request body for request to set final no release blocks
    /// </summary>
    public class SetNoReleaseBlocksRequest : SmartContractTransactionRequest
    {
        /// <summary>
        /// The number of blocks before the end of a sale, where ticket release is not allowed
        /// </summary>
        public ulong Count { get; set; }
    }
}
