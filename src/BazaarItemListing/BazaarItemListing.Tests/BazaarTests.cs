using Moq;
using Stratis.SmartContracts;
using Stratis.SmartContracts.CLR;
using System;
using System.Collections.Generic;
using System.Reflection;
using Xunit;

namespace BazaarItemListing.Tests
{
    public class BazaarTests
    {
        private const string MaintainerAddress = "0x0000000000000000000000000000000000000001";
        private const string PartyAAddress = "0x0000000000000000000000000000000000000002";
        private const string PartyBAddress = "0x0000000000000000000000000000000000000003";
        private const string NewContractAddress = "0x0000000000000000000000000000000000000004";

        private static readonly Address Maintainer = MaintainerAddress.HexToAddress();
        private static readonly Address PartyA = PartyAAddress.HexToAddress();
        private static readonly Address PartyB = PartyBAddress.HexToAddress();
        private readonly Mock<ISmartContractState> mockContractState;
        private readonly Mock<IPersistentState> mockPersistentState;
        private readonly Mock<IInternalTransactionExecutor> mockInternalExecutor;
        private readonly Mock<IContractLogger> mockContractLogger;

        public BazaarTests()
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
        [InlineData(nameof(Bazaar.PartyA))]
        [InlineData(nameof(Bazaar.PartyB))]
        [InlineData(nameof(Bazaar.BazaarMaintainer))]
        [InlineData(nameof(Bazaar.BalancePartyA))]
        [InlineData(nameof(Bazaar.BalancePartyB))]
        [InlineData(nameof(Bazaar.State))]
        [InlineData(nameof(Bazaar.CurrentSeller))]
        [InlineData(nameof(Bazaar.CurrentItemListing))]
        [InlineData(nameof(Bazaar.ItemName))]
        [InlineData(nameof(Bazaar.ItemPrice))]
        public void Property_Setter_Is_Private(string propertyName)
        {
            Type type = typeof(Bazaar);

            PropertyInfo property = type.GetProperty(propertyName);

            Assert.True(property.SetMethod.IsPrivate);
        }

        [Fact]
        public void Constructor_Sets_Initial_Values()
        {
            var bazaar = this.NewBazaar();

            this.mockPersistentState.Verify(s => s.SetAddress(nameof(Bazaar.PartyA), PartyA));
            this.mockPersistentState.Verify(s => s.SetAddress(nameof(Bazaar.PartyB), PartyB));
            this.mockPersistentState.Verify(s => s.SetAddress(nameof(Bazaar.BazaarMaintainer), Maintainer));
            this.mockPersistentState.Verify(s => s.SetUInt64(nameof(Bazaar.BalancePartyA), 0));
            this.mockPersistentState.Verify(s => s.SetUInt64(nameof(Bazaar.BalancePartyB), 0));
            this.mockPersistentState.Verify(s => s.SetUInt32(nameof(Bazaar.State), (uint)Bazaar.StateEnum.PartyProvisioned));
        }

        [Fact]
        public void Constructor_Checks_Parties_Are_Different()
        {
            this.mockContractState.Setup(s => s.Message.Sender).Returns(Maintainer);

            Assert.Throws<SmartContractAssertException>(() => new Bazaar(this.mockContractState.Object, PartyA, 0, PartyA, 0));
        }

        [Theory]
        [InlineData(nameof(Bazaar.PartyA), ulong.MinValue, ulong.MinValue)]
        [InlineData(nameof(Bazaar.PartyA), 0, 0)]
        [InlineData(nameof(Bazaar.PartyA), 200, 100)]
        [InlineData(nameof(Bazaar.PartyA), ulong.MaxValue, ulong.MaxValue)]
        [InlineData(nameof(Bazaar.PartyB), ulong.MinValue, ulong.MinValue)]
        [InlineData(nameof(Bazaar.PartyB), 0, 0)]
        [InlineData(nameof(Bazaar.PartyB), 200, 100)]
        [InlineData(nameof(Bazaar.PartyB), ulong.MaxValue, ulong.MaxValue)]
        public void HasBalance_Buyer_Balance_Sufficient(string party, ulong partyBalance, ulong itemPrice)
        {
            var bazaar = this.NewBazaar();

            this.mockPersistentState.Setup(s => s.GetUInt64(party)).Returns(partyBalance);

            Assert.True(bazaar.HasBalance(PartyA, itemPrice));
        }

