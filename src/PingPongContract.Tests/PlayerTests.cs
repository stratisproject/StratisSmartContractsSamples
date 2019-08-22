using Moq;
using Stratis.SmartContracts;
using Stratis.SmartContracts.CLR;
using System;
using System.Reflection;
using Xunit;

namespace PingPongContract.Tests
{
    public class PlayerTests
    {
        private const string StarterContract = "0x0000000000000000000000000000000000000001";

        private static readonly Address StarterContractAddress = StarterContract.HexToAddress();

        private readonly Mock<ISmartContractState> mockContractState;
        private readonly Mock<IPersistentState> mockPersistentState;
        private readonly Mock<IInternalTransactionExecutor> mockInternalExecutor;
        private readonly Mock<IContractLogger> mockContractLogger;

        public PlayerTests()
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
        [InlineData(nameof(Player.State))]
        [InlineData(nameof(Player.PingPongGameName))]
        [InlineData(nameof(Player.GameStarter))]
        public void Property_Setter_Is_Private(string propertyName)
        {
            Type type = typeof(Player);

            PropertyInfo property = type.GetProperty(propertyName);

            Assert.True(property.SetMethod.IsPrivate);
        }

        [Fact]
        public void Constructor_Sets_Initial_Values()
        {
            this.mockContractState.Setup(s => s.Message.Sender).Returns(StarterContractAddress);

            var player = new Player(this.mockContractState.Object, "Test");

            this.mockPersistentState.Verify(s => s.SetAddress(nameof(Player.GameStarter), StarterContractAddress), Times.Once);
            this.mockPersistentState.Verify(s => s.SetString(nameof(Player.PingPongGameName), "Test"), Times.Once);
            this.mockPersistentState.Verify(s => s.SetUInt32(nameof(Player.State), (uint)Player.StateType.PingPongPlayerCreated), Times.Once);
        }

        [Fact]
        public void Ping_Sender_Not_GameStarter_Fails()
        {
            var player = this.NewPlayer();

            this.mockContractState.Setup(s => s.Message.Sender).Returns(Address.Zero);

            Assert.Throws<SmartContractAssertException>(() => player.Ping(int.MaxValue));
        }

        [Fact]
        public void Ping_CurrentPingPongTimes_GreaterThan0()
        {
            var player = this.NewPlayer();

            var currentPingPongTimes = int.MaxValue;
            var currentPingPongTimesMinus1 = currentPingPongTimes - 1;

            // Only the starter contract can call this.
            this.mockContractState.Setup(s => s.Message.Sender).Returns(StarterContractAddress);

            this.mockInternalExecutor.Setup(s =>
                s.Call(
                    It.IsAny<ISmartContractState>(),
                    It.IsAny<Address>(),
                    It.IsAny<ulong>(),
                    It.IsAny<string>(),
                    It.IsAny<object[]>(),
                    It.IsAny<ulong>()))
                .Returns(TransferResult.Empty());

            player.Ping(currentPingPongTimes);

            // Verify we call back with the pong == ping - 1.
            this.mockInternalExecutor.Verify(s => s.Call(this.mockContractState.Object, StarterContractAddress, 0, nameof(Starter.Pong), It.Is<object[]>(o => (int)o[0] == currentPingPongTimesMinus1), 0));
            this.mockPersistentState.Verify(s => s.SetUInt32(nameof(Starter.State), (uint)Player.StateType.PingPonging), Times.Once);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(0)]
        public void Ping_CurrentPingPongTimes_LessThanEqual1(uint currentPingPongTimes)
        {
            var starter = this.NewPlayer();

            // Only the player contract can call this.
            this.mockContractState.Setup(s => s.Message.Sender).Returns(StarterContractAddress);

            this.mockInternalExecutor.Setup(s =>
                s.Call(
                    It.IsAny<ISmartContractState>(),
                    It.IsAny<Address>(),
                    It.IsAny<ulong>(),
                    It.IsAny<string>(),
                    It.IsAny<object[]>(),
                    It.IsAny<ulong>()))
                .Returns(TransferResult.Empty());

            starter.Ping(currentPingPongTimes);

            this.mockInternalExecutor.Verify(s => s.Call(this.mockContractState.Object, StarterContractAddress, 0, nameof(Starter.FinishGame), null, 0));
            this.mockPersistentState.Verify(s => s.SetUInt32(nameof(Player.State), (uint)Player.StateType.GameFinished), Times.Once);
        }

        [Fact]
        public void FinishGame_Sender_Not_GameStarter_Fails()
        {
            this.mockContractState.Setup(s => s.Message.Sender).Returns(Address.Zero);

            var player = this.NewPlayer();

            Assert.Throws<SmartContractAssertException>(() => player.FinishGame());

            this.mockPersistentState.Verify(s => s.GetAddress(nameof(Player.GameStarter)), Times.Once);
        }

        [Fact]
        public void FinishGame_Success()
        {
            var player = this.NewPlayer();

            player.FinishGame();

            this.mockPersistentState.Verify(s => s.SetUInt32(nameof(Player.State), (uint)Player.StateType.GameFinished));
        }

        private Player NewPlayer()
        {
            this.mockContractState.Setup(s => s.Message.Sender).Returns(StarterContractAddress);
            this.mockPersistentState.Setup(s => s.GetAddress(nameof(Player.GameStarter))).Returns(StarterContractAddress);

            var player = new Player(this.mockContractState.Object, "Test");

            this.mockPersistentState.Invocations.Clear();
            this.mockInternalExecutor.Invocations.Clear();

            return player;
        }
    }
}
