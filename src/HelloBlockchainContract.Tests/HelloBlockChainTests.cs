using Moq;
using Stratis.SmartContracts;
using Stratis.SmartContracts.CLR;
using System;
using System.Reflection;
using Xunit;

namespace HelloBlockchainContract.Tests
{
    public class HelloBlockchainTests
    {
        private const string Requestor = "0x0000000000000000000000000000000000000001";
        private const string Responder = "0x0000000000000000000000000000000000000002";

        private static readonly Address RequestorAddress = Requestor.HexToAddress();
        private static readonly Address ResponderAddress = Responder.HexToAddress();

        private readonly Mock<ISmartContractState> mockContractState;
        private readonly Mock<IPersistentState> mockPersistentState;
        private readonly Mock<IInternalTransactionExecutor> mockInternalExecutor;
        private readonly Mock<IContractLogger> mockContractLogger;

        public HelloBlockchainTests()
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
        [InlineData(nameof(HelloBlockchain.State))]
        [InlineData(nameof(HelloBlockchain.Requestor))]
        [InlineData(nameof(HelloBlockchain.Responder))]
        [InlineData(nameof(HelloBlockchain.RequestMessage))]
        [InlineData(nameof(HelloBlockchain.ResponseMessage))]
        public void Property_Setter_Is_Private(string propertyName)
        {
            Type type = typeof(HelloBlockchain);

            PropertyInfo property = type.GetProperty(propertyName);

            Assert.True(property.SetMethod.IsPrivate);
        }

        [Fact]
        public void Constructor_Sets_Initial_Values()
        {
            this.mockContractState.Setup(s => s.Message.Sender).Returns(RequestorAddress);

            var contract = new HelloBlockchain(this.mockContractState.Object, "Test");

            this.mockPersistentState.Verify(s => s.SetAddress(nameof(HelloBlockchain.Requestor), RequestorAddress), Times.Once);
            this.mockPersistentState.Verify(s => s.SetString(nameof(HelloBlockchain.RequestMessage), "Test"), Times.Once);
            this.mockPersistentState.Verify(s => s.SetUInt32(nameof(HelloBlockchain.State), (uint)HelloBlockchain.StateType.Request), Times.Once);
        }

        [Fact]
        public void SendRequest_Fails_Sender_Not_Requestor()
        {
            // Sender is not requestor.
            this.mockContractState.Setup(s => s.Message.Sender).Returns(Address.Zero);

            var contract = this.NewHelloBlockchain();

            Assert.Throws<SmartContractAssertException>(() => contract.SendRequest("Test Request"));

            this.mockPersistentState.Verify(s => s.SetAddress(nameof(HelloBlockchain.Requestor), It.IsAny<Address>()), Times.Never);
            this.mockPersistentState.Verify(s => s.SetString(nameof(HelloBlockchain.RequestMessage), It.IsAny<string>()), Times.Never);
            this.mockPersistentState.Verify(s => s.SetUInt32(nameof(HelloBlockchain.State), It.IsAny<uint>()), Times.Never);
        }

        [Fact]
        public void SendRequest_Succeeds()
        {
            // Sender is requestor.
            this.mockContractState.Setup(s => s.Message.Sender).Returns(RequestorAddress);

            var contract = this.NewHelloBlockchain();

            contract.SendRequest("Test Request");

            this.mockPersistentState.Verify(s => s.SetString(nameof(HelloBlockchain.RequestMessage), "Test Request"), Times.Once);
            this.mockPersistentState.Verify(s => s.SetUInt32(nameof(HelloBlockchain.State), (uint)HelloBlockchain.StateType.Request), Times.Once);
        }

        [Fact]
        public void SendResponse_Succeeds()
        {
            this.mockContractState.Setup(s => s.Message.Sender).Returns(RequestorAddress);

            var contract = this.NewHelloBlockchain();

            this.mockContractState.Setup(s => s.Message.Sender).Returns(ResponderAddress);

            contract.SendResponse("Test Response");

            this.mockPersistentState.Verify(s => s.SetAddress(nameof(HelloBlockchain.Responder), ResponderAddress), Times.Once);
            this.mockPersistentState.Verify(s => s.SetString(nameof(HelloBlockchain.ResponseMessage), "Test Response"), Times.Once);
            this.mockPersistentState.Verify(s => s.SetUInt32(nameof(HelloBlockchain.State), (uint)HelloBlockchain.StateType.Respond), Times.Once);
        }

        private HelloBlockchain NewHelloBlockchain()
        {
            this.mockPersistentState.Setup(s => s.GetAddress(nameof(HelloBlockchain.Requestor))).Returns(RequestorAddress);
            this.mockPersistentState.Setup(s => s.GetString(nameof(HelloBlockchain.RequestMessage))).Returns("Test");

            var result = new HelloBlockchain(this.mockContractState.Object, "Test");

            // Clear the constructor invocations
            this.mockPersistentState.Invocations.Clear();

            return result;
        }
    }
}
