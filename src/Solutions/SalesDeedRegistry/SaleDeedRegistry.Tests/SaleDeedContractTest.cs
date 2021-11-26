using Moq;
using Xunit;
using Stratis.SmartContracts;
using Stratis.SmartContracts.CLR;
using static SaleDeedRegistryContract;
using System;
using System.Reflection;
using Moq.Protected;
using Stratis.SmartContracts.CLR.Serialization;
using System.ComponentModel;
using SaleDeedRegistry.Lib.Helpers;

namespace SaleDeedRegistry.Tests
{
    public class SaleDeedContractTest
    {
        private readonly Mock<ITransferResult> transferResult;
        private readonly Mock<ISmartContractState> mockContractState;
        private readonly Mock<IPersistentState> mockPersistentState;
        private readonly Mock<IContractLogger> mockContractLogger;
        private readonly Mock<IInternalTransactionExecutor> mockInternalExecutor;

        Address sender;
        Address buyer;
        Address payee;
        Address propertyOwner;
        string assetId = "";
        
        public SaleDeedContractTest()
        {
            transferResult = new Mock<ITransferResult>();
            mockContractLogger = new Mock<IContractLogger>();
            mockPersistentState = new Mock<IPersistentState>();
            mockContractState = new Mock<ISmartContractState>();
            mockInternalExecutor = new Mock<IInternalTransactionExecutor>();

            this.mockContractState.Setup(s => s.PersistentState).Returns(this.mockPersistentState.Object);
            this.mockContractState.Setup(s => s.ContractLogger).Returns(this.mockContractLogger.Object);
            this.mockContractState.Setup(x => x.InternalTransactionExecutor).Returns(mockInternalExecutor.Object);

            assetId = UniqueIdHelper.GenerateId(); 
            this.sender = "0x0000000000000000000000000000000000000001".HexToAddress();
            this.buyer = "0x0000000000000000000000000000000000000002".HexToAddress();
            this.propertyOwner = "0x0000000000000000000000000000000000000003".HexToAddress();
            this.payee = "0x0000000000000000000000000000000000000004".HexToAddress();
        }

        [Fact]
        [Description("Constructor creating an instance of SaleDeedRegistryContract with parameters")]
        public void Constructor_Sets_Initial_Values()
        {
            SaleDeedRegistryContract saleDeedRegistry = 
                new SaleDeedRegistryContract(mockContractState.Object, payee);
            this.mockContractState.Setup(s => s.Message.Sender).Returns(Address.Zero);
        }

        [Fact]
        public void SaleDeed_Init_Application()
        {
            this.mockContractState.Setup(s => s.PersistentState).Returns(this.mockPersistentState.Object);
            SaleDeedRegistryContract saleDeedRegistry =
                new SaleDeedRegistryContract(mockContractState.Object, payee);

            Address address = "0x0000000000000000000000000000000000000002".HexToAddress();
            this.mockPersistentState.Setup(s => s.GetUInt32($"PropertyState[" + assetId + "]"))
             .Returns((uint)PropertyStateType.InProgress);

            saleDeedRegistry.InitApplication(propertyOwner, buyer, assetId);

            // Get the Property State and validate
            uint state = saleDeedRegistry.GetPropertyState(assetId);
            Assert.True(state == (uint)PropertyStateType.InProgress);
        }

        [Fact]
        public void SaleDeed_Start_Review_Process()
        {
            SaleDeedRegistryContract saleDeedRegistry =
                new SaleDeedRegistryContract(mockContractState.Object, payee);

            Address address = "0x0000000000000000000000000000000000000002".HexToAddress();

            this.mockPersistentState.Setup(s => s.GetUInt32($"PropertyState["+ assetId + "]"))
                .Returns((uint)PropertyStateType.InProgress);

            saleDeedRegistry.StartReviewProcess(propertyOwner, buyer, assetId);

            this.mockPersistentState.Setup(s => s.GetUInt32($"PropertyState[" + assetId + "]"))
               .Returns((uint)PropertyStateType.UnderReview);

            // Get the Property State and validate
            uint state = saleDeedRegistry.GetPropertyState(assetId);
            Assert.True(state == (uint)PropertyStateType.UnderReview);
        }

