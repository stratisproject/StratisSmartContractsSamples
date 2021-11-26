namespace Ticketbooth.Api.Requests
{
    /// <summary>
    /// Request body for request to set identity verification policy
    /// </summary>
    public class SetIdentityVerificationPolicyRequest : SmartContractTransactionRequest
    {
        /// <summary>
        /// Whether the venue requires identity verification
        /// </summary>
        public bool RequireIdentityVerification { get; set; }
    }
}
