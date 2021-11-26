using Newtonsoft.Json;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using SaleDeedRegistry.Lib.Client;

using System;
using System.Threading;
using System.Dynamic;

namespace SaleDeedRegistry.Lib
{
    public class SaleDeedRegistryRequest : SaleDeedRegistryBaseRequest
    {
    }

    public class SaleDeedTransferFeeRequest : SaleDeedRegistryBaseRequest
    {
        public string PayeeAddress { get; set; }
        public int Fees { get; set; }
    }

    public class SaleDeedTransferOwnershipRequest : SaleDeedRegistryBaseRequest
    {
        
    }

    /// <summary>
    /// The Sale Registry Application Init
    /// </summary>
    public class SaleRegistryFacade
    {
        private readonly string baseUrl;
        private readonly string contractAddress;

        public SaleRegistryFacade(string baseUrl, string contractAddress)
        {
            this.baseUrl = baseUrl;
            this.contractAddress = contractAddress;
        }
        
        /// <summary>
        /// Init the sale deed application process
        /// </summary>
        /// <param name="requestObject">RequestObject</param>
        /// <returns>HttpResponseMessage</returns>
        public async Task<HttpResponseMessage> Init(SaleDeedRegistryRequest requestObject)
        {
            return await Execute(requestObject, "InitApplication");
        }

        /// <summary>
        /// Start Review Process 
        /// </summary>
        /// <param name="requestObject">RequestObject</param>
        /// <returns>HttpResponseMessage</returns>
        public async Task<HttpResponseMessage> StartReview(SaleDeedRegistryRequest requestObject)
        {
            return await Execute(requestObject, "StartReviewProcess");
        }

        /// <summary>
        /// Complete Review Process 
        /// </summary>
        /// <param name="requestObject">RequestObject</param>
        /// <returns>HttpResponseMessage</returns>
        public async Task<HttpResponseMessage> CompleteReview(SaleDeedRegistryRequest requestObject)
        {
            return await Execute(requestObject, "CompleteReviewProcess");
        }

        /// <summary>
        /// Reject the Application
        /// </summary>
        /// <param name="requestObject">RequestObject</param>
        /// <returns>HttpResponseMessage</returns>
        public async Task<HttpResponseMessage> RejectApplication(SaleDeedRegistryRequest requestObject)
        {
            return await Execute(requestObject, "RejectApplication");
        }

        /// <summary>
        /// Re-Apply Application
        /// </summary>
        /// <param name="requestObject">RequestObject</param>
        /// <returns>HttpResponseMessage</returns>
        public async Task<HttpResponseMessage> ReApplyApplication(SaleDeedRegistryRequest requestObject)
        {
            return await Execute(requestObject, "ReApply");
        }

        /// <summary>
        /// Try getting the receipt response until the specified timeout
        /// </summary>
        /// <param name="saleRegistryFacade"></param>
        /// <param name="transactionId"></param>
        /// <param name="waitTimeInMinutes"></param>
        /// <returns>ReceiptResponse</returns>
        public async Task<ReceiptResponse> TryReceiptResponse(string transactionId,
            int waitTimeInMinutes = 10, int sleepTime = 1000)
        {
            DateTime dateTime = DateTime.Now;
            DateTime dateTimeMax = dateTime.AddMinutes(waitTimeInMinutes);
            ReceiptResponse receiptInfo = null;

            // Wait until you get a Receipt Info
            while (dateTime < dateTimeMax)
            {
                receiptInfo = await GetReceiptInfo(transactionId);
                if (receiptInfo != null)
                {
                    if(receiptInfo.success)
                        break;
                    if (receiptInfo.error != null)
                        throw new ApplicationException(receiptInfo.error.ToString());
                }
                Thread.Sleep(sleepTime);
            }

            return receiptInfo;
        }

        /// <summary>
        /// Get the V1 Receipt Response
        /// </summary>
        /// <param name="txId">Transaction Id</param>
        /// <returns>HttpResponseMessage</returns>
        public async Task<ReceiptResponse> GetReceiptInfo(string txId)
        {
            using (var httpClient = new HttpClient())
            {
                using (var request = new HttpRequestMessage(new HttpMethod("GET"), $"{baseUrl}/api/SmartContracts/receipt?txHash={txId}"))
                {
                    request.Headers.TryAddWithoutValidation("accept", "application/json");
                    var response = await httpClient.SendAsync(request);

                    if (response.IsSuccessStatusCode)
                    {
                        string responseJson = await response.Content.ReadAsStringAsync();
                        var receiptResponse = JsonConvert.DeserializeObject<ReceiptResponse>(responseJson);
                        return receiptResponse;
                    }
                }
                return null;
            }
        }