        [Theory]
        [InlineData(nameof(Bazaar.PartyA), ulong.MinValue, ulong.MinValue + 1)]
        [InlineData(nameof(Bazaar.PartyA), 0, 1)]
        [InlineData(nameof(Bazaar.PartyA), 100, 200)]
        [InlineData(nameof(Bazaar.PartyA), ulong.MaxValue, ulong.MaxValue - 1)]
        [InlineData(nameof(Bazaar.PartyB), ulong.MinValue, ulong.MinValue + 1)]
        [InlineData(nameof(Bazaar.PartyB), 0, 1)]
        [InlineData(nameof(Bazaar.PartyB), 100, 200)]
        [InlineData(nameof(Bazaar.PartyB), ulong.MaxValue, ulong.MaxValue - 1)]
        public void HasBalance_Buyer_Balance_Insufficient(string party, ulong partyBalance, ulong itemPrice)
        {
            var bazaar = this.NewBazaar();

            this.mockPersistentState.Setup(s => s.GetUInt64(party)).Returns(partyBalance);

            Assert.False(bazaar.HasBalance(PartyA, itemPrice));
        }

        [Fact]
        public void HasBalance_BuyerAddress_Not_PartyA_Or_PartyB()
        {
            var bazaar = this.NewBazaar();

            Assert.False(bazaar.HasBalance(Address.Zero, 0));
        }

        [Fact]
        public void UpdateBalance_Fails_If_Sender_Not_ItemListing()
        {
            var bazaar = this.NewBazaar();

            // Any sender will do here, as long as it's not an item listing.
            this.mockContractState.Setup(s => s.Message.Sender).Returns(Address.Zero);

            Assert.Throws<SmartContractAssertException>(() => bazaar.HasBalance(PartyA, 0));
        }

        [Theory]
        [MemberData(nameof(UpdateBalanceData))]
        public void UpdateBalance_Succeeds(string sellerParty, Address seller, ulong sellerPartyBalance, string buyerParty, Address buyer, ulong buyerPartyBalance, ulong price)
        {
            var bazaar = this.NewBazaar();

            // Ensure the party has enough balance
            this.mockPersistentState.Setup(s => s.GetUInt64(buyerParty)).Returns(buyerPartyBalance);

            bazaar.UpdateBalance(seller, buyer, price);

            // Verify the correct calls were made
            this.mockPersistentState.Verify(s => s.SetUInt64(sellerParty, sellerPartyBalance + price));

            this.mockPersistentState.Verify(s => s.SetUInt64(buyerParty, buyerPartyBalance - price));

            this.mockPersistentState.Verify(s => s.SetUInt32(nameof(Bazaar.State), (uint) Bazaar.StateEnum.CurrentSaleFinalized));
        }

        [Fact]
        public void UpdateBalance_Price_MaxValue_SellerPartyA_HasBalance_Throws_OverflowException()
        {
            var bazaar = this.NewBazaar();

            var itemPrice = ulong.MaxValue;

            // Seller starts with a balance of 1.
            this.mockPersistentState.Setup(s => s.GetUInt64(nameof(Bazaar.PartyA))).Returns(1);

            // Buyer starts with a balance equal to the item price.
            this.mockPersistentState.Setup(s => s.GetUInt64(nameof(Bazaar.PartyB))).Returns(itemPrice);

            // Party A sells the item to Party B for ulong.MaxValue.
            // This should cause an overflow Party A's balance will attempt to be set to ulong.MaxValue + 1.
            Assert.Throws<OverflowException>(() => bazaar.UpdateBalance(PartyA, PartyB, itemPrice));
        }

