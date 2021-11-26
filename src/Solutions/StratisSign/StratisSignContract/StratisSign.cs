using System;
using Stratis.SmartContracts;

/// <summary>
/// Smart Contract to create agreements and sign documents.
/// It saves the stamping information to the contract state. 
/// </summary>
[Deploy]
public class StratisSign : SmartContract
{
    public StratisSign(ISmartContractState smartContractState)
        : base(smartContractState)
    {
    }

    public Agreement GetAgreement(string id)
    {
        return this.PersistentState.GetStruct<Agreement>(id);
    }

    private void SetAgreement(Agreement agreement)
    {
        this.PersistentState.SetStruct(agreement.Id, agreement);
    }

    private void SetStamp(Stamp stamp)
    {
        this.PersistentState.SetStruct($"stamp:{stamp.AgreementId}:{stamp.Signer}", stamp);
    }

    public Stamp GetStamp(string agreementId, Address signer)
    {
        return this.PersistentState.GetStruct<Stamp>($"stamp:{agreementId}:{signer}");
    }

    public bool CreateAgreement(string agreementId, string fileHash)
    {
        Assert(string.IsNullOrEmpty(GetAgreement(agreementId).Id), "Agreement already exists!");

        this.SetAgreement(new Agreement
        {
            Id = agreementId,
            FileHash = fileHash,
            Owner = Message.Sender
        });

        return true;
    }

    public bool AddSigners(string agreementId, Address signer)
    {
        var agreement = this.GetAgreement(agreementId);

        Assert(!string.IsNullOrEmpty(agreement.Id), "Agreement does not exists!");

        Assert(agreement.Owner == Message.Sender);

        if (agreement.Signers == null)
        {            
            //Initialize a dynamic array with length 1, resize it later to add more signers.             
            agreement.Signers = new Address[1];
            agreement.Signers[0] = signer;
        }
        else
        {
            Assert(agreement.Signers.Length < 2, "Only two signers are allowed");

            //resize an array for other signers. 
            //This will be used when more than two signers are allowed to add.
            Array.Resize(ref agreement.Signers, agreement.Signers.Length + 1);
            agreement.Signers[agreement.Signers.Length - 1] = signer;
        }

        this.SetAgreement(agreement);
        return true;
    }

    public bool SignAgreement(string agreementId, string digitalSign)
    {
        var agreement = this.GetAgreement(agreementId);

        Assert(!string.IsNullOrEmpty(agreement.Id), "Agreement does not exists!");

        Assert(!agreement.IsSigned, "Agreement is already signed");

        Assert(CheckRequestedSignerIsValid(agreement.Signers, Message.Sender), "Requested signer is not valid for this agreement");

        this.SetStamp(new Stamp
        {
            AgreementId = agreement.Id,
            Signer = Message.Sender,
            DigitalSign = digitalSign,
            BlockNumber = Block.Number
        });

        agreement.TotalSignDone++;

        if (agreement.Signers.Length == agreement.TotalSignDone)
        {
            agreement.IsSigned = true;
        }

        this.SetAgreement(agreement);

        return true;
    }

    private bool CheckRequestedSignerIsValid(Address[] agreementSigners, Address requestedSigner)
    {
        for (int i = 0; i < agreementSigners.Length; i++)
        {
            if (agreementSigners[i] == requestedSigner)
            {
                return true;
            }
        }
        return false;
    }

    public struct Agreement
    {
        [Index]
        public string Id;

        public string FileHash;

        public Address Owner;        

        public Address[] Signers;

        public Int32 TotalSignDone;

        public bool IsSigned;
    }

    public struct Stamp
    {
        public string AgreementId;

        public Address Signer;

        public string DigitalSign;

        public ulong BlockNumber;
    }
}
