using Moq;
using Stratis.SmartContracts;
using Stratis.SmartContracts.CLR;
using System;
using System.Collections.Generic;
using System.Reflection;
using Xunit;

namespace BazaarItemListing.Tests
{
    public class ItemListingTests
    {
        private const string MaintainerAddress = "0x0000000000000000000000000000000000000001";
        private const string PartyAAddress = "0x0000000000000000000000000000000000000002";
        private const string PartyBAddress = "0x0000000000000000000000000000000000000003";
        private const string ParentContractAddress = "0x0000000000000000000000000000000000000004";
        private static readonly Address Maintainer = MaintainerAddress.HexToAddress();
        private static readonly Address PartyA = PartyAAddress.HexToAddress();
        private static readonly Address PartyB = PartyBAddress.HexToAddress();
        private static readonly Address ParentContract = ParentContractAddress.HexToAddress();
        private readonly Mock<ISmartContractState> mockContractState;
        private readonly Mock<IPersistentState> mockPersistentState;
        private readonly Mock<IInternalTransactionExecutor> mockInternalExecutor;
        private readonly Mock<IContractLogger> mockContractLogger;

        public ItemListingTests()
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
        [InlineData(nameof(ItemListing.State))]
        [InlineData(nameof(ItemListing.Seller))]
        [InlineData(nameof(ItemListing.Buyer))]
        [InlineData(nameof(ItemListing.ParentContract))]
        [InlineData(nameof(ItemListing.ItemName))]
        [InlineData(nameof(ItemListing.ItemPrice))]
        [InlineData(nameof(ItemListing.PartyA))]
        [InlineData(nameof(ItemListing.PartyB))]
        public void Property_Setter_Is_Private(string propertyName)
        {
            Type type = typeof(ItemListing);

            PropertyInfo property = type.GetProperty(propertyName);

            Assert.True(property.SetMethod.IsPrivate);
        }

        [Fact]
        public void Constructor_Sets_Initial_Values()
        {
            var itemName = "Test";
            var itemPrice = 1UL;

            this.mockContractState.Setup(s => s.Message.Sender).Returns(Maintainer);

            var itemListing = new ItemListing(this.mockContractState.Object, itemName, itemPrice, PartyA, ParentContract, PartyA, PartyB);

            this.mockPersistentState.Verify(s => s.SetAddress(nameof(ItemListing.PartyA), PartyA));
            this.mockPersistentState.Verify(s => s.SetAddress(nameof(ItemListing.PartyB), PartyB));
            this.mockPersistentState.Verify(s => s.SetAddress(nameof(ItemListing.Seller), PartyA));
            this.mockPersistentState.Verify(s => s.SetAddress(nameof(ItemListing.ParentContract), ParentContract));
            this.mockPersistentState.Verify(s => s.SetString(nameof(ItemListing.ItemName), itemName));
            this.mockPersistentState.Verify(s => s.SetUInt64(nameof(ItemListing.ItemPrice), itemPrice));
            this.mockPersistentState.Verify(s => s.SetUInt32(nameof(Bazaar.State), (uint)ItemListing.StateType.ItemAvailable));
        }

        [Fact]
        public void BuyItem_Fails_If_Buyer_Is_Seller()
        {
            var seller = PartyA;

            this.mockContractState.Setup(s => s.Message.Sender).Returns(ParentContract);

            var itemListing = new ItemListing(this.mockContractState.Object, "Test", 1, seller, ParentContract, PartyA, PartyB);

            // Make the sender the seller.
            this.mockContractState.Setup(s => s.Message.Sender).Returns(seller);

            Assert.Throws<SmartContractAssertException>(() => itemListing.BuyItem());
        }

        [Fact]
        public void BuyItem_Fails_If_Buyer_Balance_Insufficient()
        {
            var seller = PartyA;
            var buyer = PartyB;
            var itemPrice = 1UL;

            this.mockContractState.Setup(s => s.Message.Sender).Returns(ParentContract);

            var itemListing = new ItemListing(this.mockContractState.Object, "Test", itemPrice, seller, ParentContract, PartyA, PartyB);

            this.mockInternalExecutor.Setup(c => c.Call(It.IsAny<ISmartContractState>(), ParentContract, 0, nameof(Bazaar.HasBalance), new object[] { buyer, itemPrice }, 0))
                .Returns(TransferResult.Transferred(false));

            this.mockContractState.Setup(s => s.Message.Sender).Returns(buyer);

            Assert.Throws<SmartContractAssertException>(() => itemListing.BuyItem());

            this.mockInternalExecutor.Verify(c => c.Call(It.IsAny<ISmartContractState>(), ParentContract, 0, nameof(Bazaar.HasBalance), new object[] { buyer, itemPrice }, 0), Times.Once);
        }

        [Fact]
        public void BuyItem_Succeeds()
        {
            var seller = PartyA;
            var buyer = PartyB;
            var itemPrice = 1UL;

            this.mockContractState.Setup(s => s.Message.Sender).Returns(ParentContract);

            var itemListing = new ItemListing(this.mockContractState.Object, "Test", itemPrice, seller, ParentContract, PartyA, PartyB);

            this.mockInternalExecutor.Setup(c => c.Call(It.IsAny<ISmartContractState>(), ParentContract, 0, nameof(Bazaar.HasBalance), new object[] { buyer, itemPrice }, 0))
                .Returns(TransferResult.Transferred(true));

            this.mockContractState.Setup(s => s.Message.Sender).Returns(buyer);

            this.mockInternalExecutor.Verify(c => c.Call(It.IsAny<ISmartContractState>(), ParentContract, 0, nameof(Bazaar.HasBalance), new object[] { buyer, itemPrice }, 0), Times.Once);
            this.mockInternalExecutor.Verify(c => c.Call(It.IsAny<ISmartContractState>(), ParentContract, 0, nameof(Bazaar.UpdateBalance), new object[] { seller, buyer, itemPrice }, 0), Times.Once);
            this.mockPersistentState.Verify(s => s.SetUInt32(nameof(Bazaar.State), (uint)ItemListing.StateType.ItemSold));
        }

        public ItemListing NewItemListing()
        {
            this.mockContractState.Setup(s => s.Message.Sender).Returns(ParentContract);

           return new ItemListing(this.mockContractState.Object, "Test", 1, PartyA, ParentContract, PartyA, PartyB);
        }
    }
}
