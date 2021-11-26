using Newtonsoft.Json;
using RestSharp;
using SaleDeedRegistry.Wallet.Entities;
using System;
using System.Dynamic;

namespace SaleDeedRegistry.Wallet.Wallet
{
    public interface IWalletServiceProxy
    {
        string CreateMnemonic(string languge, int wordcount = 24);
        bool CreateWallet(string mnemonic, string password,
           string passphrase, string name);
        bool LoadWallet(string walletName, string walletPassword);
        bool RecoverWallet(string walletName, string walletPassword,
            string mnemonic, DateTime creationDate);
        WalletHistoryRoot GetWalletHistory(string walletName);
        WalletBalanceRoot GetWalletBalance(string walletName);
        WalletAddressesRoot GetAllAddresses(string walletName, string accountName);
        WalletInfo GetWalletInfo(string walletName);
    }

    /// <summary>
    /// The Wallet Service Proxy, deals with the Stratis Wallet Management
    /// </summary>
    public class WalletServiceProxy : IWalletServiceProxy
    {
        private readonly string baseUrl = "http://localhost:37223";
        public WalletServiceProxy(string baseUrl = "http://localhost:37223")
        {
            this.baseUrl = baseUrl;
        }

        /// <summary>
        /// Create Mnemonic by language and wordcount
        /// </summary>
        /// <param name="languge">Language</param>
        /// <param name="wordcount">Wordcount</param>
        /// <returns>Mnemonic</returns>
        public string CreateMnemonic(string languge, int wordcount = 24)
        {
            var client = new RestClient($"{baseUrl}/api/wallet/mnemonic?language={languge}&wordcount={wordcount}");
            var request = new RestRequest(Method.GET);
            request.AddHeader("Connection", "keep-alive");
            request.AddHeader("Accept-Encoding", "gzip, deflate");
            request.AddHeader("Accept", "*/*");
            IRestResponse response = client.Execute(request);
            if (response.IsSuccessful)
                return response.Content;
            return string.Empty;
        }

        /// <summary>
        /// Create a wallet 
        /// </summary>
        /// <param name="mnemonic">Mnemonic</param>
        /// <param name="password">Wallet Password</param>
        /// <param name="passphrase">Wallet PassPhrase</param>
        /// <param name="name">Wallet Name</param>
        /// <returns>True/False</returns>
        public bool CreateWallet(string mnemonic, string password, 
            string passphrase, string name)
        {
            var client = new RestClient($"{baseUrl}/api/wallet/create");
            var request = new RestRequest(Method.POST);
            request.AddHeader("Content-Type", "application/json");

            dynamic expando = new ExpandoObject();
            expando.password = password;
            expando.name = name;
            expando.mnemonic = mnemonic;
            expando.passphrase = passphrase;

            request.AddJsonBody(expando);
            IRestResponse response = client.Execute(request);
            if (response.IsSuccessful)
                return true;
            return false;
        }

        /// <summary>
        /// Load the wallet using the specified Wallet Name & Password
        /// </summary>
        /// <param name="walletName">Wallet Name</param>
        /// <param name="walletPassword">Wallet Password</param>
        /// <returns>True/False</returns>
        public bool LoadWallet(string walletName, string walletPassword)
        {
            var client = new RestClient($"{baseUrl}/api/wallet/load");
            var request = new RestRequest(Method.POST);
            request.AddHeader("Content-Type", "application/json");

            dynamic expando = new ExpandoObject();
            expando.password = walletPassword;
            expando.name = walletName;

            request.AddJsonBody(expando);
            IRestResponse response = client.Execute(request);
            if (response.IsSuccessful)
                return true;
            return false;
        }