        [Fact]
        public void UpdateBalance_Price_MaxValue_SellerPartyB_HasBalance_Throws_OverflowException()
        {
            var bazaar = this.NewBazaar();

            var itemPrice = ulong.MaxValue;

            // Seller starts with a balance of 1.
            this.mockPersistentState.Setup(s => s.GetUInt64(nameof(Bazaar.PartyB))).Returns(1);

            // Buyer starts with a balance equal to the item price.
            this.mockPersistentState.Setup(s => s.GetUInt64(nameof(Bazaar.PartyA))).Returns(itemPrice);

            // Party B sells the item to Party A for ulong.MaxValue.
            Assert.Throws<OverflowException>(() => bazaar.UpdateBalance(PartyB, PartyA, itemPrice));
        }

        [Fact]
        public void ListItem_Fails_Caller_Is_Not_PartyA_PartyB()
        {
            var bazaar = this.NewBazaar();

            this.mockContractState.Setup(s => s.Message.Sender).Returns(Address.Zero);

            Assert.Throws<SmartContractAssertException>(() => bazaar.ListItem("Test", 0));
        }

        [Theory]
        [InlineData(PartyAAddress)]
        [InlineData(PartyBAddress)]
        public void ListItem_Success_PartyA_PartyB(string sender)
        {
            var senderAddress = sender.HexToAddress();
            var newContractAddress = NewContractAddress.HexToAddress();

            var bazaar = this.NewBazaar();

            this.mockContractState.Setup(s => s.Message.Sender).Returns(senderAddress);

            this.mockInternalExecutor.Setup(i => i.Create<ItemListing>(this.mockContractState.Object, 0, null, 0)).Returns(CreateResult.Succeeded(newContractAddress));

            bazaar.ListItem("Test", 0);

            this.mockInternalExecutor.Verify(i => i.Create<ItemListing>(this.mockContractState.Object, 0, null, 0));

            this.mockPersistentState.Verify(s => s.SetAddress(nameof(Bazaar.CurrentItemListing), newContractAddress));

            this.mockPersistentState.Verify(s => s.SetUInt32(nameof(Bazaar.State), (uint)Bazaar.StateEnum.ItemListed));
        }

        public static IEnumerable<object[]> UpdateBalanceData
        {
            get
            {
                var result = new List<object[]>();

                // SellerParty, SellerPartyAddress, SellerPartyBalance, BuyerParty, BuyerPartyAddress, BuyerPartyBalance, SalePrice
                result.Add(new object[] { nameof(Bazaar.PartyA), PartyA, 0, nameof(Bazaar.PartyB), PartyB, 0, 0 });
                result.Add(new object[] { nameof(Bazaar.PartyA), PartyA, 100, nameof(Bazaar.PartyB), PartyB, 200, 150 });
                result.Add(new object[] { nameof(Bazaar.PartyA), PartyA, 0, nameof(Bazaar.PartyB), PartyB, uint.MaxValue, uint.MaxValue - 1, });
                result.Add(new object[] { nameof(Bazaar.PartyA), PartyA, 100, nameof(Bazaar.PartyB), PartyB, 200, 150 });

                result.Add(new object[] { nameof(Bazaar.PartyB), PartyB, 0, nameof(Bazaar.PartyA), PartyA, 0, 0 });
                result.Add(new object[] { nameof(Bazaar.PartyB), PartyB, 100, nameof(Bazaar.PartyA), PartyA, 200, 150 });
                result.Add(new object[] { nameof(Bazaar.PartyB), PartyB, 0, nameof(Bazaar.PartyA), PartyA, uint.MaxValue, uint.MaxValue - 1 });
                result.Add(new object[] { nameof(Bazaar.PartyB), PartyB, 100, nameof(Bazaar.PartyA), PartyA, 200, 150 });

                return result;
            }
        }

        public Bazaar NewBazaar()
        {
            this.mockContractState.Setup(s => s.Message.Sender).Returns(Maintainer);

            return new Bazaar(this.mockContractState.Object, PartyA, 0, PartyB, 0);
        }
    }
}
