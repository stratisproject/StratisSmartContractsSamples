namespace Ticketbooth.Api.Responses
{
    /// <summary>
    /// Cbc cipher parts
    /// </summary>
    public class CbcValues
    {
        /// <summary>
        /// Cipher key
        /// </summary>
        public byte[] Key { get; set; }

        /// <summary>
        /// Initialization vector
        /// </summary>
        public byte[] IV { get; set; }
    }
}
