using Moq;
using Stratis.SmartContracts;
using Stratis.SmartContracts.CLR;
using System;
using System.Reflection;
using Xunit;

namespace PassportApplicationContract.Tests
{
    public class PassportApplicationTests
    {
        private const string AppId = "882B9E48-F3D0-4E3C-92B7-AF1F3851335D";
        private const string Applicant = "0x0000000000000000000000000000000000000001";
        private const string Provider = "0x0000000000000000000000000000000000000002";
        private const string RefNumber = "123456";

        private static readonly Address ApplicantAddress = Applicant.HexToAddress();
        private static readonly Address ProviderAddress = Provider.HexToAddress();
        private readonly Mock<ISmartContractState> mockContractState;
        private readonly Mock<IPersistentState> mockPersistentState;
        private readonly Mock<IInternalTransactionExecutor> mockInternalExecutor;
        private readonly Mock<IContractLogger> mockContractLogger;

        public PassportApplicationTests()
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
        [InlineData(nameof(PassportApplication.State))]
        [InlineData(nameof(PassportApplication.AppId))]
        [InlineData(nameof(PassportApplication.Provider))]
        [InlineData(nameof(PassportApplication.ReferenceNumber))]
        [InlineData(nameof(PassportApplication.Applicant))]
        public void Property_Setter_Is_Private(string propertyName)
        {
            Type type = typeof(PassportApplication);

            PropertyInfo property = type.GetProperty(propertyName);

            Assert.True(property.SetMethod.IsPrivate);
        }

        [Fact]
        public void Constructor_Sets_Initial_Values()
        {
            this.mockContractState.Setup(s => s.Message.Sender).Returns(ApplicantAddress);

            var passportApplication = new PassportApplication(this.mockContractState.Object, AppId, ProviderAddress, RefNumber);

            this.mockPersistentState.Verify(s => s.SetString(nameof(PassportApplication.AppId), AppId));
            this.mockPersistentState.Verify(s => s.SetAddress(nameof(PassportApplication.Applicant), ApplicantAddress));
            this.mockPersistentState.Verify(s => s.SetAddress(nameof(PassportApplication.Provider), ProviderAddress));
            this.mockPersistentState.Verify(s => s.SetString(nameof(PassportApplication.ReferenceNumber), RefNumber));
            this.mockPersistentState.Verify(s => s.SetUInt32(nameof(PassportApplication.State), (uint)PassportApplication.StateType.MakeAppointment));
        }

        [Fact]
        public void Pay_Fails_WhenSenderIsNotApplicant()
        {
            // Setup incorrect sender.
            this.mockPersistentState.Setup(s => s.GetAddress(nameof(PassportApplication.Applicant))).Returns(ApplicantAddress);
            this.mockContractState.Setup(s => s.Message.Sender).Returns(Address.Zero);

            var contract = new PassportApplication(this.mockContractState.Object, AppId, ProviderAddress, RefNumber);

            // Setup set state.
            this.mockPersistentState.Setup(s => s.GetUInt32(nameof(PassportApplication.State))).Returns((uint)PassportApplication.StateType.MakeAppointment);

            // Attempt pay of wrong applicant should fail.
            Assert.Throws<SmartContractAssertException>(() => contract.Pay());
        }

        [Theory]
        [InlineData((uint)PassportApplication.StateType.ApprovedApplication)]
        [InlineData((uint)PassportApplication.StateType.RejectedApplication)]
        [InlineData((uint)PassportApplication.StateType.CancelledApplication)]
        [InlineData((uint)PassportApplication.StateType.PersonalAppearance)]
        public void Pay_Fails_WhenStateIsNotMakeAppointment(uint state)
        {
            // Setup correct applicant
            this.mockPersistentState.Setup(s => s.GetAddress(nameof(PassportApplication.Applicant))).Returns(ApplicantAddress);
            this.mockContractState.Setup(s => s.Message.Sender).Returns(ApplicantAddress);

            var contract = new PassportApplication(this.mockContractState.Object, AppId, ProviderAddress, RefNumber);

            // Setup wrong state.
            this.mockPersistentState.Setup(s => s.GetUInt32(nameof(PassportApplication.State))).Returns(state);

            // Attempt pay when state is not set to MakeAppointment should fail.
            Assert.Throws<SmartContractAssertException>(() => contract.Pay());
        }

