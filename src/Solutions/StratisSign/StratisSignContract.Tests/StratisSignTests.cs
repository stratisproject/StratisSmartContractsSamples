namespace StratisSignContract.Tests
{
    using Moq;
    using Stratis.SmartContracts;
    using Stratis.SmartContracts.CLR;
    using Xunit;

    public class StratisSignTests
    {
        private const string Signer1 = "0x0000000000000000000000000000000000000001";
        private const string Signer2 = "0x0000000000000000000000000000000000000002";
        private const string Signer3 = "0x0000000000000000000000000000000000000003";
        private const string Owner = "0x0000000000000000000000000000000000000004";

        private const string AgreementId = "9f1cf27c-adf9-4c6b-ae3a-c5bdcbbc1f5c";
        private const string FileHash = "3151f9a87664abad39cc07b47f9e57a890492347441aa8e3e2da64f6cf29af85";
        private const string DigitalSignature = "cc07b47f9e57a890492347441";

        private static readonly Address OwnerAddress = Owner.HexToAddress();
        private static readonly Address Signer1Address = Signer1.HexToAddress();
        private static readonly Address Signer2Address = Signer2.HexToAddress();
        private static readonly Address Signer3Address = Signer3.HexToAddress();

        private readonly Mock<ISmartContractState> mockContractState;
        private readonly Mock<IPersistentState> mockPersistentState;
        private readonly Mock<IInternalTransactionExecutor> mockInternalExecutor;
        private readonly Mock<IContractLogger> mockContractLogger;

        public StratisSignTests()
        {
            this.mockContractLogger = new Mock<IContractLogger>();
            this.mockPersistentState = new Mock<IPersistentState>();
            this.mockInternalExecutor = new Mock<IInternalTransactionExecutor>();
            this.mockContractState = new Mock<ISmartContractState>();
            this.mockContractState.Setup(s => s.PersistentState).Returns(this.mockPersistentState.Object);
            this.mockContractState.Setup(s => s.ContractLogger).Returns(this.mockContractLogger.Object);
            this.mockContractState.Setup(s => s.InternalTransactionExecutor).Returns(this.mockInternalExecutor.Object);
        }

        [Theory]
        [InlineData(AgreementId, FileHash)]
        public void CreateAgreement_Succeeds(string id, string fileHash)
        {
            this.mockContractState.Setup(s => s.Message.Sender).Returns(OwnerAddress);

            var contract = this.NewStratisSign();
            var result = contract.CreateAgreement(id, fileHash);

            this.mockPersistentState.Verify(s => s.SetStruct(id, this.NewAgreement()), Times.Once);
            Assert.True(result);
        }

        [Theory]
        [InlineData(AgreementId)]
        public void CreateAgreement_Fails_For_Same_Id(string agreementId)
        {
            var newAgreement = this.NewAgreement();

            this.mockContractState.Setup(s => s.Message.Sender).Returns(OwnerAddress);
            this.mockPersistentState.Setup(s => s.GetStruct<StratisSign.Agreement>(agreementId)).Returns(newAgreement);

            var contract = this.NewStratisSign();

            Assert.Throws<SmartContractAssertException>(() => contract.CreateAgreement(agreementId, FileHash));

            this.mockPersistentState.Verify(s => s.SetStruct(agreementId, newAgreement), Times.Never);
        }

        [Theory]
        [InlineData(AgreementId)]
        public void AddSigner_Succeeds(string agreementId)
        {
            var newAgreement = this.NewAgreement();
            this.mockPersistentState.Setup(s => s.GetStruct<StratisSign.Agreement>(agreementId)).Returns(newAgreement);

            this.mockContractState.Setup(s => s.Message.Sender).Returns(OwnerAddress);

            var contract = this.NewStratisSign();
            var result = contract.AddSigners(agreementId, Signer1Address);

            Assert.True(result);
        }

        [Fact]
        public void AddSigner_Fails_If_Agreement_Not_Exists()
        {
            var newAgreement = this.NewAgreement();
            var contract = this.NewStratisSign();

            Assert.Throws<SmartContractAssertException>(() => contract.AddSigners(newAgreement.Id, Address.Zero));
        }

        [Fact]
        public void AddSigner_Fails_Sender_Not_Owner()
        {
            var newAgreement = this.NewAgreement();

            this.mockContractState.Setup(s => s.Message.Sender).Returns(Address.Zero);

            this.mockPersistentState.Setup(s => s.GetStruct<StratisSign.Agreement>(AgreementId)).Returns(newAgreement);

            var contract = this.NewStratisSign();
            Assert.Throws<SmartContractAssertException>(() => contract.AddSigners(newAgreement.Id, Signer1Address));
        }

        [Fact]
        public void AddSigner_Fails_If_Morethan_Two_Signers()
        {
            var newAgreement = this.NewAgreement();
            newAgreement.Signers = new Address[2];
            newAgreement.Signers[0] = Signer1Address;
            newAgreement.Signers[1] = Signer2Address;

            this.mockPersistentState.Setup(s => s.GetStruct<StratisSign.Agreement>(AgreementId)).Returns(newAgreement);

            this.mockContractState.Setup(s => s.Message.Sender).Returns(OwnerAddress);

            var contract = this.NewStratisSign();
            Assert.Throws<SmartContractAssertException>(() => contract.AddSigners(AgreementId, Address.Zero));
        }

        [Theory]
        [InlineData(AgreementId)]
        public void SignAgreement_Succeeds(string agreementId)
        {
            var newAgreement = this.NewAgreementWithSigners();

            this.mockPersistentState.Setup(s => s.GetStruct<StratisSign.Agreement>(agreementId)).Returns(newAgreement);
            this.mockContractState.Setup(s => s.Message.Sender).Returns(Signer1Address);
            this.mockContractState.Setup(s => s.Block.Number).Returns(It.IsAny<ulong>);

            var contract = this.NewStratisSign();
            var result = contract.SignAgreement(agreementId, DigitalSignature);

            this.mockPersistentState.Verify(s => s.SetStruct($"stamp:{newAgreement.Id}:{Signer1Address}", this.NewStamp(Signer1Address)), Times.Once);
            Assert.True(result);
        }

        [Fact]
        public void SignAgreement_Fails_If_Agreement_Not_Exists()
        {
            var newAgreement = this.NewAgreement();
            var contract = this.NewStratisSign();

            this.mockContractState.Setup(s => s.Message.Sender).Returns(Signer1Address);

            Assert.Throws<SmartContractAssertException>(() => contract.SignAgreement(newAgreement.Id, DigitalSignature));
            this.mockPersistentState.Verify(s => s.SetStruct($"stamp:{newAgreement.Id}:{Signer1Address}", this.NewStamp(Signer1Address)), Times.Never);
        }

        [Fact]
        public void SignAgreement_Fails_If_Already_Signed()
        {
            var newAgreement = this.NewAgreement();
            newAgreement.IsSigned = true;

            this.mockPersistentState.Setup(s => s.GetStruct<StratisSign.Agreement>(newAgreement.Id)).Returns(newAgreement);

            var contract = this.NewStratisSign();

            Assert.Throws<SmartContractAssertException>(() => contract.SignAgreement(newAgreement.Id, DigitalSignature));
        }

        [Fact]
        public void SignAgreement_Fails_If_Signer_Is_Invalid()
        {
            var newAgreement = this.NewAgreementWithSigners();

            this.mockPersistentState.Setup(s => s.GetStruct<StratisSign.Agreement>(newAgreement.Id)).Returns(newAgreement);
            this.mockContractState.Setup(s => s.Message.Sender).Returns(Signer3Address);

            var contract = this.NewStratisSign();

            Assert.Throws<SmartContractAssertException>(() => contract.SignAgreement(newAgreement.Id, DigitalSignature));
        }


        private StratisSign.Agreement NewAgreementWithSigners()
        {
            var newAgreement = this.NewAgreement();
            newAgreement.Signers = new Address[2];
            newAgreement.Signers[0] = Signer1Address;
            newAgreement.Signers[1] = Signer2Address;

            return newAgreement;
        }

        private StratisSign.Agreement NewAgreement()
        {
            return new StratisSign.Agreement
            {
                Id = AgreementId,
                FileHash = FileHash,
                Owner = OwnerAddress
            };
        }

        private StratisSign.Stamp NewStamp(Address signer)
        {
            return new StratisSign.Stamp
            {
                AgreementId = AgreementId,
                Signer = signer,
                BlockNumber = 0,
                DigitalSign = DigitalSignature
            };
        }

        private StratisSign NewStratisSign()
        {
            return new StratisSign(this.mockContractState.Object);
        }
    }
}