        /// <summary>
        /// Get the Property State
        /// </summary>
        /// <param name="requestEntity">RequestEntity</param>
        /// <returns>HttpResponseMessage</returns>
        public async Task<HttpResponseMessage> GetPropertyState(SaleDeedRegistryRequest requestEntity)
        {
            using (var httpClient = new HttpClient())
            {
                using (var httpRequest = new HttpRequestMessage(new HttpMethod("POST"), $"{baseUrl}/api/contract/{contractAddress}/method/GetPropertyState"))
                {
                    SetRequestHeader(requestEntity, httpRequest);

                    httpRequest.Content = new StringContent("{  \"assetId\": \"{0}\"}".Replace("{0}", requestEntity.AssetId));
                    httpRequest.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                    var response = await httpClient.SendAsync(httpRequest);                   
                    return response;
                }
            }
        }

        /// <summary>
        /// Pay Application Transfer Fee
        /// </summary>
        /// <param name="requestObject">RequestObject</param>
        /// <returns>HttpResponseMessage</returns>
        public async Task<HttpResponseMessage> PayApplicationTransferFee(SaleDeedTransferFeeRequest transferFeeObject)
        {
            using (var httpClient = new HttpClient())
            {
                using (var httpRequest = new HttpRequestMessage(new HttpMethod("POST"), $"{baseUrl}/api/contract/{contractAddress}/method/PayApplicationTransferFee"))
                {
                    SetRequestHeader(transferFeeObject, httpRequest);

                    dynamic expando = new ExpandoObject();
                    expando.buyerAddress = transferFeeObject.BuyerAddress;
                    expando.payeeAddress = transferFeeObject.PayeeAddress;
                    expando.assetId = transferFeeObject.AssetId;
                    expando.fee = (ulong)transferFeeObject.Fees;

                    httpRequest.Content = new StringContent(JsonConvert.SerializeObject(expando));
                    httpRequest.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                    var response = await httpClient.SendAsync(httpRequest);
                    return response;
                }
            }
        }

        /// <summary>
        /// Transfer Ownership
        /// </summary>
        /// <param name="requestObject">RequestObject</param>
        /// <returns>HttpResponseMessage</returns>
        public async Task<HttpResponseMessage> TransferOwnership(SaleDeedTransferOwnershipRequest ownershipEntity)
        {
            using (var httpClient = new HttpClient())
            {
                using (var httpRequest = new HttpRequestMessage(new HttpMethod("POST"), $"{baseUrl}/api/contract/{contractAddress}/method/TransferOwnership"))
                {
                    SetRequestHeader(ownershipEntity, httpRequest);

                    dynamic expando = new ExpandoObject();
                    expando.propertyOwner = ownershipEntity.OwnerAddress;
                    expando.propertyBuyer = ownershipEntity.BuyerAddress;
                    expando.assetId = ownershipEntity.AssetId;

                    httpRequest.Content = new StringContent(JsonConvert.SerializeObject(expando));
                    httpRequest.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                    var response = await httpClient.SendAsync(httpRequest);
                    return response;
                }
            }
        }

        private static void SetRequestHeader(SaleDeedRegistryBaseRequest requestObj, HttpRequestMessage httpRequest)
        {
            httpRequest.Headers.Add("accept", "application/json");
            httpRequest.Headers.Add("GasPrice", requestObj.GasPrice.ToString());
            httpRequest.Headers.Add("GasLimit", requestObj.GasLimit.ToString());
            httpRequest.Headers.Add("Amount", requestObj.Amount.ToString());
            httpRequest.Headers.Add("FeeAmount", requestObj.FeeAmount.ToString());
            httpRequest.Headers.Add("WalletName", requestObj.WalletName);
            httpRequest.Headers.Add("WalletPassword", requestObj.WalletPassword);
            httpRequest.Headers.Add("Sender", requestObj.Sender);
        }

        /// <summary>
        /// Execute the Action based on the specified one in the parameter
        /// </summary>
        /// <param name="requestEntity">RequestObject</param>
        /// <param name="action">Action</param>
        /// <returns>HttpResponseMessage</returns>
        private async Task<HttpResponseMessage> Execute(SaleDeedRegistryRequest requestEntity,
            string action)
        {
            using (var httpClient = new HttpClient())
            {
                using (var httpRequest = new HttpRequestMessage(new HttpMethod("POST"), $"{baseUrl}/api/contract/{contractAddress}/method/{action}"))
                {
                    SetRequestHeader(requestEntity, httpRequest);
                  
                    dynamic expando = new ExpandoObject();
                    expando.propertyOwnerAddress = requestEntity.OwnerAddress;
                    expando.buyerAddress = requestEntity.BuyerAddress;
                    expando.assetId = requestEntity.AssetId;

                    httpRequest.Content = new StringContent(JsonConvert.SerializeObject(expando));
                    httpRequest.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                    var response = await httpClient.SendAsync(httpRequest);
                    return response;
                }
            }
            //
        }
    }
}
