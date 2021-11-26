using System.Configuration;

namespace SaleDeedRegistry.Lib.Helper
{
    /// <summary>
    /// A one stop Configuration Helper. 
    /// Handles all of the Application related configurations.
    /// </summary>
    public static class ConfigHelper
    {
        #region "Gas Constants"

        public static int GasPrice
        {
            get
            {
                return int.Parse(ConfigurationManager.AppSettings.Get("GasPrice"));
            }
        }

        public static int GasLimit
        {
            get
            {
                return int.Parse(ConfigurationManager.AppSettings.Get("GasLimit"));
            }
        }

        public static double GasFee
        {
            get
            {
                return double.Parse(ConfigurationManager.AppSettings.Get("GasFee"));
            }
        }

        public static int Amount
        {
            get
            {
                return int.Parse(ConfigurationManager.AppSettings.Get("Amount"));
            }
        }

        #endregion
        
        #region Addresses

        public static string GetPayeeAddress
        {
            get
            {
                return ConfigurationManager.AppSettings.Get("PayeeAddress");
            }
        }

        public static string GetOwnerAddress
        {
            get
            {
                return ConfigurationManager.AppSettings.Get("OwnerAddress");
            }
        }

        public static string GetSenderAddress
        {
            get
            {
                return ConfigurationManager.AppSettings.Get("SenderAddress");
            }
        }

        public static string GetBuyerAddress
        {
            get
            {
                return ConfigurationManager.AppSettings.Get("BuyerAddress");
            }
        }

        public static string GetContractAddress
        {
            get
            {
                return ConfigurationManager.AppSettings.Get("ContractAddress");
            }
        }

        #endregion

        #region Wallet Related

        public static string GetWalletName
        {
            get
            {
                return ConfigurationManager.AppSettings.Get("WalletName");
            }
        }

        public static string GetWalletPassword
        {
            get
            {
                return ConfigurationManager.AppSettings.Get("WalletPassword");
            }
        }

        public static string GetSmartContractBaseUrl
        {
            get
            {
                return ConfigurationManager.AppSettings.Get("SmartContractBaseUrl");
            }
        }

        #endregion

        #region Others

        public static int ApplicationFee
        {
            get
            {
                return int.Parse(ConfigurationManager.AppSettings.Get("ApplicationFee"));
            }
        }

        #endregion
    }
}
