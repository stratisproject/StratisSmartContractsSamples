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
        private const string PlayerAdd = "0x0000000000000000000000000000000000000001";
        private const string Opponent = "0x0000000000000000000000000000000000000002";

        private static readonly Address PlayerAddress = PlayerAdd.HexToAddress();
        private static readonly Address OpponentAddress = Opponent.HexToAddress();

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
        [InlineData(nameof(Player.GameName))]
        [InlineData(nameof(Player.PlayerAddress))]
        [InlineData(nameof(Player.Opponent))]
        [InlineData(nameof(Player.PingsSent))]
        [InlineData(nameof(Player.PingsReceived))]
        public void Property_Setter_Is_Private(string propertyName)
        {
            Type type = typeof(Player);

            PropertyInfo property = type.GetProperty(propertyName);

            Assert.True(property.SetMethod.IsPrivate);
        }

        [Fact]
        public void Constructor_Sets_Initial_Values()
        {
            this.mockContractState.Setup(s => s.Message.Sender).Returns(PlayerAddress);

            var player = new Player(this.mockContractState.Object, PlayerAddress, OpponentAddress, "Test");

            this.mockPersistentState.Verify(s => s.SetAddress(nameof(Player.PlayerAddress), PlayerAddress), Times.Once);
            this.mockPersistentState.Verify(s => s.SetAddress(nameof(Player.Opponent), OpponentAddress), Times.Once);
            this.mockPersistentState.Verify(s => s.SetString(nameof(Player.GameName), "Test"), Times.Once);
            this.mockPersistentState.Verify(s => s.SetUInt32(nameof(Player.State), (uint)Player.StateType.Provisioned), Times.Once);
        }

        [Fact]
        public void SendPing_Sender_Not_Player_Fails()
        {
            var player = this.NewPlayer();

            this.mockContractState.Setup(s => s.Message.Sender).Returns(Address.Zero);

            Assert.Throws<SmartContractAssertException>(() => player.SendPing());
        }

        [Theory]
        [InlineData((uint)Player.StateType.Finished)]
        [InlineData((uint)Player.StateType.SentPing)]
        public void SendPing_State_Not_Received_Or_Provisioned_Player_Fails(uint state)
        {
            var player = this.NewPlayer();

            this.mockContractState.Setup(s => s.Message.Sender).Returns(PlayerAddress);
            this.mockPersistentState.Setup(s => s.GetUInt32(nameof(Player.State))).Returns(state);

            Assert.Throws<SmartContractAssertException>(() => player.SendPing());
        }

        [Fact]
        public void SendPing_Opponent_Finished_Success()
        {
            var player = this.NewPlayer();

            this.mockContractState.Setup(s => s.Message.Sender).Returns(PlayerAddress);
            this.mockPersistentState.Setup(s => s.GetUInt32(nameof(Player.State))).Returns((uint)Player.StateType.ReceivedPing);

            // Return true
            this.mockInternalExecutor.Setup(s =>
                s.Call(
                    It.IsAny<ISmartContractState>(),
                    It.IsAny<Address>(),
                    It.IsAny<ulong>(),
                    nameof(Player.IsFinished),
                    It.IsAny<object[]>(),
                    It.IsAny<ulong>()))
                .Returns(TransferResult.Transferred(true));

            player.SendPing();

            this.mockInternalExecutor.Verify(s => s.Call(this.mockContractState.Object, OpponentAddress, 0, nameof(Player.IsFinished), null, 0));
            this.mockPersistentState.Verify(s => s.SetUInt32(nameof(Player.State), (uint)Player.StateType.Finished), Times.Once);
        }

        [Theory]
        [InlineData(uint.MinValue, 1)]
        [InlineData(uint.MaxValue, 0)] // uint.MaxValue should overflow
        public void SendPing_Opponent_Not_Finished_Success(uint initialPingsSent, uint expectedPingsSent)
        {
            var player = this.NewPlayer();

            this.mockContractState.Setup(s => s.Message.Sender).Returns(PlayerAddress);
            this.mockPersistentState.Setup(s => s.GetUInt32(nameof(Player.State))).Returns((uint)Player.StateType.ReceivedPing);
            this.mockPersistentState.Setup(s => s.GetUInt32(nameof(Player.PingsSent))).Returns(initialPingsSent);

            // Return false
            this.mockInternalExecutor.Setup(s =>
                s.Call(
                    It.IsAny<ISmartContractState>(),
                    It.IsAny<Address>(),
                    It.IsAny<ulong>(),
                    nameof(Player.IsFinished),
                    It.IsAny<object[]>(),
                    It.IsAny<ulong>()))
                .Returns(TransferResult.Transferred(false));

            this.mockInternalExecutor.Setup(s =>
                s.Call(
                    It.IsAny<ISmartContractState>(),
                    It.IsAny<Address>(),
                    It.IsAny<ulong>(),
                    nameof(Player.ReceivePing),
                    It.IsAny<object[]>(),
                    It.IsAny<ulong>()))
                .Returns(TransferResult.Empty());

            player.SendPing();

            this.mockInternalExecutor.Verify(s => s.Call(this.mockContractState.Object, OpponentAddress, 0, nameof(Player.IsFinished), null, 0));
            this.mockInternalExecutor.Verify(s => s.Call(this.mockContractState.Object, OpponentAddress, 0, nameof(Player.ReceivePing), null, 0));
            this.mockPersistentState.Verify(s => s.SetUInt32(nameof(Player.PingsSent), expectedPingsSent), Times.Once);
            this.mockPersistentState.Verify(s => s.SetUInt32(nameof(Player.State), (uint)Player.StateType.SentPing), Times.Once);
        }

        [Fact]
        public void ReceivePing_Sender_Not_Player_Fails()
        {
            var player = this.NewPlayer();

            this.mockContractState.Setup(s => s.Message.Sender).Returns(Address.Zero);

            Assert.Throws<SmartContractAssertException>(() => player.ReceivePing());
        }

        [Theory]
        [InlineData((uint)Player.StateType.Finished)]
        [InlineData((uint)Player.StateType.ReceivedPing)]
        public void ReceivePing_State_Not_Sent_Or_Provisioned_Player_Fails(uint state)
        {
            var player = this.NewPlayer();

            this.mockContractState.Setup(s => s.Message.Sender).Returns(PlayerAddress);
            this.mockPersistentState.Setup(s => s.GetUInt32(nameof(Player.State))).Returns(state);

            Assert.Throws<SmartContractAssertException>(() => player.ReceivePing());
        }

        [Theory]
        [InlineData(uint.MinValue, 1)]
        [InlineData(uint.MaxValue, 0)] // uint.MaxValue should overflow
        public void ReceivePing_Success(uint initialPingsReceived, uint expectedPingsReceived)
        {
            var player = this.NewPlayer();

            this.mockContractState.Setup(s => s.Message.Sender).Returns(OpponentAddress);
            this.mockPersistentState.Setup(s => s.GetUInt32(nameof(Player.State))).Returns((uint)Player.StateType.SentPing);
            this.mockPersistentState.Setup(s => s.GetUInt32(nameof(Player.PingsReceived))).Returns(initialPingsReceived);

            player.ReceivePing();

            this.mockPersistentState.Verify(s => s.SetUInt32(nameof(Player.PingsReceived), expectedPingsReceived), Times.Once);
            this.mockPersistentState.Verify(s => s.SetUInt32(nameof(Player.State), (uint)Player.StateType.ReceivedPing), Times.Once);
        }

        [Fact]
        public void FinishGame_Sender_Not_GameStarter_Fails()
        {
            this.mockContractState.Setup(s => s.Message.Sender).Returns(Address.Zero);

            var player = this.NewPlayer();

            this.mockContractState.Setup(s => s.Message.Sender).Returns(Address.Zero);
            Assert.Throws<SmartContractAssertException>(() => player.FinishGame());

            this.mockPersistentState.Verify(s => s.GetAddress(nameof(Player.PlayerAddress)), Times.Once);
        }

        [Fact]
        public void FinishGame_Success()
        {
            var player = this.NewPlayer();

            this.mockContractState.Setup(s => s.Message.Sender).Returns(PlayerAddress);

            player.FinishGame();

            this.mockPersistentState.Verify(s => s.SetUInt32(nameof(Player.State), (uint)Player.StateType.Finished));
        }

        private Player NewPlayer()
        {
            this.mockContractState.Setup(s => s.Message.Sender).Returns(PlayerAddress);
            this.mockPersistentState.Setup(s => s.GetAddress(nameof(Player.PlayerAddress))).Returns(PlayerAddress);
            this.mockPersistentState.Setup(s => s.GetAddress(nameof(Player.Opponent))).Returns(OpponentAddress);

            var player = new Player(this.mockContractState.Object, PlayerAddress, OpponentAddress, "Test");

            this.mockPersistentState.Invocations.Clear();
            this.mockInternalExecutor.Invocations.Clear();

            return player;
        }
    }
}