        [Fact]
        public void Pay_Fails_WhenBalanceIsNotEnough()
        {
            // Setup incorrect sender.
            this.mockPersistentState.Setup(s => s.GetAddress(nameof(PassportApplication.Applicant))).Returns(ApplicantAddress);
            this.mockContractState.Setup(s => s.Message.Sender).Returns(ApplicantAddress);

            var contract = new PassportApplication(this.mockContractState.Object, AppId, ProviderAddress, RefNumber);

            // Setup set state.
            this.mockPersistentState.Setup(s => s.GetUInt32(nameof(PassportApplication.State))).Returns((uint)PassportApplication.StateType.MakeAppointment);

            // Setup insufficient balance
            this.mockPersistentState.Setup(s => s.GetUInt64(nameof(PassportApplication.Balance))).Returns(7000000000);

            // Attempt pay of insufficient balance should fail.
            Assert.Throws<SmartContractAssertException>(() => contract.Pay());
        }

        [Fact]
        public void Pay_Succeeds_WhenBalanceIsEnough()
        {
            // Setup incorrect sender.
            this.mockPersistentState.Setup(s => s.GetAddress(nameof(PassportApplication.Applicant))).Returns(ApplicantAddress);
            this.mockContractState.Setup(s => s.Message.Sender).Returns(ApplicantAddress);

            var contract = new PassportApplication(this.mockContractState.Object, AppId, ProviderAddress, RefNumber);

            // Setup set state.
            this.mockPersistentState.Setup(s => s.GetUInt32(nameof(PassportApplication.State))).Returns((uint)PassportApplication.StateType.MakeAppointment);

            // Setup sufficient balance
            this.mockPersistentState.Setup(s => s.GetUInt64(nameof(PassportApplication.Balance))).Returns(8000000000);

            contract.Pay();

            // Attempt pay of with sufficient balance should succeed.
            this.mockPersistentState.Verify(s => s.SetUInt32(nameof(PassportApplication.State), (uint)PassportApplication.StateType.PersonalAppearance));
        }

        [Fact]
        public void CancelApplication_Fails_WhenSenderIsNotApplicant()
        {
            // Setup incorrect sender.
            this.mockPersistentState.Setup(s => s.GetAddress(nameof(PassportApplication.Applicant))).Returns(ApplicantAddress);
            this.mockContractState.Setup(s => s.Message.Sender).Returns(Address.Zero);

            var contract = new PassportApplication(this.mockContractState.Object, AppId, ProviderAddress, RefNumber);

            // Setup set state.
            this.mockPersistentState.Setup(s => s.GetUInt32(nameof(PassportApplication.State))).Returns((uint)PassportApplication.StateType.MakeAppointment);

            // Attempt CancelApplication when sender is not applicant should fail.
            Assert.Throws<SmartContractAssertException>(() => contract.CancelApplication());
        }

        [Theory]
        [InlineData((uint)PassportApplication.StateType.ApprovedApplication)]
        [InlineData((uint)PassportApplication.StateType.RejectedApplication)]
        [InlineData((uint)PassportApplication.StateType.CancelledApplication)]
        public void CancelApplication_Fails_WhenStateIsNotMakeAppointmentOrIsNotPersonalAppearance(uint state)
        {
            // Setup incorrect sender.
            this.mockPersistentState.Setup(s => s.GetAddress(nameof(PassportApplication.Applicant))).Returns(ApplicantAddress);
            this.mockContractState.Setup(s => s.Message.Sender).Returns(ApplicantAddress);

            var contract = new PassportApplication(this.mockContractState.Object, AppId, ProviderAddress, RefNumber);

            // Setup set state.
            this.mockPersistentState.Setup(s => s.GetUInt32(nameof(PassportApplication.State))).Returns(state);

            // Attempt CancelApplication when state is not MakeAppointment or PersonalAppearance should fail.
            Assert.Throws<SmartContractAssertException>(() => contract.CancelApplication());
        }

