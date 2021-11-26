using Stratis.Bitcoin.Features.Wallet.Models;
using System.Collections.Generic;

namespace Ticketbooth.Api.Requests
{
    public class SmartContractTransactionRequest
    {
        /// <summary>
        /// The name of the wallet containing funds to use to cover transaction fees, gas, and any other costs.
        /// </summary>
        public string WalletName { get; set; }

        /// <summary>
        /// The name of the wallet account containing funds to use to cover transaction fees, gas, and any other costs.
        /// </summary>
        public string AccountName { get; set; }

        /// <summary>
        /// A list of outpoints if required.
        /// </summary>
        public List<OutpointRequest> Outpoints { get; set; }

        /// <summary>
        /// The password for the wallet.
        /// </summary>=
        public string Password { get; set; }

        /// <summary>
        /// The gas price to charge when the smart contract constructor is run by the miner mining the creation transaction.
        /// </summary>
        public ulong GasPrice { get; set; }

        /// <summary>
        /// A wallet address containing the funds to cover transaction fees, gas, and any other costs.
        /// It is recommended that you use /api/SmartContractWallet/account-addresses to retrieve
        /// an address to use for smart contracts. This enables you to obtain a smart contract transaction history.
        /// However, any sender address containing the required funds will work.
        /// </summary>
        public string Sender { get; set; }
    }
}
