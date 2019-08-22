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
        private const string GameStarter = "0x0000000000000000000000000000000000000001";
        private const string GamePlayer = "0x0000000000000000000000000000000000000002";

        private static readonly Address GameStarterAddress = GameStarter.HexToAddress();
        private static readonly Address GamePlayerAddress = GamePlayer.HexToAddress();

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

        [Theory]
        [InlineData(nameof(Starter.State))]
        [InlineData(nameof(Starter.PingPongGameName))]
        [InlineData(nameof(Starter.GameStarter))]
        [InlineData(nameof(Starter.GamePlayer))]
        [InlineData(nameof(Starter.PingPongTimes))]
        public void Property_Setter_Is_Private(string propertyName)
        {
            Type type = typeof(Starter);

            PropertyInfo property = type.GetProperty(propertyName);

            Assert.True(property.SetMethod.IsPrivate);
        }

        [Fact]
        public void Constructor_Sets_Initial_Values()
        {
            this.mockContractState.Setup(s => s.Message.Sender).Returns(GameStarterAddress);

            // Set up Create to return a new player address.
            this.mockInternalExecutor.Setup(s =>
                s.Create<Player>(
                    It.IsAny<ISmartContractState>(),
                    It.IsAny<ulong>(),
                    It.IsAny<object[]>(),
                    It.IsAny<ulong>()))
                .Returns(CreateResult.Succeeded(GamePlayerAddress));

            var starter = new Starter(this.mockContractState.Object, "Test");

            this.mockInternalExecutor.Verify(s =>
                s.Create<Player>(
                    this.mockContractState.Object,
                    0,
                    It.Is<object[]>(o => (string)o[0] == "Test"),
                    0));

            this.mockPersistentState.Verify(s => s.SetAddress(nameof(Starter.GameStarter), GameStarterAddress), Times.Once);
            this.mockPersistentState.Verify(s => s.SetAddress(nameof(Starter.GamePlayer), GamePlayerAddress), Times.Once);
            this.mockPersistentState.Verify(s => s.SetString(nameof(Starter.PingPongGameName), "Test"), Times.Once);
            this.mockPersistentState.Verify(s => s.SetUInt32(nameof(Starter.State), (uint)Starter.StateType.GameProvisioned), Times.Once);
        }

        [Fact]
        public void StartPingPong_Sender_Not_GameStarter_Fails()
        {
            var starter = this.NewStarter();

            this.mockContractState.Setup(s => s.Message.Sender).Returns(Address.Zero);

            Assert.Throws<SmartContractAssertException>(() => starter.StartPingPong(int.MaxValue));
        }

        [Theory]
        [InlineData(int.MinValue)]
        [InlineData(0)]
        [InlineData(int.MaxValue)]
        public void StartPingPong_Success(int pingPongTimes)
        {
            var starter = this.NewStarter();

            // Set the calling address to be the game starter.
            this.mockContractState.Setup(s => s.Message.Sender).Returns(GameStarterAddress);

            // Only the starter contract can call this.
            this.mockContractState.Setup(s => s.Message.Sender).Returns(GameStarterAddress);

            this.mockInternalExecutor.Setup(s =>
                s.Call(
                    It.IsAny<ISmartContractState>(),
                    It.IsAny<Address>(),
                    It.IsAny<ulong>(),
                    It.IsAny<string>(),
                    It.IsAny<object[]>(),
                    It.IsAny<ulong>()))
                .Returns(TransferResult.Empty());

            starter.StartPingPong(pingPongTimes);

            this.mockPersistentState.Verify(s => s.SetInt32(nameof(Starter.PingPongTimes), pingPongTimes), Times.Once);
            this.mockPersistentState.Verify(s => s.GetAddress(nameof(Starter.GamePlayer)), Times.Once);
            this.mockPersistentState.Verify(s => s.SetUInt32(nameof(Starter.State), (uint)Starter.StateType.PingPonging), Times.Once);

            this.mockInternalExecutor.Verify(s =>
                s.Call(
                    this.mockContractState.Object,
                    GamePlayerAddress,
                    0,
                    nameof(Player.Ping),
                    It.Is<object[]>(o => (int)o[0] == pingPongTimes),
                    0));
        }

        [Fact]
        public void Pong_Sender_Not_GamePlayer_Fails()
        {
            var starter = this.NewStarter();

            this.mockContractState.Setup(s => s.Message.Sender).Returns(Address.Zero);

            Assert.Throws<SmartContractAssertException>(() => starter.Pong(int.MaxValue));
        }

        [Fact]
        public void Pong_CurrentPingPongTimes_GreaterThan0()
        {
            var starter = this.NewStarter();

            var currentPingPongTimes = int.MaxValue;
            var currentPingPongTimesMinus1 = currentPingPongTimes - 1;

            // Only the player contract can call this.
            this.mockContractState.Setup(s => s.Message.Sender).Returns(GamePlayerAddress);

            this.mockInternalExecutor.Setup(s =>
                s.Call(
                    It.IsAny<ISmartContractState>(),
                    It.IsAny<Address>(),
                    It.IsAny<ulong>(),
                    It.IsAny<string>(),
                    It.IsAny<object[]>(),
                    It.IsAny<ulong>()))
                .Returns(TransferResult.Empty());

            starter.Pong(currentPingPongTimes);

            // Verify we call back with the pong == ping - 1.
            this.mockInternalExecutor.Verify(s => s.Call(this.mockContractState.Object, GamePlayerAddress, 0, nameof(Player.Ping), It.Is<object[]>(o => (int)o[0] == currentPingPongTimesMinus1), 0));
            this.mockPersistentState.Verify(s => s.SetUInt32(nameof(Starter.State), (uint)Starter.StateType.PingPonging), Times.Once);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(0)]
        [InlineData(int.MinValue)]
        public void Pong_CurrentPingPongTimes_LessThanEqual1(int currentPingPongTimes)
        {
            var starter = this.NewStarter();

            // Only the player contract can call this.
            this.mockContractState.Setup(s => s.Message.Sender).Returns(GamePlayerAddress);

            this.mockInternalExecutor.Setup(s =>
                s.Call(
                    It.IsAny<ISmartContractState>(),
                    It.IsAny<Address>(),
                    It.IsAny<ulong>(),
                    It.IsAny<string>(),
                    It.IsAny<object[]>(),
                    It.IsAny<ulong>()))
                .Returns(TransferResult.Empty());

            starter.Pong(currentPingPongTimes);

            this.mockInternalExecutor.Verify(s => s.Call(this.mockContractState.Object, GamePlayerAddress, 0, nameof(Player.FinishGame), null, 0));
            this.mockPersistentState.Verify(s => s.SetUInt32(nameof(Player.State), (uint)Player.StateType.GameFinished), Times.Once);
        }

        [Fact]
        public void FinishGame_Sender_Not_GamePlayer_Fails()
        {
            var starter = this.NewStarter();

            this.mockContractState.Setup(s => s.Message.Sender).Returns(Address.Zero);

            Assert.Throws<SmartContractAssertException>(() => starter.FinishGame());

            this.mockPersistentState.Verify(s => s.GetAddress(nameof(Starter.GamePlayer)), Times.Once);
        }

        [Fact]
        public void FinishGame_Success()
        {
            var starter = this.NewStarter();

            this.mockContractState.Setup(s => s.Message.Sender).Returns(GamePlayerAddress);

            starter.FinishGame();

            this.mockPersistentState.Verify(s => s.GetAddress(nameof(Starter.GamePlayer)), Times.Once);
            this.mockPersistentState.Verify(s => s.SetUInt32(nameof(Player.State), (uint)Player.StateType.GameFinished));
        }

        private Starter NewStarter()
        {
            this.mockContractState.Setup(s => s.Message.Sender).Returns(GameStarterAddress);
            this.mockPersistentState.Setup(s => s.GetAddress(nameof(Starter.GameStarter))).Returns(GameStarterAddress);
            this.mockPersistentState.Setup(s => s.GetAddress(nameof(Starter.GamePlayer))).Returns(GamePlayerAddress);

            // Set up Create to return a new player address.
            this.mockInternalExecutor.Setup(s =>
                s.Create<Player>(
                    It.IsAny<ISmartContractState>(),
                    It.IsAny<ulong>(),
                    It.IsAny<object[]>(),
                    It.IsAny<ulong>()))
                .Returns(CreateResult.Succeeded(GamePlayerAddress));

            var starter = new Starter(this.mockContractState.Object, "Test");

            this.mockPersistentState.Invocations.Clear();
            this.mockInternalExecutor.Invocations.Clear();

            return starter;
        }
    }
}
