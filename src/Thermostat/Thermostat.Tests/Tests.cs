using Moq;
using Stratis.SmartContracts;
using Stratis.SmartContracts.CLR;
using System;
using System.Reflection;
using Xunit;

namespace Thermostat.Tests
{
    public class Tests
    {
        private const int TargetTemperature = 20;
        private readonly Mock<ISmartContractState> mockContractState;
        private readonly Mock<IPersistentState> mockPersistentState;
        private readonly Mock<IContractLogger> mockContractLogger;
        private Address installer;
        private Address user;

        public Tests()
        {
            this.mockContractLogger = new Mock<IContractLogger>();
            this.mockPersistentState = new Mock<IPersistentState>();
            this.mockContractState = new Mock<ISmartContractState>();
            this.mockContractState.Setup(s => s.PersistentState).Returns(this.mockPersistentState.Object);
            this.mockContractState.Setup(s => s.ContractLogger).Returns(this.mockContractLogger.Object);
            this.installer = "0x0000000000000000000000000000000000000001".HexToAddress();
            this.user = "0x0000000000000000000000000000000000000002".HexToAddress();
        }

        [Fact]
        public void Constructor_Sets_Initial_Values()
        {
            ThermostatContract thermostat = this.NewThermostat();

            this.mockPersistentState.Verify(s => s.SetAddress(nameof(ThermostatContract.Installer), this.installer));
            this.mockPersistentState.Verify(s => s.SetAddress(nameof(ThermostatContract.User), this.user));
            this.mockPersistentState.Verify(s => s.SetInt32(nameof(ThermostatContract.TargetTemperature), TargetTemperature));
        }

        [Theory]
        [InlineData(nameof(ThermostatContract.Installer))]
        [InlineData(nameof(ThermostatContract.User))]
        [InlineData(nameof(ThermostatContract.TargetTemperature))]
        [InlineData(nameof(ThermostatContract.State))]
        [InlineData(nameof(ThermostatContract.Mode))]
        public void Property_Setter_Is_Private(string propertyName)
        {
            Type type = typeof(ThermostatContract);

            PropertyInfo property = type.GetProperty(propertyName);

            Assert.True(property.SetMethod.IsPrivate);
        }

        [Fact]
        public void StartThermostat_Throws_If_Caller_Not_Installer()
        {
            ThermostatContract thermostat = this.NewThermostat();

            this.mockPersistentState.Setup(s => s.GetAddress(nameof(ThermostatContract.Installer))).Returns(this.installer);

            this.mockContractState.Setup(s => s.Message.Sender).Returns(Address.Zero);

            Assert.Throws<SmartContractAssertException>(() => thermostat.StartThermostat());
        }

        [Fact]
        public void StartThermostat_Throws_If_Caller_Installer_But_State_Not_Created()
        {
            ThermostatContract thermostat = this.NewThermostat();

            this.mockPersistentState.Setup(s => s.GetUInt32(nameof(ThermostatContract.State))).Returns((uint)ThermostatContract.StateType.InUse);

            this.mockPersistentState.Setup(s => s.GetAddress(nameof(ThermostatContract.Installer))).Returns(this.installer);

            this.mockContractState.Setup(s => s.Message.Sender).Returns(this.installer);

            Assert.Throws<SmartContractAssertException>(() => thermostat.StartThermostat());
        }

        [Fact]
        public void StartThermostat_Sets_State_InUse()
        {
            ThermostatContract thermostat = this.NewThermostat();

            this.mockPersistentState.Setup(s => s.GetUInt32(nameof(ThermostatContract.State))).Returns((uint)ThermostatContract.StateType.Created);

            this.mockPersistentState.Setup(s => s.GetAddress(nameof(ThermostatContract.Installer))).Returns(this.installer);

            this.mockContractState.Setup(s => s.Message.Sender).Returns(this.installer);

            thermostat.StartThermostat();

            this.mockPersistentState.Verify(s => s.SetUInt32(nameof(ThermostatContract.State), (uint)ThermostatContract.StateType.InUse));
        }

        [Fact]
        public void SetTargetTemperature_Throws_If_Caller_Not_User()
        {
            ThermostatContract thermostat = this.NewThermostat();

            this.mockPersistentState.Setup(s => s.GetAddress(nameof(ThermostatContract.User))).Returns(this.user);

            this.mockContractState.Setup(s => s.Message.Sender).Returns(Address.Zero);

            Assert.Throws<SmartContractAssertException>(() => thermostat.SetTargetTemperature(TargetTemperature - 1));
        }

