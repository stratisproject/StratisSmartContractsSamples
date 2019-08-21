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
        private const string NewBankAgent = "0x0000000000000000000000000000000000000003";
        private const string ThirdPartyRequestor = "0x0000000000000000000000000000000000000004";
        private const string CurrentAuthorizedUser = "0x0000000000000000000000000000000000000005";

        private static readonly Address OwnerAddress = Owner.HexToAddress();
        private static readonly Address BankAgentAddress = BankAgent.HexToAddress();
        private static readonly Address NewBankAgentAddress = NewBankAgent.HexToAddress();
        private static readonly Address ThirdPartyRequestorAddress = ThirdPartyRequestor.HexToAddress();
        private static readonly Address CurrentAuthorizedUserAddress = CurrentAuthorizedUser.HexToAddress();

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

        [Fact]
        public void BeginReviewProcess_Caller_Must_Not_Be_Owner()
        {
            var digitalLocker = this.NewDigitalLocker();

            // Sender is the owner.
            this.mockContractState.Setup(s => s.Message.Sender).Returns(OwnerAddress);

            Assert.Throws<SmartContractAssertException>(() => digitalLocker.BeginReviewProcess());

            this.mockPersistentState.Verify(s => s.SetAddress(nameof(DigitalLocker.BankAgent), NewBankAgentAddress));
            this.mockPersistentState.Verify(s => s.SetString(nameof(DigitalLocker.LockerStatus), DigitalLocker.Pending));
            this.mockPersistentState.Verify(s => s.SetUInt32(nameof(DigitalLocker.State), (uint)DigitalLocker.StateType.DocumentReview));
        }

        [Fact]
        public void BeginReviewProcess_Success()
        {
            var digitalLocker = this.NewDigitalLocker();

            // Sender is not the owner.
            this.mockContractState.Setup(s => s.Message.Sender).Returns(NewBankAgentAddress);

            digitalLocker.BeginReviewProcess();

            this.mockPersistentState.Verify(s => s.SetAddress(nameof(DigitalLocker.BankAgent), It.IsAny<Address>()), Times.Never);
            this.mockPersistentState.Verify(s => s.SetString(nameof(DigitalLocker.LockerStatus), It.IsAny<string>()), Times.Never);
            this.mockPersistentState.Verify(s => s.SetUInt32(nameof(DigitalLocker.State), It.IsAny<uint>()), Times.Never);
        }

        [Fact]
        public void RejectApplication_Caller_Must_Be_BankAgent()
        {
            var digitalLocker = this.NewDigitalLocker();

            // Sender is not the bank agent.
            this.mockContractState.Setup(s => s.Message.Sender).Returns(Address.Zero);

            Assert.Throws<SmartContractAssertException>(() => digitalLocker.RejectApplication(string.Empty));

            this.mockPersistentState.Verify(s => s.SetString(nameof(DigitalLocker.RejectionReason), It.IsAny<string>()), Times.Never);
            this.mockPersistentState.Verify(s => s.SetString(nameof(DigitalLocker.LockerStatus), It.IsAny<string>()), Times.Never);
            this.mockPersistentState.Verify(s => s.SetUInt32(nameof(DigitalLocker.State), It.IsAny<uint>()), Times.Never);
        }

        [Theory]
        [InlineData("")]
        [InlineData("Test Rejection Reason")]
        public void RejectApplication_Success(string rejectionReason)
        {
            var digitalLocker = this.NewDigitalLocker();

            // Sender is the bank agent.
            this.mockContractState.Setup(s => s.Message.Sender).Returns(BankAgentAddress);

            digitalLocker.RejectApplication(rejectionReason);

            this.mockPersistentState.Verify(s => s.SetString(nameof(DigitalLocker.RejectionReason), rejectionReason));
            this.mockPersistentState.Verify(s => s.SetString(nameof(DigitalLocker.LockerStatus), DigitalLocker.Rejected));
            this.mockPersistentState.Verify(s => s.SetUInt32(nameof(DigitalLocker.State), (uint)DigitalLocker.StateType.DocumentReview));
        }

        [Fact]
        public void UploadDocuments_Caller_Must_Be_BankAgent()
        {
            var digitalLocker = this.NewDigitalLocker();

            // Sender is not the bank agent.
            this.mockContractState.Setup(s => s.Message.Sender).Returns(Address.Zero);

            Assert.Throws<SmartContractAssertException>(() => digitalLocker.UploadDocuments(string.Empty, string.Empty));

            this.mockPersistentState.Verify(s => s.SetString(nameof(DigitalLocker.LockerIdentifier), It.IsAny<string>()), Times.Never);
            this.mockPersistentState.Verify(s => s.SetString(nameof(DigitalLocker.Image), It.IsAny<string>()), Times.Never);
            this.mockPersistentState.Verify(s => s.SetString(nameof(DigitalLocker.LockerStatus), It.IsAny<string>()), Times.Never);
            this.mockPersistentState.Verify(s => s.SetUInt32(nameof(DigitalLocker.State), It.IsAny<uint>()), Times.Never);
        }

        [Theory]
        [InlineData("", "")]
        [InlineData("Test Locker Identifier", "Test Image")]
        public void UploadDocument_Success(string lockerIdentifier, string image)
        {
            var digitalLocker = this.NewDigitalLocker();

            // Sender is the bank agent.
            this.mockContractState.Setup(s => s.Message.Sender).Returns(BankAgentAddress);

            digitalLocker.UploadDocuments(lockerIdentifier, image);

            this.mockPersistentState.Verify(s => s.SetString(nameof(DigitalLocker.LockerIdentifier), lockerIdentifier));
            this.mockPersistentState.Verify(s => s.SetString(nameof(DigitalLocker.Image), image));
            this.mockPersistentState.Verify(s => s.SetString(nameof(DigitalLocker.LockerStatus), DigitalLocker.Approved));
            this.mockPersistentState.Verify(s => s.SetUInt32(nameof(DigitalLocker.State), (uint)DigitalLocker.StateType.AvailableToShare));
        }

        [Fact]
        public void ShareWithThirdParty_Caller_Must_Be_Owner()
        {
            var digitalLocker = this.NewDigitalLocker();

            // Sender is not the owner.
            this.mockContractState.Setup(s => s.Message.Sender).Returns(Address.Zero);

            Assert.Throws<SmartContractAssertException>(() => digitalLocker.ShareWithThirdParty(Address.Zero, string.Empty, string.Empty));

            this.mockPersistentState.Verify(s => s.SetAddress(nameof(DigitalLocker.ThirdPartyRequestor), It.IsAny<Address>()), Times.Never);
            this.mockPersistentState.Verify(s => s.SetAddress(nameof(DigitalLocker.CurrentAuthorizedUser), It.IsAny<Address>()), Times.Never);
            this.mockPersistentState.Verify(s => s.SetString(nameof(DigitalLocker.LockerStatus), It.IsAny<string>()), Times.Never);
            this.mockPersistentState.Verify(s => s.SetString(nameof(DigitalLocker.IntendedPurpose), It.IsAny<string>()), Times.Never);
            this.mockPersistentState.Verify(s => s.SetString(nameof(DigitalLocker.ExpirationDate), It.IsAny<string>()), Times.Never);
            this.mockPersistentState.Verify(s => s.SetUInt32(nameof(DigitalLocker.State), It.IsAny<uint>()), Times.Never);
        }

        [Theory]
        [InlineData("0x0000000000000000000000000000000000000000", "", "")]
        // TODO
        public void ShareWithThirdParty_Success(string thirdPartyRequestor, string expirationDate, string intendedPurpose)
        {
            var digitalLocker = this.NewDigitalLocker();

            // Sender is not the owner.
            this.mockContractState.Setup(s => s.Message.Sender).Returns(OwnerAddress);

            var thirdPartyRequestorAddress = thirdPartyRequestor.HexToAddress();

            digitalLocker.ShareWithThirdParty(thirdPartyRequestorAddress, expirationDate, intendedPurpose);

            this.mockPersistentState.Verify(s => s.SetAddress(nameof(DigitalLocker.ThirdPartyRequestor), thirdPartyRequestorAddress));
            this.mockPersistentState.Verify(s => s.SetAddress(nameof(DigitalLocker.CurrentAuthorizedUser), thirdPartyRequestorAddress));
            this.mockPersistentState.Verify(s => s.SetString(nameof(DigitalLocker.LockerStatus), DigitalLocker.Shared));
            this.mockPersistentState.Verify(s => s.SetString(nameof(DigitalLocker.IntendedPurpose), intendedPurpose));
            this.mockPersistentState.Verify(s => s.SetString(nameof(DigitalLocker.ExpirationDate), expirationDate));
            this.mockPersistentState.Verify(s => s.SetUInt32(nameof(DigitalLocker.State), (uint)DigitalLocker.StateType.SharingWithThirdParty));
        }

        [Fact]
        public void AcceptSharingRequest_Caller_Must_Be_Owner()
        {
            var digitalLocker = this.NewDigitalLocker();

            // Sender is not the owner.
            this.mockContractState.Setup(s => s.Message.Sender).Returns(Address.Zero);

            Assert.Throws<SmartContractAssertException>(() => digitalLocker.AcceptSharingRequest());

            this.mockPersistentState.Verify(s => s.SetAddress(nameof(DigitalLocker.CurrentAuthorizedUser), It.IsAny<Address>()), Times.Never);
            this.mockPersistentState.Verify(s => s.SetUInt32(nameof(DigitalLocker.State), It.IsAny<uint>()), Times.Never);
        }

        [Fact]
        public void AcceptSharingRequest_Success()
        {
            var digitalLocker = this.NewDigitalLocker();

            // Sender is the owner.
            this.mockContractState.Setup(s => s.Message.Sender).Returns(OwnerAddress);

            // Set up the third party requestor.
            this.mockPersistentState.Setup(s => s.GetAddress(nameof(DigitalLocker.ThirdPartyRequestor))).Returns(ThirdPartyRequestorAddress);

            digitalLocker.AcceptSharingRequest();

            this.mockPersistentState.Verify(s => s.SetAddress(nameof(DigitalLocker.CurrentAuthorizedUser), ThirdPartyRequestorAddress));
            this.mockPersistentState.Verify(s => s.SetUInt32(nameof(DigitalLocker.State), (uint)DigitalLocker.StateType.SharingWithThirdParty));
        }

        [Fact]
        public void RejectSharingRequest_Caller_Must_Be_Owner()
        {
            var digitalLocker = this.NewDigitalLocker();

            // Sender is not the owner.
            this.mockContractState.Setup(s => s.Message.Sender).Returns(Address.Zero);

            Assert.Throws<SmartContractAssertException>(() => digitalLocker.RejectSharingRequest());

            this.mockPersistentState.Verify(s => s.SetString(nameof(DigitalLocker.LockerStatus), It.IsAny<string>()), Times.Never);
            this.mockPersistentState.Verify(s => s.SetAddress(nameof(DigitalLocker.CurrentAuthorizedUser), It.IsAny<Address>()), Times.Never);
            this.mockPersistentState.Verify(s => s.SetUInt32(nameof(DigitalLocker.State), It.IsAny<uint>()), Times.Never);
        }

        [Fact]
        public void RejectSharingRequest_Success()
        {
            var digitalLocker = this.NewDigitalLocker();

            // Sender is the owner.
            this.mockContractState.Setup(s => s.Message.Sender).Returns(OwnerAddress);

            digitalLocker.AcceptSharingRequest();

            this.mockPersistentState.Verify(s => s.SetString(nameof(DigitalLocker.LockerStatus), DigitalLocker.Available));
            this.mockPersistentState.Verify(s => s.SetAddress(nameof(DigitalLocker.CurrentAuthorizedUser), Address.Zero));
            this.mockPersistentState.Verify(s => s.SetUInt32(nameof(DigitalLocker.State), (uint)DigitalLocker.StateType.AvailableToShare));
        }

        [Fact]
        public void RequestLockerAccess_Caller_Must_Be_Owner()
        {
            var digitalLocker = this.NewDigitalLocker();

            // Sender is not the owner.
            this.mockContractState.Setup(s => s.Message.Sender).Returns(Address.Zero);

            Assert.Throws<SmartContractAssertException>(() => digitalLocker.RequestLockerAccess(""));

            this.mockPersistentState.Verify(s => s.SetAddress(nameof(DigitalLocker.ThirdPartyRequestor), It.IsAny<Address>()), Times.Never);
            this.mockPersistentState.Verify(s => s.SetString(nameof(DigitalLocker.IntendedPurpose), It.IsAny<string>()), Times.Never);
            this.mockPersistentState.Verify(s => s.SetUInt32(nameof(DigitalLocker.State), It.IsAny<uint>()), Times.Never);
        }

        [Fact]
        public void RequestLockerAccess_Success()
        {
            var digitalLocker = this.NewDigitalLocker();

            // Sender is the owner.
            this.mockContractState.Setup(s => s.Message.Sender).Returns(OwnerAddress);

            var purpose = "Test purpose";

            digitalLocker.RequestLockerAccess(purpose);

            this.mockPersistentState.Verify(s => s.SetAddress(nameof(DigitalLocker.ThirdPartyRequestor), OwnerAddress));
            this.mockPersistentState.Verify(s => s.SetString(nameof(DigitalLocker.IntendedPurpose), purpose));
            this.mockPersistentState.Verify(s => s.SetUInt32(nameof(DigitalLocker.State), (uint)DigitalLocker.StateType.SharingRequestPending));
        }

        [Fact]
        public void ReleaseLockerAccess_Caller_Must_Be_CurrentAuthorizedUser()
        {
            var digitalLocker = this.NewDigitalLocker();

            // Set up the current authorized user.
            this.mockPersistentState.Setup(s => s.GetAddress(nameof(DigitalLocker.CurrentAuthorizedUser))).Returns(CurrentAuthorizedUserAddress);

            // Sender is not the current authorized user.
            this.mockContractState.Setup(s => s.Message.Sender).Returns(Address.Zero);

            Assert.Throws<SmartContractAssertException>(() => digitalLocker.ReleaseLockerAccess());

            this.mockPersistentState.Verify(s => s.SetString(nameof(DigitalLocker.LockerStatus), It.IsAny<string>()), Times.Never);
            this.mockPersistentState.Verify(s => s.SetAddress(nameof(DigitalLocker.ThirdPartyRequestor), It.IsAny<Address>()), Times.Never);
            this.mockPersistentState.Verify(s => s.SetAddress(nameof(DigitalLocker.CurrentAuthorizedUser), It.IsAny<Address>()), Times.Never);
            this.mockPersistentState.Verify(s => s.SetString(nameof(DigitalLocker.IntendedPurpose), It.IsAny<string>()), Times.Never);
            this.mockPersistentState.Verify(s => s.SetUInt32(nameof(DigitalLocker.State), It.IsAny<uint>()), Times.Never);
        }

        [Fact]
        public void ReleaseLockerAccess_Success()
        {
            var digitalLocker = this.NewDigitalLocker();

            // Set up the current authorized user.
            this.mockPersistentState.Setup(s => s.GetAddress(nameof(DigitalLocker.CurrentAuthorizedUser))).Returns(CurrentAuthorizedUserAddress);

            // Sender is the current authorized user.
            this.mockContractState.Setup(s => s.Message.Sender).Returns(CurrentAuthorizedUserAddress);

            digitalLocker.ReleaseLockerAccess();

            this.mockPersistentState.Verify(s => s.SetString(nameof(DigitalLocker.LockerStatus), DigitalLocker.Available));
            this.mockPersistentState.Verify(s => s.SetAddress(nameof(DigitalLocker.ThirdPartyRequestor), Address.Zero));
            this.mockPersistentState.Verify(s => s.SetAddress(nameof(DigitalLocker.CurrentAuthorizedUser), Address.Zero));
            this.mockPersistentState.Verify(s => s.SetString(nameof(DigitalLocker.IntendedPurpose), string.Empty));
            this.mockPersistentState.Verify(s => s.SetUInt32(nameof(DigitalLocker.State), (uint)DigitalLocker.StateType.AvailableToShare));
        }

        [Fact]
        public void RevokeAccessFromThirdParty_Caller_Must_Be_Owner()
        {
            var digitalLocker = this.NewDigitalLocker();

            // Sender is not the owner.
            this.mockContractState.Setup(s => s.Message.Sender).Returns(Address.Zero);

            Assert.Throws<SmartContractAssertException>(() => digitalLocker.RevokeAccessFromThirdParty());

            this.mockPersistentState.Verify(s => s.SetString(nameof(DigitalLocker.LockerStatus), It.IsAny<string>()), Times.Never);
            this.mockPersistentState.Verify(s => s.SetAddress(nameof(DigitalLocker.CurrentAuthorizedUser), It.IsAny<Address>()), Times.Never);
            this.mockPersistentState.Verify(s => s.SetUInt32(nameof(DigitalLocker.State), It.IsAny<uint>()), Times.Never);
        }

        [Fact]
        public void RevokeAccessFromThirdParty_Success()
        {
            var digitalLocker = this.NewDigitalLocker();

            // Sender is the owner.
            this.mockContractState.Setup(s => s.Message.Sender).Returns(OwnerAddress);

            digitalLocker.RevokeAccessFromThirdParty();

            this.mockPersistentState.Verify(s => s.SetString(nameof(DigitalLocker.LockerStatus), DigitalLocker.Available));
            this.mockPersistentState.Verify(s => s.SetAddress(nameof(DigitalLocker.CurrentAuthorizedUser), Address.Zero));
            this.mockPersistentState.Verify(s => s.SetUInt32(nameof(DigitalLocker.State), (uint)DigitalLocker.StateType.AvailableToShare));
        }

        [Fact]
        public void Terminate_Caller_Must_Be_Owner()
        {
            var digitalLocker = this.NewDigitalLocker();

            // Sender is not the owner.
            this.mockContractState.Setup(s => s.Message.Sender).Returns(Address.Zero);

            Assert.Throws<SmartContractAssertException>(() => digitalLocker.Terminate());

            this.mockPersistentState.Verify(s => s.SetAddress(nameof(DigitalLocker.CurrentAuthorizedUser), It.IsAny<Address>()), Times.Never);
            this.mockPersistentState.Verify(s => s.SetUInt32(nameof(DigitalLocker.State), It.IsAny<uint>()), Times.Never);
        }

        [Fact]
        public void Terminate_Success()
        {
            var digitalLocker = this.NewDigitalLocker();

            // Sender is the owner.
            this.mockContractState.Setup(s => s.Message.Sender).Returns(OwnerAddress);

            digitalLocker.Terminate();

            this.mockPersistentState.Verify(s => s.SetAddress(nameof(DigitalLocker.CurrentAuthorizedUser), Address.Zero));
            this.mockPersistentState.Verify(s => s.SetUInt32(nameof(DigitalLocker.State), (uint)DigitalLocker.StateType.Terminated));
        }

        /// <summary>
        /// Sets up a new digital locker with an expected state.
        /// </summary>
        /// <returns></returns>
        private DigitalLocker NewDigitalLocker()
        {
            this.mockContractState.Setup(s => s.Message.Sender).Returns(OwnerAddress);

            this.mockPersistentState.Setup(s => s.GetAddress(nameof(DigitalLocker.Owner))).Returns(OwnerAddress);
            this.mockPersistentState.Setup(s => s.GetString(nameof(DigitalLocker.LockerFriendlyName))).Returns("Test");
            this.mockPersistentState.Setup(s => s.GetUInt32(nameof(DigitalLocker.State))).Returns((uint)DigitalLocker.StateType.DocumentReview);
            this.mockPersistentState.Setup(s => s.GetAddress(nameof(DigitalLocker.BankAgent))).Returns(BankAgentAddress);

            this.mockPersistentState.Invocations.Clear();

            return new DigitalLocker(this.mockContractState.Object, "Test", BankAgentAddress);
        }
    }
}