        [Fact]
        public void CancelApplication_Succeeds_WhenStateIsMakeAppointment()
        {
            // Setup incorrect sender.
            this.mockPersistentState.Setup(s => s.GetAddress(nameof(PassportApplication.Applicant))).Returns(ApplicantAddress);
            this.mockContractState.Setup(s => s.Message.Sender).Returns(ApplicantAddress);

            var contract = new PassportApplication(this.mockContractState.Object, AppId, ProviderAddress, RefNumber);

            // Setup set state.
            this.mockPersistentState.Setup(s => s.GetUInt32(nameof(PassportApplication.State))).Returns((uint)PassportApplication.StateType.MakeAppointment);

            contract.CancelApplication();

            // Attempt CancelApplication when state is MakeAppointment should succeed.
            this.mockPersistentState.Verify(s => s.SetUInt32(nameof(PassportApplication.State), (uint)PassportApplication.StateType.CancelledApplication));
        }

        [Fact]
        public void CancelApplication_Succeeds_WhenStateIsPersonalAppearance()
        {
            // Setup incorrect sender.
            this.mockPersistentState.Setup(s => s.GetAddress(nameof(PassportApplication.Applicant))).Returns(ApplicantAddress);
            this.mockContractState.Setup(s => s.Message.Sender).Returns(ApplicantAddress);

            var contract = new PassportApplication(this.mockContractState.Object, AppId, ProviderAddress, RefNumber);

            // Setup set state.
            this.mockPersistentState.Setup(s => s.GetUInt32(nameof(PassportApplication.State))).Returns((uint)PassportApplication.StateType.PersonalAppearance);

            // Setup set balance.
            this.mockPersistentState.Setup(s => s.GetUInt64(nameof(PassportApplication.Balance))).Returns(8000000000);

            contract.CancelApplication();

            // Attempt CancelApplication when state is PersonalAppearance should succeed.
            this.mockPersistentState.Verify(s => s.SetUInt32(nameof(PassportApplication.State), (uint)PassportApplication.StateType.CancelledApplication));
        }

        [Fact]
        public void ApproveApplication_Fails_WhenSenderIsNotProvider()
        {
            // Setup incorrect sender.
            this.mockPersistentState.Setup(s => s.GetAddress(nameof(PassportApplication.Provider))).Returns(ProviderAddress);
            this.mockContractState.Setup(s => s.Message.Sender).Returns(Address.Zero);

            var contract = new PassportApplication(this.mockContractState.Object, AppId, ProviderAddress, RefNumber);

            // Setup set state.
            this.mockPersistentState.Setup(s => s.GetUInt32(nameof(PassportApplication.State))).Returns((uint)PassportApplication.StateType.PersonalAppearance);

            // Attempt ApproveApplication when sender is not the provider should fail.
            Assert.Throws<SmartContractAssertException>(() => contract.ApproveApplication());
        }

        [Theory]
        [InlineData((uint)PassportApplication.StateType.MakeAppointment)]
        [InlineData((uint)PassportApplication.StateType.ApprovedApplication)]
        [InlineData((uint)PassportApplication.StateType.RejectedApplication)]
        [InlineData((uint)PassportApplication.StateType.CancelledApplication)]
        public void ApproveApplication_Fails_WhenStateIsNotPersonalAppearance(uint state)
        {
            // Setup incorrect sender.
            this.mockPersistentState.Setup(s => s.GetAddress(nameof(PassportApplication.Applicant))).Returns(ApplicantAddress);
            this.mockContractState.Setup(s => s.Message.Sender).Returns(ApplicantAddress);

            var contract = new PassportApplication(this.mockContractState.Object, AppId, ProviderAddress, RefNumber);

            // Setup set state.
            this.mockPersistentState.Setup(s => s.GetUInt32(nameof(PassportApplication.State))).Returns(state);

            // Attempt ApproveApplication when state is not PersonalAppearance should fail.
            Assert.Throws<SmartContractAssertException>(() => contract.ApproveApplication());
        }

