using System;
using Stratis.SmartContracts;

public class BasicProvenanceContract : SmartContract
{
    public BasicProvenanceContract(ISmartContractState smartContractState, Address supplyChainOwner, Address supplyChainObserver)
    : base(smartContractState)
    {
        this.InitiatingCounterParty = Message.Sender;
        this.CounterParty = Message.Sender;
        this.SupplyChainOwner = supplyChainOwner;
        this.SupplyChainObserver = supplyChainObserver;
        State = (uint)StateType.Created;
    }

    public enum StateType : uint
    {
        Created = 0,
        InTransit = 1,
        Completed = 2
    }

    public uint State
    {
        get => this.PersistentState.GetUInt32(nameof(this.State));
        private set => this.PersistentState.SetUInt32(nameof(this.State), value);
    }

    public Address InitiatingCounterParty
    {
        get => this.PersistentState.GetAddress(nameof(this.InitiatingCounterParty));
        private set => this.PersistentState.SetAddress(nameof(this.InitiatingCounterParty), value);
    }

    public Address CounterParty
    {
        get => this.PersistentState.GetAddress(nameof(this.CounterParty));
        private set => this.PersistentState.SetAddress(nameof(this.CounterParty), value);
    }

    public Address PreviousCounterParty
    {
        get => this.PersistentState.GetAddress(nameof(this.PreviousCounterParty));
        private set => this.PersistentState.SetAddress(nameof(this.PreviousCounterParty), value);
    }

    public Address SupplyChainOwner
    {
        get => this.PersistentState.GetAddress(nameof(this.SupplyChainOwner));
        private set => this.PersistentState.SetAddress(nameof(this.SupplyChainOwner), value);
    }
    public Address SupplyChainObserver
    {
        get => this.PersistentState.GetAddress(nameof(this.SupplyChainObserver));
        private set => this.PersistentState.SetAddress(nameof(this.SupplyChainObserver), value);
    }

    public void TransferResponsibility(Address newCounterParty)
    {
        Assert(this.CounterParty == Message.Sender && this.State != (uint)StateType.Completed);

        if (this.State == (uint)StateType.Created)
        {
            State = (uint)StateType.InTransit;
        }

        PreviousCounterParty = CounterParty;
        CounterParty = newCounterParty;
    }

    public void Complete()
    {
        Assert(this.SupplyChainOwner == Message.Sender && State != (uint)StateType.Completed);

        State = (uint)StateType.Completed;
        PreviousCounterParty = CounterParty;
        CounterParty = Address.Zero;
    }
}