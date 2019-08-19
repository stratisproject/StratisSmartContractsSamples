using Moq;
using Stratis.SmartContracts;
using Stratis.SmartContracts.CLR;
using System;
using System.Reflection;
using Xunit;

namespace BasicProvenance.Tests
{
    public class BasicProvenanceTests
    {
        private const string InitiatingCounterParty = "0x0000000000000000000000000000000000000001";
        private const string CounterParty = "0x0000000000000000000000000000000000000002";
        private const string PreviousCounterParty = "0x0000000000000000000000000000000000000003";
        private const string SupplyChainOwner = "0x0000000000000000000000000000000000000004";
        private const string SupplyChainObserver = "0x0000000000000000000000000000000000000005";
        private const string Intermediary = "0x0000000000000000000000000000000000000006";

        private static readonly Address InitiatingCounterPartyAddress = InitiatingCounterParty.HexToAddress();
        private static readonly Address CounterPartyAddress = CounterParty.HexToAddress();
        private static readonly Address PreviousCounterPartyAddress = PreviousCounterParty.HexToAddress();
        private static readonly Address SupplyChainOwnerAddress = SupplyChainOwner.HexToAddress();
        private static readonly Address SupplyChainObserverAddress = SupplyChainObserver.HexToAddress();
        private static readonly Address IntermediaryAddress = Intermediary.HexToAddress();
        private readonly Mock<ISmartContractState> mockContractState;
        private readonly Mock<IPersistentState> mockPersistentState;
        private readonly Mock<IInternalTransactionExecutor> mockInternalExecutor;
        private readonly Mock<IContractLogger> mockContractLogger;

        public BasicProvenanceTests()
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
        [InlineData(nameof(BasicProvenanceContract.State))]
        [InlineData(nameof(BasicProvenanceContract.InitiatingCounterParty))]
        [InlineData(nameof(BasicProvenanceContract.CounterParty))]
        [InlineData(nameof(BasicProvenanceContract.PreviousCounterParty))]
        [InlineData(nameof(BasicProvenanceContract.SupplyChainOwner))]
        [InlineData(nameof(BasicProvenanceContract.SupplyChainObserver))]
        public void Property_Setter_Is_Private(string propertyName)
        {
            Type type = typeof(BasicProvenanceContract);

            PropertyInfo property = type.GetProperty(propertyName);

            Assert.True(property.SetMethod.IsPrivate);
        }

        [Fact]
        public void Constructor_Sets_Initial_Values()
        {
            this.mockContractState.Setup(s => s.Message.Sender).Returns(InitiatingCounterPartyAddress);

            var basicProvenance = new BasicProvenanceContract(this.mockContractState.Object, SupplyChainOwnerAddress, SupplyChainObserverAddress);

            this.mockPersistentState.Verify(s => s.SetAddress(nameof(BasicProvenanceContract.InitiatingCounterParty), InitiatingCounterPartyAddress));
            this.mockPersistentState.Verify(s => s.SetAddress(nameof(BasicProvenanceContract.CounterParty), InitiatingCounterPartyAddress));
            this.mockPersistentState.Verify(s => s.SetAddress(nameof(BasicProvenanceContract.SupplyChainOwner), SupplyChainOwnerAddress));
            this.mockPersistentState.Verify(s => s.SetAddress(nameof(BasicProvenanceContract.SupplyChainObserver), SupplyChainObserverAddress));
            this.mockPersistentState.Verify(s => s.SetUInt32(nameof(BasicProvenanceContract.State), (uint)BasicProvenanceContract.StateType.Created));
        }

        [Fact]
        public void TransferResponsibility_Fails_Sender_Not_CounterParty()
        {
            var contract = this.NewBasicProvenanceContract();

            // Setup incorrect sender.
            this.mockContractState.Setup(s => s.Message.Sender).Returns(Address.Zero);

            // Setup counterparty address.
            this.mockPersistentState.Setup(s => s.GetAddress(nameof(BasicProvenanceContract.CounterParty))).Returns(CounterPartyAddress);

            // Attempt to transfer to any address should fail.
            Assert.Throws<SmartContractAssertException>(() => contract.TransferResponsibility(Address.Zero));
        }

        [Fact]
        public void TransferResponsibility_Fails_State_Is_Completed()
        {
            var contract = this.NewBasicProvenanceContract();

            // Setup correct sender.
            this.mockContractState.Setup(s => s.Message.Sender).Returns(CounterPartyAddress);

            // Setup counterparty address.
            this.mockPersistentState.Setup(s => s.GetAddress(nameof(BasicProvenanceContract.CounterParty))).Returns(CounterPartyAddress);

            // Setup state = completed.
            this.mockPersistentState.Setup(s => s.GetUInt32(nameof(BasicProvenanceContract.State))).Returns((uint)BasicProvenanceContract.StateType.Completed);

            // Attempt to transfer to any address should fail.
            Assert.Throws<SmartContractAssertException>(() => contract.TransferResponsibility(Address.Zero));
        }

