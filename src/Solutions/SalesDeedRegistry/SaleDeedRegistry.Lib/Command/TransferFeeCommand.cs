using System;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SaleDeedRegistry.Lib.Client;

namespace SaleDeedRegistry.Lib.Command
{
    /// <summary>
    /// The SaleDeedRegistry Application Transfer Fee Command
    /// </summary>
    public class TransferFeeCommand : ISaleDeedRegistryCommand
    {
        private readonly string smartContractUrl;
        private readonly string smartContractAddress;
        private readonly SaleRegistryFacade saleRegistryFacade;

        public TransferFeeCommand(string contractUrl, string contractAddress)
        {
            smartContractUrl = contractUrl;
            smartContractAddress = contractAddress;
            saleRegistryFacade = new SaleRegistryFacade(smartContractUrl, smartContractAddress);
        }

        public async Task<ReceiptResponse> Execute(SaleDeedRegistryBaseRequest requestObject)
        { 
            try
            {
                var response = saleRegistryFacade.PayApplicationTransferFee((SaleDeedTransferFeeRequest)requestObject).Result;
                if (response.IsSuccessStatusCode)
                {
                    string json = await response.Content.ReadAsStringAsync();
                    var commandResponse = JsonConvert.DeserializeObject<CommandResponse>(json);
                    return await TryGetReceiptResponse(commandResponse.transactionId);
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return null;
        }

        private async Task<ReceiptResponse> TryGetReceiptResponse(string transactionId)
        {
            var receiptResponse = await saleRegistryFacade.TryReceiptResponse(transactionId);
            if (receiptResponse != null && receiptResponse.success)
            {
                return receiptResponse;
            }
            return null;
        }
    }
}
