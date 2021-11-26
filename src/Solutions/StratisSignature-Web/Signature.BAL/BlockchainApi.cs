using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Signature.BAL.Interface;
using Signature.Shared.Models;
using Signature.Utility;

namespace Signature.BAL
{

    public class BlockchainApi : IBlockchainApi
    {
        private readonly IOptions<AppSettings> _appSettings;

        public BlockchainApi(IOptions<AppSettings> appSettings)
        {
            _appSettings = appSettings;
        }

        public async Task<TransactionResponse> CreateDocument(string agreementId, string fileHash, string senderWalletAddress)
        {
            try
            {
                var requestBody = new
                {
                    agreementId = agreementId,
                    fileHash = fileHash
                };

                using (var client = new BaseClient(_appSettings).ContractCall(senderWalletAddress))
                {
                    var response = await client.PostAsync("CreateAgreement", CreateJsonContent(requestBody));
                    if (response.IsSuccessStatusCode)
                    {
                        var content = await response.Content.ReadAsStringAsync();
                        var result = JsonConvert.DeserializeObject<TransactionResponse>(content);
                        return result;
                    }

                    var errorMessage = response.RequestMessage.ToString();
                    return new TransactionResponse { Success = false, Message = errorMessage };
                }
            }
            catch (Exception ex)
            {
                return new TransactionResponse { Success = false, Message = ex.Message };
            }
        }

        public async Task<ApiLocalCallResponse<Agreement>> GetDocument(string agreementId, string senderWalletAddress)
        {
            try
            {
                using (var client = new BaseClient(_appSettings).LocalCall())
                {
                    var requestBody = new
                    {
                        contractAddress = _appSettings.Value.ContractAddress,
                        methodName = "GetAgreement",
                        amount = _appSettings.Value.Amount,
                        gasPrice = _appSettings.Value.GasPrice,
                        gasLimit = _appSettings.Value.GasLimit,
                        sender = senderWalletAddress,
                        parameters = new List<string> { $"4#{agreementId}" }
                    };

                    HttpResponseMessage response = await client.PostAsync(client.BaseAddress, CreateJsonContent(requestBody));

                    if (response.IsSuccessStatusCode)
                    {
                        var content = await response.Content.ReadAsStringAsync();
                        var result = JsonConvert.DeserializeObject<ApiLocalCallResponse<Agreement>>(content);
                        return result;
                    }
                    return new ApiLocalCallResponse<Agreement> { ErrorMessage = response.RequestMessage.ToString() };
                }
            }
            catch (Exception ex)
            {
                return new ApiLocalCallResponse<Agreement> { ErrorMessage = ex.Message };
            }
        }

        public async Task<TransactionResponse> AddSigner(string agreementId, string senderWalletAddress, string signerWalletAddress)
        {
            try
            {
                var requestBody = new
                {
                    agreementId = agreementId,
                    signer = signerWalletAddress
                };

                using (var client = new BaseClient(_appSettings).ContractCall(senderWalletAddress))
                {
                    var response = await client.PostAsync("AddSigners", CreateJsonContent(requestBody));
                    if (response.IsSuccessStatusCode)
                    {
                        var content = await response.Content.ReadAsStringAsync();
                        var result = JsonConvert.DeserializeObject<TransactionResponse>(content);
                        return result;
                    }

                    return new TransactionResponse { Success = false, Message = response.RequestMessage.ToString() };
                }
            }
            catch (Exception ex)
            {
                return new TransactionResponse { Success = false, Message = ex.Message };
            }
        }

        public async Task<string> SignMessage(string message, string signerWalletAddress, string signerWalletPassword)
        {

            var requestBody = new
            {
                walletName = _appSettings.Value.WalletName,
                password = signerWalletPassword,
                externalAddress = signerWalletAddress,
                message = message
            };

            using (var client = new BaseClient(_appSettings).WalletCall())
            {
                var response = await client.PostAsync("signmessage", CreateJsonContent(requestBody));
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var result = JsonConvert.DeserializeObject<string>(content);
                    return result;
                }

            }
            return null;
        }

        public async Task<TransactionResponse> SignDocument(string agreementId, string signerWalletAddress, string digitalSign)
        {
            try
            {
                var requestBody = new
                {
                    agreementId = agreementId,
                    digitalSign = digitalSign
                };

                using (var client = new BaseClient(_appSettings).ContractCall(signerWalletAddress))
                {
                    var response = await client.PostAsync("SignAgreement", CreateJsonContent(requestBody));
                    if (response.IsSuccessStatusCode)
                    {
                        var content = await response.Content.ReadAsStringAsync();
                        var result = JsonConvert.DeserializeObject<TransactionResponse>(content);
                        return result;
                    }

                    return new TransactionResponse { Success = false, Message = response.RequestMessage.ToString() };

                }
            }
            catch (Exception ex)
            {
                return new TransactionResponse { Success = false, Message = ex.Message };
            }
        }

        public async Task<string> Verify(string signature, string message, string signerWalletAddress)
        {
            var requestBody = new
            {
                signature = signature,
                externalAddress = signerWalletAddress,
                message = message
            };

            using (var client = new BaseClient(_appSettings).WalletCall())
            {
                var response = await client.PostAsync("verifymessage", CreateJsonContent(requestBody));
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var result = JsonConvert.DeserializeObject<string>(content);
                    return result;
                }

            }
            return null;
        }

        public async Task<ApiLocalCallResponse<Stamp>> GetStamp(string agreementId, string signerWalletAddress)
        {
            try
            {
                using (var client = new BaseClient(_appSettings).LocalCall())
                {
                    var requestBody = new
                    {
                        contractAddress = _appSettings.Value.ContractAddress,
                        methodName = "GetStamp",
                        amount = _appSettings.Value.Amount,
                        gasPrice = _appSettings.Value.GasPrice,
                        gasLimit = _appSettings.Value.GasLimit,
                        sender = signerWalletAddress,
                        parameters = new List<string> { $"4#{agreementId}", $"9#{signerWalletAddress}" }
                    };

                    HttpResponseMessage response = await client.PostAsync(client.BaseAddress, CreateJsonContent(requestBody));

                    if (response.IsSuccessStatusCode)
                    {
                        var content = await response.Content.ReadAsStringAsync();
                        var result = JsonConvert.DeserializeObject<ApiLocalCallResponse<Stamp>>(content);
                        return result;
                    }
                    return new ApiLocalCallResponse<Stamp> { ErrorMessage = response.ReasonPhrase };
                }
            }
            catch (Exception ex)
            {
                return new ApiLocalCallResponse<Stamp> { ErrorMessage = ex.Message };
            }
        }

        public async Task<TransactionReceipt> GetReceipt(string txHash)
        {
            try
            {
                using (var client = new BaseClient(_appSettings).ReceiptCall())
                {
                    var response = await client.GetAsync($"{client.BaseAddress}?txHash={txHash}");

                    if (response.IsSuccessStatusCode)
                    {
                        var content = await response.Content.ReadAsStringAsync();
                        var result = JsonConvert.DeserializeObject<TransactionReceipt>(content);
                        return result;
                    }
                    return new TransactionReceipt { Success = false, Error = response.RequestMessage.ToString() };
                }
            }
            catch (Exception ex)
            {
                return new TransactionReceipt { Success = false, Error = ex.Message };
            }
        }

        private static StringContent CreateJsonContent(object obj)
        {
            var json = JsonConvert.SerializeObject(obj);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            return content;
        }
    }
}
