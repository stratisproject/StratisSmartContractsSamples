using Stratis.SmartContracts;
using System;
[Deploy]

public class AgreementContract : SmartContract
{
    public AgreementContract(ISmartContractState smartContractState, Address Retailer, Address Distributor, ulong IssueDate, ulong WithdrawStartRole)
   : base(smartContractState)
    {
        this.Issuer = Message.Sender;
        this.Distributor = Distributor;
        this.Retailer = Retailer;
        this.IssueDate = IssueDate;
        this.WithdrawStartRole = WithdrawStartRole;
    }
    public enum StatusType : uint
    {
        Inventory = 0,
        Sold = 1
    }

    public Address Issuer
    {
        get => PersistentState.GetAddress(nameof(Issuer));
        private set => PersistentState.SetAddress(nameof(Issuer), value);
    }
    public Address Retailer
    {
        get => PersistentState.GetAddress(nameof(Retailer));
        private set => PersistentState.SetAddress(nameof(Retailer), value);
    }
    public Address Distributor
    {
        get => PersistentState.GetAddress(nameof(Distributor));
        private set => PersistentState.SetAddress(nameof(Distributor), value);
    }
    public bool DistributorSign
    {
        get => PersistentState.GetBool(nameof(DistributorSign));
        private set => PersistentState.SetBool(nameof(DistributorSign), value);
    }
    public bool RetailerSign
    {
        get => PersistentState.GetBool(nameof(RetailerSign));
        private set => PersistentState.SetBool(nameof(RetailerSign), value);
    }
    public ulong IssueDate
    {
        get => PersistentState.GetUInt64(nameof(IssueDate));
        private set => PersistentState.SetUInt64(nameof(IssueDate), value);
    }
    public ulong SignDate
    {
        get => PersistentState.GetUInt64(nameof(SignDate));
        private set => PersistentState.SetUInt64(nameof(SignDate), value);
    }
    //public ulong ValidationDate
    //{
    //    get => PersistentState.GetUInt64(nameof(ValidationDate));
    //    private set => PersistentState.SetUInt64(nameof(ValidationDate), value);
    //}
    public ulong WithdrawStartRole
    {
        get => PersistentState.GetUInt64(nameof(WithdrawStartRole));
        private set => PersistentState.SetUInt64(nameof(WithdrawStartRole), value);
    }
    public ulong ItemCount
    {
        get => PersistentState.GetUInt64(nameof(ItemCount));
        private set => PersistentState.SetUInt64(nameof(ItemCount), value);
    }
    public ulong SoldItems
    {
        get => PersistentState.GetUInt64(nameof(SoldItems));
        private set => PersistentState.SetUInt64(nameof(SoldItems), value);
    }
    public ulong PaidAmount
    {
        get => PersistentState.GetUInt64(nameof(PaidAmount));
        private set => PersistentState.SetUInt64(nameof(PaidAmount), value);
    }
    public ulong TotalPrice
    {
        get => PersistentState.GetUInt64(nameof(TotalPrice));
        private set => PersistentState.SetUInt64(nameof(TotalPrice), value);
    }
    public string AgreementContractHash
    {
        get => PersistentState.GetString(nameof(AgreementContractHash));
        private set => PersistentState.SetString(nameof(AgreementContractHash), value);
    }
    public Item GeBellItem(string Key)
    {
        return PersistentState.GetStruct<Item>($"BellItem:{Key}");
    }

    private void SetBellItem(string Key, Item value)
    {
        PersistentState.SetStruct($"BellItem:{Key}", value);
    }
    public struct Item
    {
        public string ItemName; public ulong itemPrice; public uint status;

    }
    /// <summary>
    /// both parties should call it to agree on the contract 
    /// </summary>
    /// <param name="AgreementContractHash"> hash of the paper document digitally signed</param>
    public void Sign(string AgreementContractHash)
    {
        Assert(this.Distributor == this.Message.Sender || this.Retailer == this.Message.Sender);
        if (this.Distributor == this.Message.Sender)
        {
            this.DistributorSign = true;
        }
        else
        {
            this.RetailerSign = true;
        }
        this.AgreementContractHash = AgreementContractHash;
        this.SignDate = this.Block.Number;
    }
    // this should be added as array in constractor but it seams that it's not supported 
    /// <summary>
    /// any party can call it to ass product item to contract before signing
    /// </summary>
    /// <param name="Key"> item identification key , must be unique in string datatype</param>
    /// <param name="ItemName"> item name in string datatype</param>
    /// <param name="itemPrice"> item price to be paid, should be ulong datatype</param>
    public void AddToBill(string Key, string ItemName, ulong itemPrice)
    {
        Assert(this.Distributor == this.Message.Sender || this.Retailer == this.Message.Sender);
        Assert(this.DistributorSign == false && this.RetailerSign == false);
        Item Item = new Item { ItemName = ItemName, itemPrice = itemPrice, status = (uint)StatusType.Inventory };
        this.ItemCount++;
        this.TotalPrice += itemPrice;
        this.SetBellItem(Key, Item);
    }
    /// <summary>
    /// retailer can call it to add bought items as long as both parties have agreed on the contract after calling sign method
    /// </summary>
    /// <param name="Key"> item key to get it from dictionary</param>
    public void BuyItem(string Key)
    {
        Assert(this.Retailer == this.Message.Sender);
        Assert(this.DistributorSign == true && this.RetailerSign == true);

        var Item = this.GeBellItem(Key);
        Assert(Item.itemPrice <= this.Message.Value);
        Assert(Item.status == (uint)StatusType.Inventory);
        Item.status = (uint)StatusType.Sold;
        this.SoldItems++;
        this.SetBellItem(Key, Item);
    }
    /// <summary>
    /// distributor call it to withdraw his money from contract
    /// </summary>
    /// <returns>boolean to indicate the transfer status</returns>
    public bool Withdraw()
    {
        Assert(this.Distributor == this.Message.Sender);
        var currentPercentage = (this.SoldItems * 100) / this.ItemCount;
        Assert(currentPercentage >= this.WithdrawStartRole);
        this.PaidAmount += this.Balance;
        ITransferResult transferResult = Transfer(this.Distributor, this.Balance);

        return transferResult.Success;
    }


}