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
        private const string ItemListingAddress = "0x0000000000000000000000000000000000000005";
        private const string BazaarAddress = "0x0000000000000000000000000000000000000006";

        private static readonly Address Maintainer = MaintainerAddress.HexToAddress();
        private static readonly Address PartyA = PartyAAddress.HexToAddress();
        private static readonly Address PartyB = PartyBAddress.HexToAddress();
        private static readonly Address ItemListing = ItemListingAddress.HexToAddress();
        private static readonly Address BazaarContractAddress = BazaarAddress.HexToAddress();

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
        [InlineData(nameof(Bazaar.BalancePartyA), PartyAAddress, ulong.MinValue, ulong.MinValue)]
        [InlineData(nameof(Bazaar.BalancePartyA), PartyAAddress, 0, 0)]
        [InlineData(nameof(Bazaar.BalancePartyA), PartyAAddress, 100, 100)]
        [InlineData(nameof(Bazaar.BalancePartyA), PartyAAddress, 200, 100)]
        [InlineData(nameof(Bazaar.BalancePartyA), PartyAAddress, ulong.MaxValue, ulong.MaxValue)]
        [InlineData(nameof(Bazaar.BalancePartyB), PartyBAddress, ulong.MinValue, ulong.MinValue)]
        [InlineData(nameof(Bazaar.BalancePartyB), PartyBAddress, 0, 0)]
        [InlineData(nameof(Bazaar.BalancePartyB), PartyBAddress, 100, 100)]
        [InlineData(nameof(Bazaar.BalancePartyB), PartyBAddress, 200, 100)]
        [InlineData(nameof(Bazaar.BalancePartyB), PartyBAddress, ulong.MaxValue, ulong.MaxValue)]
        public void HasBalance_Buyer_Balance_Sufficient(string party, string partyAddress, ulong partyBalance, ulong itemPrice)
        {
            var bazaar = this.NewBazaar();

            this.mockPersistentState.Setup(s => s.GetUInt64(party)).Returns(partyBalance);

            Assert.True(bazaar.HasBalance(partyAddress.HexToAddress(), itemPrice));
        }

        [Theory]
        [InlineData(nameof(Bazaar.BalancePartyA), PartyAAddress, ulong.MinValue, ulong.MinValue + 1)]
        [InlineData(nameof(Bazaar.BalancePartyA), PartyAAddress, 0, 1)]
        [InlineData(nameof(Bazaar.BalancePartyA), PartyAAddress, 100, 200)]
        [InlineData(nameof(Bazaar.BalancePartyA), PartyAAddress, ulong.MaxValue - 1, ulong.MaxValue)]
        [InlineData(nameof(Bazaar.BalancePartyB), PartyBAddress, ulong.MinValue, ulong.MinValue + 1)]
        [InlineData(nameof(Bazaar.BalancePartyB), PartyBAddress, 0, 1)]
        [InlineData(nameof(Bazaar.BalancePartyB), PartyBAddress, 100, 200)]
        [InlineData(nameof(Bazaar.BalancePartyB), PartyBAddress, ulong.MaxValue - 1, ulong.MaxValue)]
        public void HasBalance_Buyer_Balance_Insufficient(string party, string partyAddress, ulong partyBalance, ulong itemPrice)
        {
            var bazaar = this.NewBazaar();

            this.mockPersistentState.Setup(s => s.GetUInt64(party)).Returns(partyBalance);

            Assert.False(bazaar.HasBalance(partyAddress.HexToAddress(), itemPrice));
        }

        [Fact]
        public void HasBalance_BuyerAddress_Not_PartyA_Or_PartyB()
        {
            var bazaar = this.NewBazaar();

            Assert.Throws<SmartContractAssertException>(() => bazaar.HasBalance(Address.Zero, 0));
        }

        [Fact]
        public void UpdateBalance_Fails_If_Sender_Not_ItemListing()
        {
            var bazaar = this.NewBazaar();

            this.mockPersistentState.Setup(s => s.GetAddress(nameof(Bazaar.CurrentItemListing))).Returns(ItemListing);
            this.mockContractState.Setup(s => s.Message.Sender).Returns(Address.Zero);

            Assert.Throws<SmartContractAssertException>(() => bazaar.UpdateBalance(PartyA, PartyB, 0));
        }

        [Theory]
        [MemberData(nameof(UpdateBalanceData))]
        public void UpdateBalance_Succeeds(string sellerParty, Address seller, ulong sellerPartyBalance, string buyerParty, Address buyer, ulong buyerPartyBalance, ulong price)
        {
            var bazaar = this.NewBazaar();

            // Ensure the party has enough balance
            this.mockPersistentState.Setup(s => s.GetUInt64(buyerParty)).Returns(buyerPartyBalance);

            // Item listing contract is making the call.
            this.mockContractState.Setup(s => s.Message.Sender).Returns(ItemListing);
            this.mockPersistentState.Setup(s => s.GetAddress(nameof(Bazaar.CurrentItemListing))).Returns(ItemListing);

            // Set the initial balances
            this.mockPersistentState.Setup(s => s.GetUInt64(sellerParty)).Returns(sellerPartyBalance);
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
            // Use a variable + value function here to emulate the behaviour of persistent state.
            var balancePartyA = 1UL;

            this.mockPersistentState.Setup(s => s.GetUInt64(nameof(Bazaar.BalancePartyA))).Returns(() => balancePartyA);
            this.mockPersistentState.Setup(s => s.SetUInt64(nameof(Bazaar.BalancePartyA), It.IsAny<ulong>())).Callback<string, ulong>((_, x) => balancePartyA = x);

            // Buyer starts with a balance equal to the item price.
            var balancePartyB = itemPrice;
            this.mockPersistentState.Setup(s => s.GetUInt64(nameof(Bazaar.BalancePartyB))).Returns(() => balancePartyB);
            this.mockPersistentState.Setup(s => s.SetUInt64(nameof(Bazaar.BalancePartyB), It.IsAny<ulong>())).Callback<string, ulong>((_, x) => balancePartyB = x);

            // Item listing contract is making the call.
            this.mockContractState.Setup(s => s.Message.Sender).Returns(ItemListing);
            this.mockPersistentState.Setup(s => s.GetAddress(nameof(Bazaar.CurrentItemListing))).Returns(ItemListing);

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
            this.mockPersistentState.Setup(s => s.GetUInt64(nameof(Bazaar.BalancePartyA))).Returns(1);

            // Buyer starts with a balance equal to the item price.
            this.mockPersistentState.Setup(s => s.GetUInt64(nameof(Bazaar.BalancePartyB))).Returns(itemPrice);

            // Item listing contract is making the call.
            this.mockContractState.Setup(s => s.Message.Sender).Returns(ItemListing);
            this.mockPersistentState.Setup(s => s.GetAddress(nameof(Bazaar.CurrentItemListing))).Returns(ItemListing);

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
        [InlineData(PartyAAddress, ulong.MinValue)]
        [InlineData(PartyBAddress, ulong.MinValue)]
        [InlineData(PartyAAddress, ulong.MaxValue)]
        [InlineData(PartyBAddress, ulong.MaxValue)]
        public void ListItem_Success_PartyA_PartyB(string sender, ulong itemPrice)
        {
            var senderAddress = sender.HexToAddress();
            var newContractAddress = NewContractAddress.HexToAddress();
            var itemName = "Test";

            var bazaar = this.NewBazaar();

            this.mockContractState.Setup(s => s.Message.Sender).Returns(senderAddress);

            this.mockInternalExecutor.Setup(i => i.Create<ItemListing>(this.mockContractState.Object, 0, It.IsAny<object[]>(), 0)).Returns(CreateResult.Succeeded(newContractAddress));

            bazaar.ListItem("Test", itemPrice);

            this.mockInternalExecutor.Verify(i => i.Create<ItemListing>(this.mockContractState.Object, 0,
                It.Is<object[]>(o =>
                    (string)o[0] == itemName &&
                    (ulong)o[1] == itemPrice &&
                    (Address)o[2] == senderAddress &&
                    (Address)o[3] == BazaarContractAddress &&
                    (Address)o[4] == PartyA &&
                    (Address)o[5] == PartyB
                ),
                0));

            this.mockPersistentState.Verify(s => s.SetAddress(nameof(Bazaar.CurrentItemListing), newContractAddress));

            this.mockPersistentState.Verify(s => s.SetUInt32(nameof(Bazaar.State), (uint)Bazaar.StateEnum.ItemListed));
        }

        public static IEnumerable<object[]> UpdateBalanceData
        {
            get
            {
                var result = new List<object[]>();

                // SellerParty, SellerPartyAddress, SellerPartyBalance, BuyerParty, BuyerPartyAddress, BuyerPartyBalance, SalePrice
                result.Add(new object[] { nameof(Bazaar.BalancePartyA), PartyA, 0, nameof(Bazaar.BalancePartyB), PartyB, 0, 0 });
                result.Add(new object[] { nameof(Bazaar.BalancePartyA), PartyA, 100, nameof(Bazaar.BalancePartyB), PartyB, 200, 150 });
                result.Add(new object[] { nameof(Bazaar.BalancePartyA), PartyA, 0, nameof(Bazaar.BalancePartyB), PartyB, uint.MaxValue, uint.MaxValue - 1, });
                result.Add(new object[] { nameof(Bazaar.BalancePartyA), PartyA, 100, nameof(Bazaar.BalancePartyB), PartyB, 200, 150 });

                result.Add(new object[] { nameof(Bazaar.BalancePartyB), PartyB, 0, nameof(Bazaar.BalancePartyA), PartyA, 0, 0 });
                result.Add(new object[] { nameof(Bazaar.BalancePartyB), PartyB, 100, nameof(Bazaar.BalancePartyA), PartyA, 200, 150 });
                result.Add(new object[] { nameof(Bazaar.BalancePartyB), PartyB, 0, nameof(Bazaar.BalancePartyA), PartyA, uint.MaxValue, uint.MaxValue - 1 });
                result.Add(new object[] { nameof(Bazaar.BalancePartyB), PartyB, 100, nameof(Bazaar.BalancePartyA), PartyA, 200, 150 });

                return result;
            }
        }

        public Bazaar NewBazaar()
        {
            this.mockContractState.Setup(s => s.Message.Sender).Returns(Maintainer);
            this.mockContractState.Setup(s => s.Message.ContractAddress).Returns(BazaarContractAddress);
            this.mockPersistentState.Setup(s => s.GetAddress(nameof(Bazaar.PartyA))).Returns(PartyA);
            this.mockPersistentState.Setup(s => s.GetUInt64(nameof(Bazaar.BalancePartyA))).Returns(0);
            this.mockPersistentState.Setup(s => s.GetAddress(nameof(Bazaar.PartyB))).Returns(PartyB);
            this.mockPersistentState.Setup(s => s.GetUInt64(nameof(Bazaar.BalancePartyB))).Returns(0);
            return new Bazaar(this.mockContractState.Object, PartyA, 0, PartyB, 0);
        }
    }
}
