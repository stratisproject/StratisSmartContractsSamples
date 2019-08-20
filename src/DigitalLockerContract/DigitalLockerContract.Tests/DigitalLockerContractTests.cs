using Moq;
using Stratis.SmartContracts;
using Stratis.SmartContracts.CLR;
using System;
using System.Reflection;
using Xunit;

namespace DigitalLockerContract.Tests
{
    public class DigitalLockerContractTests
    {
        private const string Owner = "0x0000000000000000000000000000000000000001";
        private const string BankAgent = "0x0000000000000000000000000000000000000002";

        private static readonly Address OwnerAddress = Owner.HexToAddress();
        private static readonly Address BankAgentAddress = BankAgent.HexToAddress();

        private readonly Mock<ISmartContractState> mockContractState;
        private readonly Mock<IPersistentState> mockPersistentState;
        private readonly Mock<IInternalTransactionExecutor> mockInternalExecutor;
        private readonly Mock<IContractLogger> mockContractLogger;

        public DigitalLockerContractTests()
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
        [InlineData(nameof(DigitalLocker.Owner))]
        [InlineData(nameof(DigitalLocker.BankAgent))]
        [InlineData(nameof(DigitalLocker.LockerFriendlyName))]
        [InlineData(nameof(DigitalLocker.CurrentAuthorizedUser))]
        [InlineData(nameof(DigitalLocker.ExpirationDate))]
        [InlineData(nameof(DigitalLocker.Image))]
        [InlineData(nameof(DigitalLocker.ThirdPartyRequestor))]
        [InlineData(nameof(DigitalLocker.IntendedPurpose))]
        [InlineData(nameof(DigitalLocker.LockerStatus))]
        [InlineData(nameof(DigitalLocker.RejectionReason))]
        [InlineData(nameof(DigitalLocker.State))]
        public void Property_Setter_Is_Private(string propertyName)
        {
            Type type = typeof(DigitalLocker);

            PropertyInfo property = type.GetProperty(propertyName);

            Assert.True(property.SetMethod.IsPrivate);
        }

        [Fact]
        public void Constructor_Sets_Initial_Values()
        {
            this.mockContractState.Setup(s => s.Message.Sender).Returns(OwnerAddress);

            var digitalLocker = new DigitalLocker(this.mockContractState.Object, "Test", BankAgentAddress);

            this.mockPersistentState.Verify(s => s.SetAddress(nameof(DigitalLocker.Owner), OwnerAddress), Times.Once);
            this.mockPersistentState.Verify(s => s.SetString(nameof(DigitalLocker.LockerFriendlyName), "Test"), Times.Once);
            this.mockPersistentState.Verify(s => s.SetAddress(nameof(DigitalLocker.BankAgent), BankAgentAddress), Times.Once);
            this.mockPersistentState.Verify(s => s.SetUInt32(nameof(DigitalLocker.State), (uint)DigitalLocker.StateType.DocumentReview), Times.Once);
        }
    }
}
