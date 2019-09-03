namespace NonFungibleToken.Tests
{
    using System;
    using System.Collections.Generic;
    using Moq;
    using Stratis.SmartContracts;
    using Stratis.SmartContracts.CLR;
    using Xunit;

    public class NonFungibleTokenTests
    {
        private Mock<ISmartContractState> smartContractStateMock;
        private Mock<IContractLogger> contractLoggerMock;
        private Mock<IPersistentState> persistentStateMock;
        private Dictionary<string, bool> supportedInterfaces;
        private Dictionary<string, Address> idToOwner;
        private Dictionary<string, Address> idToApproval;
        private Dictionary<string, bool> ownerToOperator;
        private Dictionary<string, ulong> ownerToNFTokenCount;

        public NonFungibleTokenTests()
        {
            this.contractLoggerMock = new Mock<IContractLogger>();
            this.persistentStateMock = new Mock<IPersistentState>();
            this.smartContractStateMock = new Mock<ISmartContractState>();
            this.smartContractStateMock.Setup(s => s.PersistentState).Returns(this.persistentStateMock.Object);
            this.smartContractStateMock.Setup(s => s.ContractLogger).Returns(this.contractLoggerMock.Object);

            this.supportedInterfaces = new Dictionary<string, bool>();
            this.idToOwner = new Dictionary<string, Address>();
            this.idToApproval = new Dictionary<string, Address>();
            this.ownerToOperator = new Dictionary<string, bool>();
            this.ownerToNFTokenCount = new Dictionary<string, ulong>();

            this.SetupPersistentState();
        }

        [Fact]
        public void Constructor_Sets_SupportedInterfaces()
        {
            var nonFungibleToken = this.CreateNonFungibleToken();

            Assert.Equal(3, this.supportedInterfaces.Count);
            Assert.True(this.supportedInterfaces["SupportedInterface:1"]);
            Assert.True(this.supportedInterfaces["SupportedInterface:2"]);
            Assert.True(this.supportedInterfaces["SupportedInterface:3"]);
        }

        [Fact]
        public void SupportsInterface_InterfaceSupported_ReturnsTrue()
        {
            var nonFungibleToken = this.CreateNonFungibleToken();

            var result = nonFungibleToken.SupportsInterface(3);

            Assert.True(result);
        }

        [Fact]
        public void SupportsInterface_InterfaceSetToFalseSupported_ReturnsFalse()
        {
            var nonFungibleToken = this.CreateNonFungibleToken();
            this.supportedInterfaces["SupportedInterface:3"] = false;

            var result = nonFungibleToken.SupportsInterface(3);

            Assert.False(result);
        }

        [Fact]
        public void SupportsInterface_InterfaceNotSupported_ReturnsFalse()
        {
            var nonFungibleToken = this.CreateNonFungibleToken();

            var result = nonFungibleToken.SupportsInterface(4);

            Assert.False(result);
        }

        [Fact]
        public void OnNonFungibleTokenReceived_ReturnsTrue()
        {
            var nonFungibleToken = this.CreateNonFungibleToken();

            var result = nonFungibleToken.OnNonFungibleTokenReceived(Address.Zero, Address.Zero, 1, new byte[0]);

            Assert.True(result);
        }

        [Fact]
        public void GetApproved_NotValidNFToken_OwnerAddressZero_ThrowsException()
        {
            var nonFungibleToken = this.CreateNonFungibleToken();

            Assert.Throws<SmartContractAssertException>(() => nonFungibleToken.GetApproved(1));
        }

        [Fact]
        public void GetApproved_ApprovalNotInStorage_ReturnsZeroAddress()
        {
            this.idToOwner.Add("IdToOwner:1", "0x0000000000000000000000000000000000000005".HexToAddress());
            this.idToApproval.Clear();

            var nonFungibleToken = this.CreateNonFungibleToken();
            var result = nonFungibleToken.GetApproved(1);

            Assert.Equal(Address.Zero, result);
        }

        [Fact]
        public void GetApproved_ApprovalInStorage_ReturnsAddress()
        {
            var approvalAddress = "0x0000000000000000000000000000000000000006".HexToAddress();
            this.idToOwner.Add("IdToOwner:1", "0x0000000000000000000000000000000000000005".HexToAddress());
            this.idToApproval.Add("IdToApproval:1", approvalAddress);

            var nonFungibleToken = this.CreateNonFungibleToken();
            var result = nonFungibleToken.GetApproved(1);

            Assert.Equal(approvalAddress, result);
        }

        [Fact]
        public void IsApprovedForAll_OwnerToOperatorInStateAsTrue_ReturnsTrue()
        {
            var ownerAddress = "0x0000000000000000000000000000000000000006".HexToAddress();
            var operatorAddresss = "0x0000000000000000000000000000000000000007".HexToAddress();
            this.ownerToOperator.Add($"OwnerToOperator:{ownerAddress}:{operatorAddresss}", true);

            var nonFungibleToken = this.CreateNonFungibleToken();

            var result = nonFungibleToken.IsApprovedForAll(ownerAddress, operatorAddresss);

            Assert.True(result);
        }

        [Fact]
        public void IsApprovedForAll_OwnerToOperatorInStateAsFalse_ReturnsFalse()
        {
            var ownerAddress = "0x0000000000000000000000000000000000000006".HexToAddress();
            var operatorAddresss = "0x0000000000000000000000000000000000000007".HexToAddress();
            this.ownerToOperator.Add($"OwnerToOperator:{ownerAddress}:{operatorAddresss}", false);

            var nonFungibleToken = this.CreateNonFungibleToken();

            var result = nonFungibleToken.IsApprovedForAll(ownerAddress, operatorAddresss);

            Assert.False(result);
        }

        [Fact]
        public void IsApprovedForAll_OwnerToOperatorNotInState_ReturnsFalse()
        {
            var ownerAddress = "0x0000000000000000000000000000000000000006".HexToAddress();
            var operatorAddresss = "0x0000000000000000000000000000000000000007".HexToAddress();
            this.ownerToOperator.Clear();

            var nonFungibleToken = this.CreateNonFungibleToken();

            var result = nonFungibleToken.IsApprovedForAll(ownerAddress, operatorAddresss);

            Assert.False(result);
        }

        [Fact]
        public void OwnerOf_IdToOwnerNotInStorage_ThrowsException()
        {
            this.idToOwner.Clear();
            var nonFungibleToken = this.CreateNonFungibleToken();

            Assert.Throws<SmartContractAssertException>(() => nonFungibleToken.OwnerOf(1));
        }

        [Fact]
        public void OwnerOf_NFTokenMappedToAddressZero_ThrowsException()
        {
            this.idToOwner.Add("IdToOwner:1", Address.Zero);
            var nonFungibleToken = this.CreateNonFungibleToken();

            Assert.Throws<SmartContractAssertException>(() => nonFungibleToken.OwnerOf(1));
        }

        [Fact]
        public void OwnerOf_NFTokenExistsWithOwner_ReturnsOwnerAddress()
        {
            var ownerAddress = "0x0000000000000000000000000000000000000006".HexToAddress();
            this.idToOwner.Add("IdToOwner:1", ownerAddress);
            var nonFungibleToken = this.CreateNonFungibleToken();

            var result = nonFungibleToken.OwnerOf(1);

            Assert.Equal(ownerAddress, result);
        }

        [Fact]
        public void BalanceOf_OwnerZero_ThrowsException()
        {
            var nonFungibleToken = this.CreateNonFungibleToken();

            Assert.Throws<SmartContractAssertException>(() => { nonFungibleToken.BalanceOf(Address.Zero); });
        }

        [Fact]
        public void BalanceOf_NftTokenCountNotInStorage_ReturnsZero()
        {
            var ownerAddress = "0x0000000000000000000000000000000000000006".HexToAddress();
            this.ownerToNFTokenCount.Clear();
            var nonFungibleToken = this.CreateNonFungibleToken();

            var result = nonFungibleToken.BalanceOf(ownerAddress);

            Assert.Equal((ulong)0, result);
        }

        [Fact]
        public void BalanceOf_OwnerNftTokenCountInStorage_ReturnsTokenCount()
        {
            var ownerAddress = "0x0000000000000000000000000000000000000006".HexToAddress();
            this.ownerToNFTokenCount.Add($"OwnerToNFTokenCount:{ownerAddress}", 15);
            var nonFungibleToken = this.CreateNonFungibleToken();

            var result = nonFungibleToken.BalanceOf(ownerAddress);

            Assert.Equal((ulong)15, result);
        }

        private NonFungibleToken CreateNonFungibleToken()
        {
            return new NonFungibleToken(this.smartContractStateMock.Object);
        }

        private void SetupPersistentState()
        {
            this.SetupSupportedInterfaces();
            this.SetupIdToOwner();
            this.SetupIdToApproval();
            this.SetupOwnerToOperators();
            this.SetupOwnerToNFTokenCount();
        }

        private void SetupOwnerToNFTokenCount()
        {
            this.persistentStateMock.Setup(p => p.SetUInt64(It.Is<string>(s => s.StartsWith("OwnerToNFTokenCount:", StringComparison.Ordinal)), It.IsAny<ulong>()))
                .Callback<string, ulong>((key, value) =>
                {
                    if (this.ownerToNFTokenCount.ContainsKey(key))
                    {
                        this.ownerToNFTokenCount[key] = value;
                    }
                    else
                    {
                        this.ownerToNFTokenCount.Add(key, value);
                    }
                });
            this.persistentStateMock.Setup(p => p.GetUInt64(It.Is<string>(s => s.StartsWith("OwnerToNFTokenCount:"))))
                .Returns<string>((key) =>
                {
                    if (this.ownerToNFTokenCount.ContainsKey(key))
                    {
                        return this.ownerToNFTokenCount[key];
                    }

                    return default(ulong);
                });
        }

        private void SetupOwnerToOperators()
        {
            this.persistentStateMock.Setup(p => p.SetBool(It.Is<string>(s => s.StartsWith("OwnerToOperator:", StringComparison.Ordinal)), It.IsAny<bool>()))
                .Callback<string, bool>((key, value) =>
                {
                    if (this.ownerToOperator.ContainsKey(key))
                    {
                        this.ownerToOperator[key] = value;
                    }
                    else
                    {
                        this.ownerToOperator.Add(key, value);
                    }
                });
            this.persistentStateMock.Setup(p => p.GetBool(It.Is<string>(s => s.StartsWith("OwnerToOperator:"))))
                .Returns<string>((key) =>
                {
                    if (this.ownerToOperator.ContainsKey(key))
                    {
                        return this.ownerToOperator[key];
                    }

                    return default(bool);
                });
        }

        private void SetupIdToApproval()
        {
            this.persistentStateMock.Setup(p => p.SetAddress(It.Is<string>(s => s.StartsWith("IdToApproval:", StringComparison.Ordinal)), It.IsAny<Address>()))
               .Callback<string, Address>((key, value) =>
               {
                   if (this.idToApproval.ContainsKey(key))
                   {
                       this.idToApproval[key] = value;
                   }
                   else
                   {
                       this.idToApproval.Add(key, value);
                   }
               });
            this.persistentStateMock.Setup(p => p.GetAddress(It.Is<string>(s => s.StartsWith("IdToApproval:"))))
                .Returns<string>((key) =>
                {
                    if (this.idToApproval.ContainsKey(key))
                    {
                        return this.idToApproval[key];
                    }

                    return Address.Zero;
                });
        }

        private void SetupIdToOwner()
        {
            this.persistentStateMock.Setup(p => p.SetAddress(It.Is<string>(s => s.StartsWith("IdToOwner:", StringComparison.Ordinal)), It.IsAny<Address>()))
               .Callback<string, Address>((key, value) =>
               {
                   if (this.idToOwner.ContainsKey(key))
                   {
                       this.idToOwner[key] = value;
                   }
                   else
                   {
                       this.idToOwner.Add(key, value);
                   }
               });
            this.persistentStateMock.Setup(p => p.GetAddress(It.Is<string>(s => s.StartsWith("IdToOwner:"))))
                .Returns<string>((key) =>
                {
                    if (this.idToOwner.ContainsKey(key))
                    {
                        return this.idToOwner[key];
                    }

                    return Address.Zero;
                });
        }

        private void SetupSupportedInterfaces()
        {
            this.persistentStateMock.Setup(p => p.SetBool(It.Is<string>(s => s.StartsWith("SupportedInterface:", StringComparison.Ordinal)), It.IsAny<bool>()))
                .Callback<string, bool>((key, value) =>
                {
                    if (this.supportedInterfaces.ContainsKey(key))
                    {
                        this.supportedInterfaces[key] = value;
                    }
                    else
                    {
                        this.supportedInterfaces.Add(key, value);
                    }
                });
            this.persistentStateMock.Setup(p => p.GetBool(It.Is<string>(s => s.StartsWith("SupportedInterface:"))))
                .Returns<string>((key) =>
                {
                    if (this.supportedInterfaces.ContainsKey(key))
                    {
                        return this.supportedInterfaces[key];
                    }

                    return default(bool);
                });
        }
    }
}