        /// <summary>
        /// Recover the Wallet based on the specified 
        ///     Wallet Name
        ///     Wallet Password
        ///     Mnemonic
        ///     Creation Date
        /// </summary>
        /// <param name="walletName">Wallet Name</param>
        /// <param name="walletPassword">Wallet Password</param>
        /// <param name="mnemonic">Mnemonic</param>
        /// <param name="creationDate">Creation Date</param>
        /// <returns>True/False</returns>
        public bool RecoverWallet(string walletName, string walletPassword,
            string mnemonic, DateTime creationDate)
        {
            var client = new RestClient($"{baseUrl}/api/wallet/recover");
            var request = new RestRequest(Method.POST);
            request.AddHeader("Content-Type", "application/json");

            dynamic expando = new ExpandoObject();
            expando.password = walletPassword;
            expando.name = walletName;
            expando.mnemonic = mnemonic;
            expando.creationDate = creationDate.ToShortDateString();

            request.AddJsonBody(expando);
            IRestResponse response = client.Execute(request);
            if (response.IsSuccessful)
                return true;
            return false;
        }

        /// <summary>
        /// Get the Wallet Info based on the specified Wallet Name
        /// </summary>
        /// <param name="walletName">Wallet Name</param>
        /// <returns>WalletInfo</returns>
        public WalletInfo GetWalletInfo(string walletName)
        {
            var client = new RestClient($"{baseUrl}/api/wallet/general-info?name={walletName}");
            var request = new RestRequest(Method.GET);

            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("Cache-Control", "no-cache");
            IRestResponse response = client.Execute(request);
            
            if (response.IsSuccessful)
            {
                return JsonConvert.DeserializeObject<WalletInfo>(response.Content);
            }
            return null;
        }

        /// <summary>
        /// Get the Wallet History Information based on the specified WalletName
        /// </summary>
        /// <param name="walletName">WalletName</param>
        /// <returns>WalletHistoryRoot</returns>
        public WalletHistoryRoot GetWalletHistory(string walletName)
        {
            var client = new RestClient($"{baseUrl}/api/wallet/history?walletname={walletName}");
            var request = new RestRequest(Method.GET);
            request.AddHeader("Connection", "keep-alive");
            request.AddHeader("Accept-Encoding", "gzip, deflate");
            request.AddHeader("Cache-Control", "no-cache");
            request.AddHeader("Accept", "*/*");
            request.AddHeader("Content-Type", "application/json");
            IRestResponse response = client.Execute(request);
            if (response.IsSuccessful)
            {
                return JsonConvert.DeserializeObject<WalletHistoryRoot>(response.Content);
            }
            return null;
        }

        /// <summary>
        /// Get Wallet Balances by the specified Wallet Name
        /// </summary>
        /// <param name="walletName">Wallet Name</param>
        /// <returns>WalletBalanceRoot</returns>
        public WalletBalanceRoot GetWalletBalance(string walletName)
        {
            var client = new RestClient($"{baseUrl}/api/wallet/balance?walletname={walletName}");
            var request = new RestRequest(Method.GET);
            request.AddHeader("Connection", "keep-alive");
            request.AddHeader("Accept-Encoding", "gzip, deflate");
            request.AddHeader("Cache-Control", "no-cache");
            request.AddHeader("Accept", "*/*");
            request.AddHeader("Content-Type", "application/json");
            IRestResponse response = client.Execute(request);
            if (response.IsSuccessful)
            {
                return JsonConvert.DeserializeObject<WalletBalanceRoot>(response.Content);
            }
            return null;
        }

        /// <summary>
        /// Get all Addresses for the specified Wallet Name and Account Name
        /// </summary>
        /// <param name="walletName">Wallet Name</param>
        /// <param name="accountName">Account Name</param>
        /// <returns>WalletAddressesRoot</returns>
        public WalletAddressesRoot GetAllAddresses(string walletName, string accountName)
        {
            var client = new RestClient($"{baseUrl}/api/wallet/addresses?walletname={walletName}&accountname={accountName}");
            var request = new RestRequest(Method.GET);
            request.AddHeader("Connection", "keep-alive");
            request.AddHeader("Cache-Control", "no-cache");
            request.AddHeader("Accept", "*/*");
            request.AddHeader("Content-Type", "application/json");
            IRestResponse response = client.Execute(request);
            if (response.IsSuccessful)
            {
                return JsonConvert.DeserializeObject<WalletAddressesRoot>(response.Content);
            }
            return null;
        }
        //
    }
}
