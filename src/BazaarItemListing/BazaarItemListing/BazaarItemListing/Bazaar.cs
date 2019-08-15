using System;
using Stratis.SmartContracts;

public class Bazaar : SmartContract
{
    public Bazaar(ISmartContractState smartContractState, Address partyA, int v1, Address partyB, int v2)
        :base(smartContractState)
    {
    }

    public enum StateEnum: uint
    {
        PartyProvisioned = 0,
        ItemListed = 1,
        CurrentSaleFinalized = 2
    }

    public uint State { get; set; }

    public Address PartyA { get; set; }

    public ulong BalancePartyA { get; set; }

    public Address PartyB { get; set; }

    public ulong BalancePartyB { get; set; }

    public Address BazaarMaintainer { get; set; }

    public Address CurrentSeller { get; set; }

    public Address CurrentItemListing { get; set; }

    public string ItemName { get; set; }

    public ulong ItemPrice { get; set; }

    public bool HasBalance(Address buyer, ulong itemPrice)
    {
        return false;
    }

    public void UpdateBalance(Address seller, Address buyer, ulong price)
    {
    }

    public void ListItem(string itemName, ulong itemPrice)
    {
    }
}
