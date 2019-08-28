using Moq;
using Stratis.SmartContracts;
using Stratis.SmartContracts.CLR;
using System;
using System.Reflection;
using Xunit;

namespace PingPongContract.Tests
{
    public class StarterTests
    {
        private const string Player = "0x0000000000000000000000000000000000000001";
        private const string Opponent = "0x0000000000000000000000000000000000000002";
        private const string PlayerContract = "0x0000000000000000000000000000000000000003";
        private const string OpponentContract = "0x0000000000000000000000000000000000000004";

        private static readonly Address PlayerAddress = Player.HexToAddress();
        private static readonly Address OpponentAddress = Opponent.HexToAddress();
        private static readonly Address PlayerContractAddress = PlayerContract.HexToAddress();
        private static readonly Address OpponentContractAddress = OpponentContract.HexToAddress();

        private readonly Mock<ISmartContractState> mockContractState;
        private readonly Mock<IPersistentState> mockPersistentState;
        private readonly Mock<IInternalTransactionExecutor> mockInternalExecutor;
        private readonly Mock<IContractLogger> mockContractLogger;

        public StarterTests()
        {
            this.mockContractLogger = new Mock<IContractLogger>();
            this.mockPersistentState = new Mock<IPersistentState>();
            this.mockInternalExecutor = new Mock<IInternalTransactionExecutor>();
            this.mockContractState = new Mock<ISmartContractState>();
            this.mockContractState.Setup(s => s.PersistentState).Returns(this.mockPersistentState.Object);
            this.mockContractState.Setup(s => s.ContractLogger).Returns(this.mockContractLogger.Object);
            this.mockContractState.Setup(s => s.InternalTransactionExecutor).Returns(this.mockInternalExecutor.Object);
        }

        [Fact]
        public void StartGame_Success()
        {
            // Set up create for player vs opponent.
            this.mockInternalExecutor.Setup(s =>
                s.Create<Player>(
                    It.IsAny<ISmartContractState>(),
                    It.IsAny<ulong>(),
                    It.Is<object[]>(o =>
                        (Address)o[0] == PlayerAddress &&
                        (Address)o[1] == OpponentAddress),
                    It.IsAny<ulong>()))
                .Returns(CreateResult.Succeeded(PlayerContractAddress));

            // Set up create for opponent vs player.
            this.mockInternalExecutor.Setup(s =>
                s.Create<Player>(
                    It.IsAny<ISmartContractState>(),
                    It.IsAny<ulong>(),
                    It.Is<object[]>(o =>
                        (Address)o[0] == OpponentAddress &&
                        (Address)o[1] == PlayerAddress),
                    It.IsAny<ulong>()))
                .Returns(CreateResult.Succeeded(OpponentContractAddress));

            var starter = new Starter(this.mockContractState.Object);

            starter.StartGame(PlayerAddress, OpponentAddress, "Test");

            this.mockInternalExecutor.Verify(
                s => s.Create<Player>(
                    this.mockContractState.Object,
                    0,
                    It.Is<object[]>(o =>
                        (Address)o[0] == PlayerAddress &&
                        (Address)o[1] == OpponentAddress &&
                        (string)o[2] == "Test"),
                    0), Times.Once);

            this.mockInternalExecutor.Verify(
                s => s.Create<Player>(
                    this.mockContractState.Object,
                    0,
                    It.Is<object[]>(o =>
                        (Address)o[0] == OpponentAddress &&
                        (Address)o[1] == PlayerAddress &&
                        (string)o[2] == "Test"),
                    0), Times.Once);

            this.mockContractLogger.Verify(
                l => l.Log(
                    It.IsAny<ISmartContractState>(),
                    new Starter.GameCreated
                    {
                        Player1Contract = PlayerContractAddress,
                        Player2Contract = OpponentContractAddress,
                        GameName = "Test"
                    }),
                Times.Once);
        }
    }
}
