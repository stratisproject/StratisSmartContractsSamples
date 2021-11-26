using static TicketContract;

namespace Ticketbooth.Api.Requests
{
    /// <summary>
    /// Request body for request to begin a sale for the ticket contract
    /// </summary>
    public class BeginSaleRequest : SmartContractTransactionRequest
    {
        /// <summary>
        /// Pricing data for each seat on the contract
        /// </summary>
        public SeatPricing[] SeatPrices { get; set; }

        /// <summary>
        /// Details about the sale
        /// </summary>
        public Show Details { get; set; }
    }

    /// <summary>
    /// Stores metadata relating to a specific ticket sale
    /// </summary>
    public class Show
    {
        /// <summary>
        /// Name of the show
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Organiser of the show
        /// </summary>
        public string Organiser { get; set; }

        /// <summary>
        /// Unix time (seconds) of the show
        /// </summary>
        public ulong Time { get; set; }

        /// <summary>
        /// Block height at which the sale ends
        /// </summary>
        public ulong EndOfSale { get; set; }
    }

    /// <summary>
    /// Represents a priced seat
    /// </summary>
    public class SeatPricing
    {
        /// <summary>
        /// The seat identifier
        /// </summary>
        public Seat Seat { get; set; }

        /// <summary>
        /// Cost of the seat in CRS sats
        /// </summary>
        public ulong Price { get; set; }
    }
}
