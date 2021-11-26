using NBitcoin;
using Stratis.Bitcoin.Features.SmartContracts.Models;

namespace Ticketbooth.Api.Tests
{
    internal class TransactionHelpers
    {
        private readonly Network _network;

        public TransactionHelpers(Network network)
        {
            _network = network;
        }

        public Transaction CreateEmpty()
        {
            return new TransactionBuilder(_network).BuildTransaction(true);
        }

        public BuildCreateContractTransactionResponse CreateSuccessfulBuildCreateContractTransactionResponse()
        {
            var transaction = CreateEmpty();
            return BuildCreateContractTransactionResponse.Succeeded(transaction, new Money(500_000_000), "tNuiVJiEhvbQgXu4P32S4TnAVEG3kgLnu8");
        }

        public BuildCallContractTransactionResponse CreateSuccessfulBuildCallContractTransactionResponse()
        {
            var transaction = CreateEmpty();
            return BuildCallContractTransactionResponse.Succeeded("methodNameDoesNotMatter", transaction, new Money(500_000_000));
        }
    }
}
