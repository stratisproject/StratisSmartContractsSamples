using Stratis.SmartContracts;

[Deploy]
public class PassportApplication : SmartContract
{
    private const ulong _SatsPerCrs = 100000000;
    private const ulong _passportApplicationCostInSats = 8000000000;
    private const ulong _passportApplicationPenaltyInSats = 800000000;

    public PassportApplication(ISmartContractState smartContractState, string appId, Address provider, string refNumber)
        : base(smartContractState)
    {
        Assert(appId == "882B9E48-F3D0-4E3C-92B7-AF1F3851335D");

        AppId = appId;
        Provider = provider;
        Applicant = Message.Sender;
        ReferenceNumber = refNumber;
        State = (uint)StateType.MakeAppointment;
    }

    public enum StateType : uint
    {
        MakeAppointment = 0,
        PersonalAppearance = 1,
        ApprovedApplication = 2,
        RejectedApplication = 3,
        CancelledApplication = 4
    }

    public uint State
    {
        get => this.PersistentState.GetUInt32(nameof(this.State));
        private set => this.PersistentState.SetUInt32(nameof(this.State), value);
    }

    public string ReferenceNumber
    {
        get => this.PersistentState.GetString(nameof(this.ReferenceNumber));
        private set => this.PersistentState.SetString(nameof(this.ReferenceNumber), value);
    }

    public string AppId
    {
        get => this.PersistentState.GetString(nameof(this.AppId));
        private set => this.PersistentState.SetString(nameof(this.AppId), value);
    }

    public Address Provider
    {
        get => this.PersistentState.GetAddress(nameof(this.Provider));
        private set => this.PersistentState.SetAddress(nameof(this.Provider), value);
    }

    public Address Applicant
    {
        get => this.PersistentState.GetAddress(nameof(this.Applicant));
        private set => this.PersistentState.SetAddress(nameof(this.Applicant), value);
    }

    private ulong ConvertAmountInCrs(ulong amount)
    {
        return (amount / _SatsPerCrs);
    }

    private ulong ConvertAmountInSats(ulong amount)
    {
        return amount * _SatsPerCrs;
    }

    public ulong CurrentBalance
    {
        get => this.PersistentState.GetUInt64(nameof(this.Balance));
    }

    public ulong PassportApplicationCost
    {
        get => ConvertAmountInCrs(_passportApplicationCostInSats);
    }

    public void Pay()
    {
        Assert(this.State == (uint)StateType.MakeAppointment);
        Assert(this.Message.Sender == Applicant);

        if(CurrentBalance < _passportApplicationCostInSats)
        {
            Transfer(Applicant, CurrentBalance);
            Assert(CurrentBalance >= _passportApplicationCostInSats, "Not enough funds.");
        }

        State = (uint)StateType.PersonalAppearance;
    }

    public void CancelApplication()
    {
        Assert(this.State == (uint)StateType.PersonalAppearance || this.State == (uint)StateType.MakeAppointment);
        Assert(this.Message.Sender == Applicant);

        if (this.State == (uint)StateType.PersonalAppearance)
        {
            var newBalanceAfterPenalty = CurrentBalance - _passportApplicationPenaltyInSats;
            Transfer(Applicant, newBalanceAfterPenalty);
            Transfer(Provider, _passportApplicationPenaltyInSats);
        }

        State = (uint)StateType.CancelledApplication;
    }

    public void RejectApplication()
    {
        Assert(this.State == (uint)StateType.PersonalAppearance);
        Assert(this.Message.Sender == Provider);

        if (this.State == (uint)StateType.PersonalAppearance)
        {
            var newBalanceAfterPenalty = CurrentBalance - _passportApplicationPenaltyInSats;
            Transfer(Applicant, newBalanceAfterPenalty);
            Transfer(Provider, _passportApplicationPenaltyInSats);
        }

        State = (uint)StateType.RejectedApplication;
    }

    public void ApproveApplication()
    {
        Assert(this.State == (uint)StateType.PersonalAppearance);
        Assert(this.Message.Sender == Provider);

        Transfer(Provider, CurrentBalance);

        State = (uint)StateType.ApprovedApplication;
    }
}

