using Newtonsoft.Json;
using SaleDeedRegistry.Lib.Client;
using SaleDeedRegistry.Lib.Command;
using SaleDeedRegistry.Lib.Helper;
using System.Threading.Tasks;

namespace SaleDeedRegistry.Lib.Actors
{
    /// <summary>
    /// The Property Buyer "Actor". The buyer is the person who's interested in purchasing the property (land/plot/house etc.) 
    /// from the Propert Owner.
    /// </summary>
    public class PropertyBuyer : BaseActor
    {
        private readonly string buyerAddress = "";
        private readonly string ownerAddress = "";

        public PropertyBuyer(string ownerAddress = "",
            string buyerAddress = "")
        {
            this.buyerAddress = buyerAddress;
            this.ownerAddress = ownerAddress;
        }

        /// <summary>
        /// Get the property buyer address from the configured app key
        /// </summary>
        /// <returns></returns>
        public string GetBuyerAddress()
        {
            if (!string.IsNullOrEmpty(this.buyerAddress))
                return this.buyerAddress;

            return ConfigHelper.GetBuyerAddress;
        }

        /// <summary>
        /// Pay the Application Fee Transfer Fee
        /// </summary>
        /// <returns>ReceiptResponse</returns>
        public async Task<ReceiptResponse> PayTransferFee(string payee, 
            string assetId, int applicationFee = 0)
        {
            SaleDeedTransferFeeRequest transferFeeObject = new SaleDeedTransferFeeRequest
            {
                AssetId = assetId,
                GasPrice = ConfigHelper.GasPrice,
                GasLimit = ConfigHelper.GasLimit,
                Amount = ConfigHelper.Amount,
                Sender = ConfigHelper.GetSenderAddress,
                BuyerAddress = ConfigHelper.GetBuyerAddress,
                FeeAmount = ConfigHelper.GasFee,
                WalletName = ConfigHelper.GetWalletName,
                WalletPassword = ConfigHelper.GetWalletPassword,
                PayeeAddress = payee,
                Fees = applicationFee > 0 ? applicationFee : ConfigHelper.ApplicationFee
            };

            if (!string.IsNullOrEmpty(ownerAddress))
                transferFeeObject.OwnerAddress = ownerAddress;

            if (!string.IsNullOrEmpty(buyerAddress))
                transferFeeObject.BuyerAddress = buyerAddress;

            ReceiptResponse receiptResponse = null;
            SaleRegistryFacade saleRegistryFacade = new SaleRegistryFacade(ConfigHelper.GetSmartContractBaseUrl, ConfigHelper.GetContractAddress);

            System.Console.WriteLine("Trying to execute the SaleDeed -> PayApplicationTransferFee Request");

            var transferFeeResponse = await saleRegistryFacade.PayApplicationTransferFee(transferFeeObject);

            System.Console.WriteLine("Completed executing SaleDeed -> PayApplicationTransferFee Request");

            var commandResponse = JsonConvert.DeserializeObject<CommandResponse>(transferFeeResponse.Content.ReadAsStringAsync().Result);
            
            if (commandResponse.success)
            {
                receiptResponse = await saleRegistryFacade.TryReceiptResponse(commandResponse.transactionId);
                if (receiptResponse != null && receiptResponse.success)
                {
                    Dump(receiptResponse);
                }
                else
                {
                    System.Console.WriteLine("PayTransferFee -> Receipt Response Error");
                    Dump(receiptResponse);
                }
            }

            return receiptResponse;
        }
    }
}
