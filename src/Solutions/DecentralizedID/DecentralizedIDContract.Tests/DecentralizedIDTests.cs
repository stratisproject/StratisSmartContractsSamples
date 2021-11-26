using Moq;
using Stratis.SmartContracts;
using Stratis.SmartContracts.CLR;
using System;
using System.Reflection;
using Xunit;

namespace DecentralizedIDContract.Tests
{
    public class DecentralizedIDTests
    {
        private const string SmartContractOwner = "0x0000000000000000000000000000000000000001";
        private static readonly Address SmartContractOwnerAddress = SmartContractOwner.HexToAddress();

        private readonly Mock<IContractLogger> mockContractLogger;
        private readonly Mock<ISmartContractState> mockContractState;
        private readonly Mock<IPersistentState> mockPersistentState;
        private readonly Mock<IInternalTransactionExecutor> mockInternalExecutor;

        public DecentralizedIDTests()
        {
            this.mockContractLogger = new Mock<IContractLogger>();
            this.mockContractState = new Mock<ISmartContractState>();
            this.mockPersistentState = new Mock<IPersistentState>();
            this.mockInternalExecutor = new Mock<IInternalTransactionExecutor>();
            this.mockContractState.Setup(s => s.PersistentState).Returns(this.mockPersistentState.Object);
            this.mockContractState.Setup(s => s.ContractLogger).Returns(this.mockContractLogger.Object);
            this.mockContractState.Setup(s => s.InternalTransactionExecutor).Returns(this.mockInternalExecutor.Object);
        }

        [Theory]
        [InlineData(nameof(DecentralizedID.SmartContractOwner))]
        public void Property_Setter_Is_Private(string propertyName)
        {
            Type type = typeof(DecentralizedID);

            PropertyInfo property = type.GetProperty(propertyName);

            Assert.True(property.SetMethod.IsPrivate);
        }

        [Fact]
        public void CreateDIDTest()
        {
            var contract = new DecentralizedID(this.mockContractState.Object);
            contract.CreateDID("Test");

            Assert.Matches(contract.GetDataOfDID(0), "Test");
        }

        [Fact]
        public void RevokeDIDTest()
        {
            var contract = new DecentralizedID(this.mockContractState.Object);
            contract.CreateDID("Test");
            contract.RevokeDID(0);

            Assert.DoesNotMatch(contract.GetDataOfDID(0), "Test");
        }

        private DecentralizedID NewDecentralizedID()
        {
            this.mockPersistentState.Setup(
                s => s.GetAddress(nameof(DecentralizedID.SmartContractOwner))
            ).Returns(SmartContractOwnerAddress);

            var result = new DecentralizedID(this.mockContractState.Object);
            this.mockPersistentState.Invocations.Clear();
            return result;
        }

    }
}
