using System;
using System.Net.Http;
using System.Net.Http.Headers;
using Microsoft.Extensions.Options;

namespace Signature.Utility
{
    public class BaseClient
    {
        private readonly IOptions<AppSettings> _appSettings;

        public BaseClient(IOptions<AppSettings> appSettings)
        {
            _appSettings = appSettings;
        }

        public HttpClient ContractCall(string senderWalletAddress)
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri($"{_appSettings.Value.ApiBaseUrl}/contract/{_appSettings.Value.ContractAddress}/method/");

            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));

            client.DefaultRequestHeaders.Add("GasPrice", _appSettings.Value.GasPrice);
            client.DefaultRequestHeaders.Add("GasLimit", _appSettings.Value.GasLimit);
            client.DefaultRequestHeaders.Add("Amount", _appSettings.Value.Amount);
            client.DefaultRequestHeaders.Add("FeeAmount", _appSettings.Value.FeeAmount);
            client.DefaultRequestHeaders.Add("WalletName", _appSettings.Value.WalletName);
            client.DefaultRequestHeaders.Add("WalletPassword", "stratis");
            client.DefaultRequestHeaders.Add("Sender", senderWalletAddress);

            return client;
        }

        public HttpClient LocalCall()
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri($"{_appSettings.Value.ApiBaseUrl}/SmartContracts/local-call");

            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));

            return client;
        }

        public HttpClient ReceiptCall()
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri($"{_appSettings.Value.ApiBaseUrl}/SmartContracts/receipt/");

            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));

            return client;
        }

        public HttpClient WalletCall()
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri($"{_appSettings.Value.ApiBaseUrl}/Wallet/");

            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));

            return client;
        }
    }
}