        [Fact]
        public void ApproveApplication_Succeeds_WhenSenderIsProviderAndStateIsPersonalAppearance()
        {
            // Setup incorrect sender.
            this.mockPersistentState.Setup(s => s.GetAddress(nameof(PassportApplication.Provider))).Returns(ProviderAddress);
            this.mockContractState.Setup(s => s.Message.Sender).Returns(ProviderAddress);

            var contract = new PassportApplication(this.mockContractState.Object, AppId, ProviderAddress, RefNumber);

            // Setup set state.
            this.mockPersistentState.Setup(s => s.GetUInt32(nameof(PassportApplication.State))).Returns((uint)PassportApplication.StateType.PersonalAppearance);

            // Attempt ApproveApplication when sender is the provider and the state is PersonalAppearance should succeed.

            contract.ApproveApplication();

            this.mockPersistentState.Verify(s => s.SetUInt32(nameof(PassportApplication.State), (uint)PassportApplication.StateType.ApprovedApplication));
        }

        [Fact]
        public void RejectApplication_Fails_WhenSenderIsNotProvider()
        {
            // Setup incorrect sender.
            this.mockPersistentState.Setup(s => s.GetAddress(nameof(PassportApplication.Provider))).Returns(ProviderAddress);
            this.mockContractState.Setup(s => s.Message.Sender).Returns(Address.Zero);

            var contract = new PassportApplication(this.mockContractState.Object, AppId, ProviderAddress, RefNumber);

            // Setup set state.
            this.mockPersistentState.Setup(s => s.GetUInt32(nameof(PassportApplication.State))).Returns((uint)PassportApplication.StateType.PersonalAppearance);

            // Attempt RejectApplication when sender is not the provider should fail.
            Assert.Throws<SmartContractAssertException>(() => contract.RejectApplication());
        }

        [Theory]
        [InlineData((uint)PassportApplication.StateType.MakeAppointment)]
        [InlineData((uint)PassportApplication.StateType.ApprovedApplication)]
        [InlineData((uint)PassportApplication.StateType.RejectedApplication)]
        [InlineData((uint)PassportApplication.StateType.CancelledApplication)]
        public void RejectApplication_Fails_WhenStateIsNotPersonalAppearance(uint state)
        {
            // Setup incorrect sender.
            this.mockPersistentState.Setup(s => s.GetAddress(nameof(PassportApplication.Applicant))).Returns(ApplicantAddress);
            this.mockContractState.Setup(s => s.Message.Sender).Returns(ApplicantAddress);

            var contract = new PassportApplication(this.mockContractState.Object, AppId, ProviderAddress, RefNumber);

            // Setup set state.
            this.mockPersistentState.Setup(s => s.GetUInt32(nameof(PassportApplication.State))).Returns(state);

            // Attempt RejectApplication when state is not PersonalAppearance should fail.
            Assert.Throws<SmartContractAssertException>(() => contract.RejectApplication());
        }

        [Fact]
        public void RejectApplication_Succeeds_WhenSenderIsProviderAndStateIsPersonalAppearance()
        {
            // Setup incorrect sender.
            this.mockPersistentState.Setup(s => s.GetAddress(nameof(PassportApplication.Provider))).Returns(ProviderAddress);
            this.mockContractState.Setup(s => s.Message.Sender).Returns(ProviderAddress);

            var contract = new PassportApplication(this.mockContractState.Object, AppId, ProviderAddress, RefNumber);

            // Setup set state.
            this.mockPersistentState.Setup(s => s.GetUInt32(nameof(PassportApplication.State))).Returns((uint)PassportApplication.StateType.PersonalAppearance);

            // Attempt RejectApplication when sender is the provider and the state is PersonalAppearance should succeed.
            contract.RejectApplication();

            this.mockPersistentState.Verify(s => s.SetUInt32(nameof(PassportApplication.State), (uint)PassportApplication.StateType.RejectedApplication));
        }

    }
}
