using SaleDeedRegistry.Lib.Helper;

namespace SaleDeedRegistry.Lib.Actors
{
    /// <summary>
    /// Actor - Payee
    /// </summary>
    public class Payee : BaseActor
    {
        public Payee()
        {

        }

        /// <summary>
        /// Get the payee address from the configured app key
        /// </summary>
        /// <returns></returns>
        public string GetPayee()
        {
            return ConfigHelper.GetPayeeAddress;
        }
    }
}
