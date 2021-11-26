using System;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SaleDeedRegistry.Lib.Client;

namespace SaleDeedRegistry.Lib.Command
{
    /// <summary>
    /// The SaleDeedRegistry Complete Review Command
    /// </summary>
    public class CompleteReviewCommand : ISaleDeedRegistryCommand
    {
        private readonly string smartContractUrl;
        private readonly string smartContractAddress;
        private readonly SaleRegistryFacade saleRegistryFacade;

        public CompleteReviewCommand(string contractUrl, string contractAddress)
        {
            smartContractUrl = contractUrl;
            smartContractAddress = contractAddress;
            saleRegistryFacade = new SaleRegistryFacade(smartContractUrl, smartContractAddress);
        }

        public async Task<ReceiptResponse> Execute(SaleDeedRegistryBaseRequest requestObject)
        {
            try
            {
                SaleRegistryFacade saleRegistryFacade = new SaleRegistryFacade(smartContractUrl, smartContractAddress);
                var response = saleRegistryFacade.CompleteReview((SaleDeedRegistryRequest)requestObject).Result;
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