        [Fact]
        public void SetTargetTemperature_Throws_If_Caller_User_But_State_Not_InUse()
        {
            ThermostatContract thermostat = this.NewThermostat();

            this.mockPersistentState.Setup(s => s.GetUInt32(nameof(ThermostatContract.State))).Returns((uint)ThermostatContract.StateType.Created);

            this.mockPersistentState.Setup(s => s.GetAddress(nameof(ThermostatContract.User))).Returns(this.user);

            this.mockContractState.Setup(s => s.Message.Sender).Returns(this.user);

            Assert.Throws<SmartContractAssertException>(() => thermostat.SetTargetTemperature(TargetTemperature - 1));
        }

        [Theory]
        [InlineData(int.MinValue)]
        [InlineData(0)]
        [InlineData(int.MaxValue)]
        public void SetTargetTemperature_Sets_TargetTemperature(int targetTemperature)
        {
            ThermostatContract thermostat = this.NewThermostat();

            this.mockPersistentState.Setup(s => s.GetUInt32(nameof(ThermostatContract.State))).Returns((uint)ThermostatContract.StateType.InUse);

            this.mockPersistentState.Setup(s => s.GetAddress(nameof(ThermostatContract.User))).Returns(this.user);

            this.mockContractState.Setup(s => s.Message.Sender).Returns(this.user);

            thermostat.SetTargetTemperature(targetTemperature);

            this.mockPersistentState.Verify(s => s.SetInt32(nameof(ThermostatContract.TargetTemperature), targetTemperature));
        }

        [Fact]
        public void SetMode_Throws_If_Caller_Not_User()
        {
            ThermostatContract thermostat = this.NewThermostat();

            this.mockPersistentState.Setup(s => s.GetAddress(nameof(ThermostatContract.User))).Returns(this.user);

            this.mockContractState.Setup(s => s.Message.Sender).Returns(Address.Zero);

            Assert.Throws<SmartContractAssertException>(() => thermostat.SetMode((uint)ThermostatContract.ModeType.Auto));
        }

        [Fact]
        public void SetMode_Throws_If_Caller_User_But_State_Not_InUse()
        {
            ThermostatContract thermostat = this.NewThermostat();

            this.mockPersistentState.Setup(s => s.GetUInt32(nameof(ThermostatContract.State))).Returns((uint)ThermostatContract.StateType.Created);

            this.mockPersistentState.Setup(s => s.GetAddress(nameof(ThermostatContract.User))).Returns(this.user);

            this.mockContractState.Setup(s => s.Message.Sender).Returns(this.user);

            Assert.Throws<SmartContractAssertException>(() => thermostat.SetMode((uint)ThermostatContract.ModeType.Auto));
        }

        [Theory]
        [InlineData((uint)ThermostatContract.ModeType.Off)]
        [InlineData((uint)ThermostatContract.ModeType.Cool)]
        [InlineData((uint)ThermostatContract.ModeType.Heat)]
        [InlineData((uint)ThermostatContract.ModeType.Auto)]
        public void SetMode_Sets_Mode(uint mode)
        {
            ThermostatContract thermostat = this.NewThermostat();

            this.mockPersistentState.Setup(s => s.GetUInt32(nameof(ThermostatContract.State))).Returns((uint)ThermostatContract.StateType.InUse);

            this.mockPersistentState.Setup(s => s.GetAddress(nameof(ThermostatContract.User))).Returns(this.user);

            this.mockContractState.Setup(s => s.Message.Sender).Returns(this.user);

            thermostat.SetMode(mode);

            this.mockPersistentState.Verify(s => s.SetUInt32(nameof(ThermostatContract.Mode), mode));
        }

        [Theory]
        [InlineData((uint)ThermostatContract.ModeType.Auto + 1)]
        public void SetMode_Invalid_Does_Not_Set_Mode(uint mode)
        {
            ThermostatContract thermostat = this.NewThermostat();

            this.mockPersistentState.Setup(s => s.GetUInt32(nameof(ThermostatContract.State))).Returns((uint)ThermostatContract.StateType.InUse);

            this.mockPersistentState.Setup(s => s.GetAddress(nameof(ThermostatContract.User))).Returns(this.user);

            this.mockContractState.Setup(s => s.Message.Sender).Returns(this.user);

            Assert.Throws<SmartContractAssertException>(() => thermostat.SetMode(mode));
        }

        private ThermostatContract NewThermostat()
        {
            this.mockContractState.Setup(s => s.Message.Sender).Returns(this.installer);

            return new ThermostatContract(this.mockContractState.Object, this.user, TargetTemperature);
        }
    }
}
