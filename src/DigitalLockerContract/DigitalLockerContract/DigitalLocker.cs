using Stratis.SmartContracts;

public class DigitalLocker : SmartContract
{
    public DigitalLocker(ISmartContractState state, string lockerFriendlyName, Address bankAgent) 
        : base(state)
    {
        this.Owner = Message.Sender;
        this.LockerFriendlyName = lockerFriendlyName;
        this.BankAgent = bankAgent;
        this.State = (uint)StateType.DocumentReview;
    }

    public enum StateType : uint
    {
        Requested = 0,
        DocumentReview = 1,
        AvailableToShare = 2,
        SharingRequestPending = 3,
        SharingWithThirdParty = 4,
        Terminated = 5
    }

    public Address Owner
    {
        get => this.PersistentState.GetAddress(nameof(this.Owner));
        private set => this.PersistentState.SetAddress(nameof(this.Owner), value);
    }

    public Address BankAgent
    {
        get => this.PersistentState.GetAddress(nameof(this.BankAgent));
        private set => this.PersistentState.SetAddress(nameof(this.BankAgent), value);
    }

    public string LockerFriendlyName
    {
        get => this.PersistentState.GetString(nameof(this.LockerFriendlyName));
        private set => this.PersistentState.SetString(nameof(this.LockerFriendlyName), value);
    }

    public string LockerIdentifier
    {
        get => this.PersistentState.GetString(nameof(this.LockerIdentifier));
        private set => this.PersistentState.SetString(nameof(this.LockerIdentifier), value);
    }

    public Address CurrentAuthorizedUser
    {
        get => this.PersistentState.GetAddress(nameof(this.CurrentAuthorizedUser));
        private set => this.PersistentState.SetAddress(nameof(this.CurrentAuthorizedUser), value);
    }

    public string ExpirationDate
    {
        get => this.PersistentState.GetString(nameof(this.ExpirationDate));
        private set => this.PersistentState.SetString(nameof(this.ExpirationDate), value);
    }

    public string Image
    {
        get => this.PersistentState.GetString(nameof(this.Image));
        private set => this.PersistentState.SetString(nameof(this.Image), value);
    }

    public Address ThirdPartyRequestor
    {
        get => this.PersistentState.GetAddress(nameof(this.ThirdPartyRequestor));
        private set => this.PersistentState.SetAddress(nameof(this.ThirdPartyRequestor), value);
    }

    public string IntendedPurpose
    {
        get => this.PersistentState.GetString(nameof(this.IntendedPurpose));
        private set => this.PersistentState.SetString(nameof(this.IntendedPurpose), value);
    }

    public string LockerStatus
    {
        get => this.PersistentState.GetString(nameof(this.LockerStatus));
        private set => this.PersistentState.SetString(nameof(this.LockerStatus), value);
    }

    public string RejectionReason
    {
        get => this.PersistentState.GetString(nameof(this.RejectionReason));
        private set => this.PersistentState.SetString(nameof(this.RejectionReason), value);
    }

    public uint State
    {
        get => this.PersistentState.GetUInt32(nameof(this.State));
        private set => this.PersistentState.SetUInt32(nameof(this.State), value);
    }

    public void BeginReviewProcess()
    {

    }

    public void RejectApplication(string rejectionReason)
    {

    }

    public void UploadDocuments(string lockerIdentifier, string image)
    {

    }

    public void ShareWithThirdParty(Address thirdPartyRequestor, string expirationDate, string intendedPurpose)
    {

    }

    public void AcceptSharingRequest()
    {

    }

    public void RejectSharingRequest()
    {

    }

    public void RequestLockerAccess(string intendedPurpose)
    {

    }

    public void ReleaseLockerAccess()
    {

    }

    public void RevokeAccessFromThirdParty()
    {

    }

    public void Terminate()
    {

    }
}
