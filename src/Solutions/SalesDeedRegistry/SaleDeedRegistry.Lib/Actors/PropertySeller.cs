using SaleDeedRegistry.Lib.Helper;

namespace SaleDeedRegistry.Lib.Actors
{
    /// <summary>
    /// Actor - Property Seller 
    /// Seller is the one who's intending to sell his/her land/plot.
    /// </summary>
    public class PropertySeller : BaseActor
    {
        private readonly string sellerAddress;
        public PropertySeller(string sellerAddress = "")
        {
            this.sellerAddress = sellerAddress;
        }

        /// <summary>
        /// Get the property owner address from the configured app key
        /// </summary>
        /// <returns></returns>
        public string GetOwnerAddress()
        {
            if (!string.IsNullOrEmpty(sellerAddress))
                return sellerAddress;
            return ConfigHelper.GetOwnerAddress;
        }
    }
}
