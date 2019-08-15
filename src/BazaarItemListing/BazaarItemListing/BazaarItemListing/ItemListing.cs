using System;
using Stratis.SmartContracts;

public class ItemListing : SmartContract
{
    public ItemListing(ISmartContractState state, string itemName, ulong itemPrice, Address seller, Address parent, Address partyA, Address partyB)
        : base(state)
    {
    }

    public enum StateType : uint
    {
        ItemAvailable = 0,
        ItemSold = 1
    }

    public uint State { get; set; }

    public Address Seller { get; set; }

    public Address Buyer { get; set; }

    public Address ParentContract { get; set; }

    public string ItemName { get; set; }

    public ulong ItemPrice { get; set; }

    public Address PartyA { get; set; }

    public Address PartyB { get; set; }

    public void BuyItem()
    {
    }
}