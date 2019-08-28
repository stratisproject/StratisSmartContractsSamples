using Stratis.SmartContracts;

public class ThermostatContract : SmartContract
{
    public ThermostatContract(ISmartContractState smartContractState, Address user, int targetTemperature)
    : base(smartContractState)
    {
        this.Installer = Message.Sender;
        this.User = user;
        this.TargetTemperature = targetTemperature;
    }

    public enum StateType : uint
    {
        Created = 0,
        InUse = 1
    }

    public enum ModeType : uint
    {
        Off = 0,
        Cool = 1,
        Heat = 2,
        Auto = 3
    }

    public Address Installer
    {
        get => this.PersistentState.GetAddress(nameof(this.Installer));
        private set => this.PersistentState.SetAddress(nameof(this.Installer), value);
    }

    public Address User
    {
        get => this.PersistentState.GetAddress(nameof(this.User));
        private set => this.PersistentState.SetAddress(nameof(this.User), value);
    }

    public int TargetTemperature
    {
        get => this.PersistentState.GetInt32(nameof(this.TargetTemperature));
        private set => this.PersistentState.SetInt32(nameof(this.TargetTemperature), value);
    }

    public uint State
    {
        get => this.PersistentState.GetUInt32(nameof(this.State));
        private set => this.PersistentState.SetUInt32(nameof(this.State), value);
    }

    public uint Mode
    {
        get => this.PersistentState.GetUInt32(nameof(this.Mode));
        private set => this.PersistentState.SetUInt32(nameof(this.Mode), value);
    }

    public void StartThermostat()
    {
        this.Assert(Message.Sender == this.Installer);
        this.Assert(this.State == (uint)StateType.Created);

        State = (uint)StateType.InUse;
    }

    public void SetTargetTemperature(int target)
    {
        this.Assert(Message.Sender == this.User);
        this.Assert(this.State == (uint)StateType.InUse);

        TargetTemperature = target;
    }

    public void SetMode(uint mode)
    {
        this.Assert(Message.Sender == this.User);
        this.Assert(this.State == (uint)StateType.InUse);
        this.Assert(mode < 4);

        Mode = mode;
    }
}