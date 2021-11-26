using Microsoft.Extensions.Logging;
using Moq;
using NBitcoin;
using NUnit.Framework;
using SmartContract.Essentials.Ciphering;
using SmartContract.Essentials.Randomness;
using Stratis.Bitcoin.Connection;
using Stratis.Bitcoin.Consensus;
using Stratis.Bitcoin.Features.MemoryPool;
using Stratis.Bitcoin.Features.SmartContracts.Models;
using Stratis.Bitcoin.Features.SmartContracts.Wallet;
using Stratis.Bitcoin.Features.Wallet.Broadcasting;
using Stratis.Bitcoin.Features.Wallet.Interfaces;
using Stratis.Bitcoin.P2P.Peer;
using Stratis.Sidechains.Networks;
using Stratis.SmartContracts;
using Stratis.SmartContracts.CLR;
using Stratis.SmartContracts.Core.State;
using Ticketbooth.Api.Controllers;
using Ticketbooth.Api.Tests.Fakes;
using static TicketContract;
using BroadcastState = Stratis.Bitcoin.Features.Wallet.Broadcasting.State;

namespace Ticketbooth.Api.Tests.Controllers
{
    public partial class PublicControllerTests
    {
        private Mock<IBroadcasterManager> _broadcasterManager;
        private Mock<ICipherFactory> _cipherFactory;
        private Mock<IConnectionManager> _connectionManager;
        private Mock<IConsensusManager> _consensusManager;
        private Mock<ILogger<PublicController>> _logger;
        private Mock<ISerializer> _serializer;
        private Mock<ISmartContractTransactionService> _smartContractTransactionService;
        private Mock<IStateRepositoryRoot> _stateRepositoryRoot;
        private Mock<IStringGenerator> _stringGenerator;

        private Network _network;
        private TransactionHelpers _transactionHelpers;

        private PublicController _publicController;

        public string ValidAddress { get; private set; }

        public string InvalidAddress { get; private set; }

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _network = CirrusNetwork.NetworksSelector.Testnet();
            _transactionHelpers = new TransactionHelpers(_network);
            ValidAddress = "tNuiVJiEhvbQgXu4P32S4TnAVEG3kgLnu8";
            InvalidAddress = "CWNYqR11FFsbt14FVtmV1X3rPntoxGtYrR";
        }

        [SetUp]
        public void SetUp()
        {
            _broadcasterManager = new Mock<IBroadcasterManager>();
            _cipherFactory = new Mock<ICipherFactory>();
            _cipherFactory.Setup(callTo => callTo.CreateCbcProvider()).Returns(new AesCbc());
            _connectionManager = new Mock<IConnectionManager>();
            _consensusManager = new Mock<IConsensusManager>();
            _logger = new Mock<ILogger<PublicController>>();
            _serializer = new Mock<ISerializer>();
            _smartContractTransactionService = new Mock<ISmartContractTransactionService>();
            _stateRepositoryRoot = new Mock<IStateRepositoryRoot>();
            _stringGenerator = new Mock<IStringGenerator>();

            _publicController = new PublicController(_broadcasterManager.Object, _cipherFactory.Object, _connectionManager.Object,
                 _consensusManager.Object, _logger.Object, _serializer.Object, _smartContractTransactionService.Object, _stateRepositoryRoot.Object,
                 _stringGenerator.Object, _network);
        }

        private void SetupContractExists(string address)
        {
            _stateRepositoryRoot.Setup(callTo => callTo.IsExist(address.ToUint160(_network))).Returns(true);
        }

        private void SetupContractDoesNotExist(string address)
        {
            _stateRepositoryRoot.Setup(callTo => callTo.IsExist(address.ToUint160(_network))).Returns(false);
        }

        private void SetupZeroConnections()
        {
            _connectionManager.Setup(callTo => callTo.ConnectedPeers).Returns(new NetworkPeerCollection());
        }

        private void SetupOneConnection()
        {
            _connectionManager.Setup(callTo => callTo.ConnectedPeers).Returns(new FakeNetworkPeerCollection { Mock.Of<INetworkPeer>() });
        }

        private BuildCreateContractTransactionResponse SetupFailedBuildCreateContractTransactionRequest()
        {
            var buildCreateTransactionResponse = BuildCreateContractTransactionResponse.Failed("Testing unsuccessful transaction");
            _smartContractTransactionService
                .Setup(callTo => callTo.BuildCreateTx(It.IsAny<BuildCreateContractTransactionRequest>()))
                .Returns(buildCreateTransactionResponse);
            return buildCreateTransactionResponse;
        }

        private BuildCreateContractTransactionResponse SetupSuccessfulBuildCreateContractTransactionRequest()
        {
            var buildCreateTransactionResponse = _transactionHelpers.CreateSuccessfulBuildCreateContractTransactionResponse();
            _smartContractTransactionService
                .Setup(callTo => callTo.BuildCreateTx(It.IsAny<BuildCreateContractTransactionRequest>()))
                .Returns(buildCreateTransactionResponse);
            return buildCreateTransactionResponse;
        }