        [Fact]
        public void SaleDeed_Complete_Review_Process()
        {
            SaleDeedRegistryContract saleDeedRegistry =
                new SaleDeedRegistryContract(mockContractState.Object, payee);

            Address address = "0x0000000000000000000000000000000000000002".HexToAddress();

            this.mockPersistentState.Setup(s => s.GetUInt32($"PropertyState[" + assetId + "]"))
                .Returns((uint)PropertyStateType.UnderReview);

            this.mockPersistentState.Setup(s => s.GetAddress($"PropertyOwner[" + assetId + "]"))
               .Returns(propertyOwner);

            saleDeedRegistry.CompleteReviewProcess(propertyOwner, buyer, assetId);

            this.mockPersistentState.Setup(s => s.GetUInt32($"PropertyState[" + assetId + "]"))
               .Returns((uint)PropertyStateType.ReviewComplete);

            // Get the Property State and validate
            uint state = saleDeedRegistry.GetPropertyState(assetId);
            Assert.True(state == (uint)PropertyStateType.ReviewComplete);
        }


        [Fact]
        public void SaleDeed_Reject_Application()
        {
            SaleDeedRegistryContract saleDeedRegistry =
                new SaleDeedRegistryContract(mockContractState.Object, payee);

            Address address = "0x0000000000000000000000000000000000000002".HexToAddress();

            this.mockPersistentState.Setup(s => s.GetUInt32($"PropertyState[" + assetId + "]"))
                .Returns((uint)PropertyStateType.Rejected);

            saleDeedRegistry.RejectApplication(assetId);

            // Get the Property State and validate
            uint state = saleDeedRegistry.GetPropertyState(assetId);
            Assert.True(state == (uint)PropertyStateType.Rejected);
        }

        [Fact]
        public void SaleDeed_ReApply_Application()
        {
            SaleDeedRegistryContract saleDeedRegistry =
                new SaleDeedRegistryContract(mockContractState.Object, payee);

            Address address = "0x0000000000000000000000000000000000000002".HexToAddress();

            this.mockPersistentState.Setup(s => s.GetUInt32($"PropertyState[" + assetId + "]"))
                .Returns((uint)PropertyStateType.InProgress);

            saleDeedRegistry.ReApply(assetId);

            // Get the Property State and validate
            uint state = saleDeedRegistry.GetPropertyState(assetId);
            Assert.True(state == (uint)PropertyStateType.InProgress);
        }

        [Fact]
        public void SaleDeed_Pay_Transfer_Fee()
        {
            var moq = new Mock<SmartContract>();

            SaleDeedRegistryContract saleDeedRegistry =
                new SaleDeedRegistryContract(mockContractState.Object, payee);

            Address address = "0x0000000000000000000000000000000000000002".HexToAddress();

            this.mockPersistentState.Setup(s => s.GetUInt32($"PropertyState[" + assetId + "]"))
                .Returns((uint)PropertyStateType.ReviewComplete);

            this.mockPersistentState.Setup(s => s.GetAddress("PayeeAddress"))
              .Returns(payee);

            mockInternalExecutor.Setup(s =>
               s.Call(
                   It.IsAny<ISmartContractState>(),
                   It.IsAny<Address>(),
                   It.IsAny<ulong>(),
                   "Transfer",
                   It.IsAny<object[]>(),
                   It.IsAny<ulong>()))
               .Returns(transferResult.Object);

            saleDeedRegistry.PayApplicationTransferFee(buyer, payee, assetId, 2000);
            
            this.mockPersistentState.Setup(s => s.GetUInt32($"PropertyState[" + assetId + "]"))
               .Returns((uint)PropertyStateType.PaidTransferFee);

            // Get the Property State and validate
            uint state = saleDeedRegistry.GetPropertyState(assetId);
            Assert.True(state == (uint)PropertyStateType.PaidTransferFee);
        }

        [Fact]
        public void SaleDeed_Transfer_Ownership()
        {
            SaleDeedRegistryContract saleDeedRegistry =
                new SaleDeedRegistryContract(mockContractState.Object, payee);

            Address address = "0x0000000000000000000000000000000000000002".HexToAddress();

            this.mockPersistentState.Setup(s => s.GetUInt32($"PropertyState[" + assetId + "]"))
                .Returns((uint)PropertyStateType.PaidTransferFee);
            
            this.mockPersistentState.Setup(s => s.GetAddress($"PropertyOwner[" + assetId + "]"))
              .Returns(propertyOwner);

            this.mockPersistentState.Setup(s => s.GetBool($"EncumbranceCleared[" + assetId + "]"))
              .Returns(true);

            this.mockPersistentState.Setup(s => s.GetString($"AssetId[" + propertyOwner + "]"))
             .Returns(assetId);

            saleDeedRegistry.TransferOwnership(propertyOwner, buyer, assetId);

            this.mockPersistentState.Setup(s => s.GetUInt32($"PropertyState[" + assetId + "]"))
                .Returns((uint)PropertyStateType.Approved);

            // Get the Property State and validate
            uint state = saleDeedRegistry.GetPropertyState(assetId);
            Assert.True(state == (uint)PropertyStateType.Approved);
        }
    }
}