        [Fact]
        public void TransferResponsibility_Succeeds_State_Is_Created()
        {
            var contract = this.NewBasicProvenanceContract();

            // Setup correct sender.
            this.mockContractState.Setup(s => s.Message.Sender).Returns(CounterPartyAddress);

            // Setup counterparty address.
            this.mockPersistentState.Setup(s => s.GetAddress(nameof(BasicProvenanceContract.CounterParty))).Returns(CounterPartyAddress);

            // Setup state = created.
            this.mockPersistentState.Setup(s => s.GetUInt32(nameof(BasicProvenanceContract.State))).Returns((uint)BasicProvenanceContract.StateType.Created);

            // Attempt to transfer to any address should succeed.
            contract.TransferResponsibility(IntermediaryAddress);

            this.mockPersistentState.Verify(s => s.SetUInt32(nameof(BasicProvenanceContract.State), (uint)BasicProvenanceContract.StateType.InTransit), Times.Once);
            this.mockPersistentState.Verify(s => s.SetAddress(nameof(BasicProvenanceContract.PreviousCounterParty), CounterPartyAddress), Times.Once);
            this.mockPersistentState.Verify(s => s.SetAddress(nameof(BasicProvenanceContract.CounterParty), IntermediaryAddress), Times.Once);
        }

        [Fact]
        public void TransferResponsibility_Succeeds_State_Is_InTransit()
        {
            var contract = this.NewBasicProvenanceContract();

            // Setup correct sender.
            this.mockContractState.Setup(s => s.Message.Sender).Returns(CounterPartyAddress);

            // Setup counterparty address.
            this.mockPersistentState.Setup(s => s.GetAddress(nameof(BasicProvenanceContract.CounterParty))).Returns(CounterPartyAddress);

            // Setup state = in transit.
            this.mockPersistentState.Setup(s => s.GetUInt32(nameof(BasicProvenanceContract.State))).Returns((uint)BasicProvenanceContract.StateType.InTransit);

            // Attempt to transfer to any address should succeed.
            contract.TransferResponsibility(IntermediaryAddress);

            // Make sure the state is not changed.
            this.mockPersistentState.Verify(s => s.SetUInt32(nameof(BasicProvenanceContract.State), It.IsAny<uint>()), Times.Never);

            this.mockPersistentState.Verify(s => s.SetAddress(nameof(BasicProvenanceContract.PreviousCounterParty), CounterPartyAddress), Times.Once);
            this.mockPersistentState.Verify(s => s.SetAddress(nameof(BasicProvenanceContract.CounterParty), IntermediaryAddress), Times.Once);
        }

        [Fact]
        public void Complete_Fails_Sender_Not_SupplyChainOwner()
        {
            var contract = this.NewBasicProvenanceContract();

            // Setup incorrect sender.
            this.mockContractState.Setup(s => s.Message.Sender).Returns(Address.Zero);

            // Setup supplychainowner address.
            this.mockPersistentState.Setup(s => s.GetAddress(nameof(BasicProvenanceContract.SupplyChainOwner))).Returns(SupplyChainOwnerAddress);

            // Attempt to call completion with incorrect sender should fail.
            Assert.Throws<SmartContractAssertException>(() => contract.Complete());
        }

        [Fact]
        public void Complete_Fails_State_Is_Completed()
        {
            var contract = this.NewBasicProvenanceContract();

            // Setup correct sender.
            this.mockContractState.Setup(s => s.Message.Sender).Returns(SupplyChainOwnerAddress);

            // Setup correct supplychainowner address.
            this.mockPersistentState.Setup(s => s.GetAddress(nameof(BasicProvenanceContract.SupplyChainOwner))).Returns(SupplyChainOwnerAddress);

            // Setup state = completed.
            this.mockPersistentState.Setup(s => s.GetUInt32(nameof(BasicProvenanceContract.State))).Returns((uint)BasicProvenanceContract.StateType.Completed);

            // Attempt to call completion with incorrect state should fail.
            Assert.Throws<SmartContractAssertException>(() => contract.Complete());
        }

        [Theory]
        [InlineData((uint)BasicProvenanceContract.StateType.InTransit)]
        [InlineData((uint)BasicProvenanceContract.StateType.Created)]
        public void Complete_Succeeds(uint state)
        {
            var contract = this.NewBasicProvenanceContract();

            // Setup correct sender.
            this.mockContractState.Setup(s => s.Message.Sender).Returns(SupplyChainOwnerAddress);

            // Setup correct supplychainowner address.
            this.mockPersistentState.Setup(s => s.GetAddress(nameof(BasicProvenanceContract.SupplyChainOwner))).Returns(SupplyChainOwnerAddress);

            // Setup correct counterparty address.
            this.mockPersistentState.Setup(s => s.GetAddress(nameof(BasicProvenanceContract.CounterParty))).Returns(CounterPartyAddress);

            // Setup current state.
            this.mockPersistentState.Setup(s => s.GetUInt32(nameof(BasicProvenanceContract.State))).Returns(state);

            // Attempt to call completion with incorrect state should fail.
            contract.Complete();

            this.mockPersistentState.Verify(s => s.SetUInt32(nameof(BasicProvenanceContract.State), (uint)BasicProvenanceContract.StateType.Completed));
            this.mockPersistentState.Verify(s => s.SetAddress(nameof(BasicProvenanceContract.PreviousCounterParty), CounterPartyAddress));
            this.mockPersistentState.Verify(s => s.SetAddress(nameof(BasicProvenanceContract.CounterParty), Address.Zero));
        }

        private BasicProvenanceContract NewBasicProvenanceContract()
        {
            this.mockContractState.Setup(s => s.Message.Sender).Returns(InitiatingCounterPartyAddress);

            var result = new BasicProvenanceContract(this.mockContractState.Object, SupplyChainOwnerAddress, SupplyChainObserverAddress);

            // Reset the invocations that happened in the constructor so we don't accidentally test them.
            this.mockPersistentState.Invocations.Clear();

            return result;
        }
    }
}