        private BuildCallContractTransactionResponse SetupFailedBuildCallContractTransactionRequest()
        {
            var buildCallTransactionResponse = BuildCallContractTransactionResponse.Failed("Testing unsuccessful transaction");
            _smartContractTransactionService
                .Setup(callTo => callTo.BuildCallTx(It.IsAny<BuildCallContractTransactionRequest>()))
                .Returns(buildCallTransactionResponse);
            return buildCallTransactionResponse;
        }

        private BuildCallContractTransactionResponse SetupSuccessfulBuildCallContractTransactionRequest()
        {
            var buildCallTransactionResponse = _transactionHelpers.CreateSuccessfulBuildCallContractTransactionResponse();
            _smartContractTransactionService
                .Setup(callTo => callTo.BuildCallTx(It.IsAny<BuildCallContractTransactionRequest>()))
                .Returns(buildCallTransactionResponse);
            return buildCallTransactionResponse;
        }

        private void SetupCannotBroadcastTransaction()
        {
            var cantBroadcastTransactionEntry = new TransactionBroadcastEntry(_transactionHelpers.CreateEmpty(), BroadcastState.CantBroadcast, new MempoolError());
            _broadcasterManager.Setup(callTo => callTo.GetTransaction(It.IsAny<uint256>())).Returns(cantBroadcastTransactionEntry);
        }

        private void SetupTransactionWasBroadcast()
        {
            var cantBroadcastTransactionEntry = new TransactionBroadcastEntry(_transactionHelpers.CreateEmpty(), BroadcastState.Broadcasted, new MempoolError());
            _broadcasterManager.Setup(callTo => callTo.GetTransaction(It.IsAny<uint256>())).Returns(cantBroadcastTransactionEntry);
        }

        private void SetIdentityVerificationPolicyTo(bool value)
        {
            var serializedIdentityVerificationPolicy = new byte[] { 1, 2, 1 };
            var serializedIdentityVerificationPolicyValue = new byte[] { 2, 1, 2 };
            _serializer.Setup(callTo => callTo.Serialize("RequireIdentityVerification")).Returns(serializedIdentityVerificationPolicy);
            _stateRepositoryRoot.Setup(callTo => callTo.GetStorageValue(It.IsAny<uint160>(), serializedIdentityVerificationPolicy)).Returns(serializedIdentityVerificationPolicyValue);
            _serializer.Setup(callTo => callTo.ToBool(serializedIdentityVerificationPolicyValue)).Returns(value);
        }

        private void SetNoReleaseBlocksTo(ulong count)
        {
            var serializedNoReleaseBlocks = new byte[] { 1, 1, 2 };
            var serializedNoReleaseBlocksValue = new byte[] { 2, 2, 1 };
            _serializer.Setup(callTo => callTo.Serialize("NoRefundBlockCount")).Returns(serializedNoReleaseBlocks);
            _stateRepositoryRoot.Setup(callTo => callTo.GetStorageValue(It.IsAny<uint160>(), serializedNoReleaseBlocks)).Returns(serializedNoReleaseBlocksValue);
            _serializer.Setup(callTo => callTo.ToUInt64(serializedNoReleaseBlocksValue)).Returns(count);
        }

        private void SetTicketReleaseFeeTo(ulong value)
        {
            var serializedTicketReleaseFee = new byte[] { 2, 1, 1 };
            var serializedTicketReleaseFeeValue = new byte[] { 1, 2, 2 };
            _serializer.Setup(callTo => callTo.Serialize("ReleaseFee")).Returns(serializedTicketReleaseFee);
            _stateRepositoryRoot.Setup(callTo => callTo.GetStorageValue(It.IsAny<uint160>(), serializedTicketReleaseFee)).Returns(serializedTicketReleaseFeeValue);
            _serializer.Setup(callTo => callTo.ToUInt64(serializedTicketReleaseFeeValue)).Returns(value);
        }

        private void SetTicketsTo(Ticket[] tickets)
        {
            var serializedTickets = new byte[] { 2, 1, 2 };
            var serializedTicketsValue = new byte[] { 1, 2, 1 };
            _serializer.Setup(callTo => callTo.Serialize("Tickets")).Returns(serializedTickets);
            _stateRepositoryRoot.Setup(callTo => callTo.GetStorageValue(It.IsAny<uint160>(), serializedTickets)).Returns(serializedTicketsValue);
            _serializer.Setup(callTo => callTo.ToArray<Ticket>(serializedTicketsValue)).Returns(tickets);
        }

        private void SetConsensusHeight(ulong height)
        {
            _consensusManager.Setup(callTo => callTo.Tip).Returns(new ChainedHeader(new BlockHeader(), new uint256(), (int)height));
        }

        private void SetEndOfSaleToBlock(ulong value)
        {
            var serializedEndOfSale = new byte[] { 1, 1, 1 };
            var serializedEndOfSaleValue = new byte[] { 2, 2, 2 };
            _serializer.Setup(callTo => callTo.Serialize("EndOfSale")).Returns(serializedEndOfSale);
            _stateRepositoryRoot.Setup(callTo => callTo.GetStorageValue(It.IsAny<uint160>(), serializedEndOfSale)).Returns(serializedEndOfSaleValue);
            _serializer.Setup(callTo => callTo.ToUInt64(serializedEndOfSaleValue)).Returns(value);
        }
    }
}
