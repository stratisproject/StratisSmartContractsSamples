using System;
using Stratis.SmartContracts;

public class DefectiveComponentCounter : SmartContract
{
    public DefectiveComponentCounter(ISmartContractState state, string defectiveComponentsCount)
        : base(state)
    {
        this.Manufacturer = Message.Sender;
        
        var components = defectiveComponentsCount.Split(',', StringSplitOptions.RemoveEmptyEntries);

        var defectiveComponents = new uint[12];

        for (uint i = 0; i < 12; i++)
        {
            if (components.Length < i + 1)
                break;

            Assert(uint.TryParse(components[i], out var value));

            defectiveComponents[i] = value;
        }

        this.DefectiveComponentsCount = defectiveComponents;
        this.Total = 0;
        this.State = (uint)StateType.Create;
    }

    public enum StateType : uint
    {
        Create = 0,
        ComputeTotal = 1
    }

    public uint State
    {
        get => this.PersistentState.GetUInt32(nameof(this.State));
        private set => this.PersistentState.SetUInt32(nameof(this.State), value);
    }

    public Address Manufacturer
    {
        get => this.PersistentState.GetAddress(nameof(this.Manufacturer));
        private set => this.PersistentState.SetAddress(nameof(this.Manufacturer), value);
    }

    private uint[] DefectiveComponentsCount
    {
        get => this.PersistentState.GetArray<uint>(nameof(this.DefectiveComponentsCount));
        set => this.PersistentState.SetArray(nameof(this.DefectiveComponentsCount), value);
    }

    public uint Total
    {
        get => this.PersistentState.GetUInt32(nameof(this.Total));
        private set => this.PersistentState.SetUInt32(nameof(this.Total), value);
    }

    public void ComputeTotal()
    {
        Assert(this.Manufacturer == Message.Sender);

        uint runningTotal = 0;

        for (uint i = 0; i < 12; i++)
        {
            checked
            {
                runningTotal += this.DefectiveComponentsCount[i];
            }
        }

        this.Total = runningTotal;

        this.State = (uint)StateType.ComputeTotal;
    }

    /// <summary>
    /// Gets the defective component count for the zero-indexed month of the year.
    /// January = 0, December = 11.
    /// </summary>
    /// <param name="month"></param>
    /// <returns></returns>
    public uint GetDefectiveComponentsCount(uint month)
    {
        Assert(month < 12U);

        return this.DefectiveComponentsCount[month];        
    }

    public void SetDefectiveComponentsCount(uint month, uint value)
    {
        Assert(Message.Sender == Manufacturer);
        Assert(month < 12U);

        /*
         Warning: here lies a potential 'gotcha'.
         In order to modify the array, we first need to make a copy of it in memory, then persist the copy.
         Modifying the property via DefectiveComponentsCount[index] will only modify a local instance of the array.
        */
        var defectiveComponents = this.DefectiveComponentsCount;

        defectiveComponents[month] = value;

        this.DefectiveComponentsCount = defectiveComponents;
    }
}
