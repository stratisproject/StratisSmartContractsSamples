using Moq;
using Stratis.SmartContracts;
using Stratis.SmartContracts.CLR;
using System;
using System.Linq;
using System.Reflection;
using Xunit;

namespace DefectiveComponentCounterContract.Tests
{
    public class DefectiveComponentCounterTests
    {
        private const string Manufacturer = "0x0000000000000000000000000000000000000001";

        private static readonly Address ManufacturerAddress = Manufacturer.HexToAddress();

        private readonly Mock<ISmartContractState> mockContractState;
        private readonly Mock<IPersistentState> mockPersistentState;
        private readonly Mock<IInternalTransactionExecutor> mockInternalExecutor;
        private readonly Mock<IContractLogger> mockContractLogger;

        public DefectiveComponentCounterTests()
        {
            this.mockContractLogger = new Mock<IContractLogger>();
            this.mockPersistentState = new Mock<IPersistentState>();
            this.mockInternalExecutor = new Mock<IInternalTransactionExecutor>();
            this.mockContractState = new Mock<ISmartContractState>();
            this.mockContractState.Setup(s => s.PersistentState).Returns(this.mockPersistentState.Object);
            this.mockContractState.Setup(s => s.ContractLogger).Returns(this.mockContractLogger.Object);
            this.mockContractState.Setup(s => s.InternalTransactionExecutor).Returns(this.mockInternalExecutor.Object);

            // Mock serializer behaviour.
            this.mockContractState.Setup(s => s.Serializer.ToUInt32(It.IsAny<byte[]>())).Returns((byte[] val) => BitConverter.ToUInt32(val));
        }

        [Theory]
        [InlineData(nameof(DefectiveComponentCounter.State))]
        [InlineData(nameof(DefectiveComponentCounter.Manufacturer))]
        [InlineData(nameof(DefectiveComponentCounter.Total))]
        public void Property_Setter_Is_Private(string propertyName)
        {
            Type type = typeof(DefectiveComponentCounter);

            PropertyInfo property = type.GetProperty(propertyName);

            Assert.True(property.SetMethod.IsPrivate);
        }

        [Fact]
        public void Constructor_Sets_Initial_Values()
        {
            this.mockContractState.Setup(s => s.Message.Sender).Returns(ManufacturerAddress);

            var arr = ToByteArray(new uint[] { 1, 2, 3 });

            var defectiveComponentsCounter = new DefectiveComponentCounter(this.mockContractState.Object, arr);

            this.mockPersistentState.Verify(s => s.SetAddress(nameof(DefectiveComponentCounter.Manufacturer), ManufacturerAddress), Times.Once);
            this.mockPersistentState.Verify(s => s.SetArray("DefectiveComponentsCount", new uint[12] { 1, 2, 3, 0, 0, 0, 0, 0, 0, 0, 0, 0 }), Times.Once);
            this.mockPersistentState.Verify(s => s.SetUInt32(nameof(DefectiveComponentCounter.Total), 0), Times.Once);
            this.mockPersistentState.Verify(s => s.SetUInt32(nameof(DefectiveComponentCounter.State), (uint)DefectiveComponentCounter.StateType.Create), Times.Once);
        }

        private static byte[] ToByteArray(uint[] arr)
        {
            return arr.Select(i => BitConverter.GetBytes(i)).SelectMany(x => x).ToArray();
        }

        [Theory]
        [InlineData(new uint[] { })]
        [InlineData(new uint[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12 })]
        [InlineData(new uint[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13 })]
        [InlineData(new uint[] { 1, 2, 3, 4, 5 })]
        [InlineData(new uint[] { 4294967295 })] // uint.MaxValue
        public void Constructor_Sets_Initial_Values_ComponentCounts_Success(uint[] componentCounts)
        {
            this.mockPersistentState.Setup(s => s.GetAddress(nameof(DefectiveComponentCounter.Manufacturer))).Returns(ManufacturerAddress);
            this.mockContractState.Setup(s => s.Message.Sender).Returns(ManufacturerAddress);

            var arr = ToByteArray(componentCounts);

            var defectiveComponentsCounter = new DefectiveComponentCounter(this.mockContractState.Object, arr);

            var persistedComponents = ToFixedWidthArray(componentCounts);
            this.mockPersistentState.Verify(s => s.SetArray("DefectiveComponentsCount", persistedComponents), Times.Once);
        }

        [Theory]
        [InlineData(new byte[sizeof(uint) - 1] { 0x00, 0x00, 0x00 })] // Invalid width
        [InlineData(new byte[sizeof(uint) + 1] { 0x00, 0x00, 0x00, 0x00, 0x00 })] // Invalid width
        public void Constructor_Sets_Initial_Values_ComponentCounts_Failure(byte[] componentCounts)
        {
            this.mockPersistentState.Setup(s => s.GetAddress(nameof(DefectiveComponentCounter.Manufacturer))).Returns(ManufacturerAddress);
            this.mockContractState.Setup(s => s.Message.Sender).Returns(ManufacturerAddress);

            Assert.Throws<SmartContractAssertException>(() => new DefectiveComponentCounter(this.mockContractState.Object, componentCounts));
        }

        [Fact]
        public void ComputeTotal_Fails_Sender_Not_Manufacturer()
        {
            this.mockPersistentState.Setup(s => s.GetAddress(nameof(DefectiveComponentCounter.Manufacturer))).Returns(ManufacturerAddress);
            this.mockContractState.Setup(s => s.Message.Sender).Returns(ManufacturerAddress);

            var defectiveComponentsCounter = new DefectiveComponentCounter(this.mockContractState.Object, new byte[] { });

            this.mockContractState.Setup(s => s.Message.Sender).Returns(Address.Zero);

            Assert.Throws<SmartContractAssertException>(() => defectiveComponentsCounter.ComputeTotal());
        }

        [Theory]
        [InlineData(new uint[] { 1, 2, 3, 4, 5 }, 15)]
        [InlineData(new uint[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12 }, 78)]
        public void ComputeTotal_Computes_Correctly(uint[] components, uint expectedTotal)
        {
            components = ToFixedWidthArray(components);

            this.mockPersistentState.Setup(s => s.GetAddress(nameof(DefectiveComponentCounter.Manufacturer))).Returns(ManufacturerAddress);
            this.mockContractState.Setup(s => s.Message.Sender).Returns(ManufacturerAddress);

            var defectiveComponentsCounter = new DefectiveComponentCounter(this.mockContractState.Object, ToByteArray(components));
            this.mockPersistentState.Setup(s => s.GetArray<uint>("DefectiveComponentsCount")).Returns(components);

            defectiveComponentsCounter.ComputeTotal();

            this.mockPersistentState.Verify(s => s.SetUInt32(nameof(DefectiveComponentCounter.Total), expectedTotal));
        }

        [Theory]
        [InlineData(new uint[] { 1, uint.MaxValue })] // 1, uint.MaxValue
        public void ComputeTotal_Overflow_Throws_Exception(uint[] components)
        {
            components = ToFixedWidthArray(components);

            this.mockPersistentState.Setup(s => s.GetAddress(nameof(DefectiveComponentCounter.Manufacturer))).Returns(ManufacturerAddress);
            this.mockContractState.Setup(s => s.Message.Sender).Returns(ManufacturerAddress);

            var defectiveComponentsCounter = new DefectiveComponentCounter(this.mockContractState.Object, new byte[] { });
            this.mockPersistentState.Setup(s => s.GetArray<uint>("DefectiveComponentsCount")).Returns(components);

            Assert.Throws<OverflowException>(() => defectiveComponentsCounter.ComputeTotal());
        }

        [Theory]
        [InlineData(uint.MaxValue)]
        [InlineData(uint.MinValue)]
        public void Set_DefectiveComponentCount_Success(uint componentCount)
        {
            this.mockPersistentState.Setup(s => s.GetAddress(nameof(DefectiveComponentCounter.Manufacturer))).Returns(ManufacturerAddress);
            this.mockContractState.Setup(s => s.Message.Sender).Returns(ManufacturerAddress);

            var defectiveComponentsCounter = new DefectiveComponentCounter(this.mockContractState.Object, new byte[] { });

            var startingCounts = new uint[12]
            {
                1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1
            };

            // Check every month.
            for (uint i = 0; i < 12; i++)
            {
                this.mockPersistentState.Setup(s => s.GetArray<uint>("DefectiveComponentsCount"))
                    .Returns(startingCounts);

                var newCounts = new uint[12];
                startingCounts.CopyTo(newCounts, 0);

                newCounts[i] = componentCount;

                defectiveComponentsCounter.SetDefectiveComponentsCount(i, componentCount);

                this.mockPersistentState.Verify(s => s.SetArray("DefectiveComponentsCount", newCounts), Times.Once);

                // Clear invocations.
                this.mockPersistentState.Invocations.Clear();
            }
        }

        [Fact]
        public void Set_DefectiveComponentCount_Failure_If_12()
        {
            this.mockPersistentState.Setup(s => s.GetAddress(nameof(DefectiveComponentCounter.Manufacturer))).Returns(ManufacturerAddress);
            this.mockContractState.Setup(s => s.Message.Sender).Returns(ManufacturerAddress);

            var defectiveComponentsCounter = new DefectiveComponentCounter(this.mockContractState.Object, new byte[] { });

            this.mockPersistentState.Invocations.Clear();

            Assert.Throws<SmartContractAssertException>(() => defectiveComponentsCounter.SetDefectiveComponentsCount(12, 1));

            this.mockPersistentState.Verify(s => s.SetArray("DefectiveComponentsCount", It.IsAny<uint[]>()), Times.Never);
        }

        [Fact]
        public void Get_DefectiveComponentCount_Success()
        {
            this.mockPersistentState.Setup(s => s.GetAddress(nameof(DefectiveComponentCounter.Manufacturer))).Returns(ManufacturerAddress);
            this.mockContractState.Setup(s => s.Message.Sender).Returns(ManufacturerAddress);

            var defectiveComponentsCounter = new DefectiveComponentCounter(this.mockContractState.Object, new byte[] { });

            var startingCounts = new uint[12]
            {
                1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12
            };

            this.mockPersistentState.Setup(s => s.GetArray<uint>("DefectiveComponentsCount"))
                .Returns(startingCounts);

            // Check every month.
            for (uint i = 0; i < 12; i++)
            {
                Assert.Equal(startingCounts[i], defectiveComponentsCounter.GetDefectiveComponentsCount(i));

                this.mockPersistentState.Verify(s => s.GetArray<uint>("DefectiveComponentsCount"), Times.Once);

                // Clear invocations.
                this.mockPersistentState.Invocations.Clear();
            }
        }

        [Fact]
        public void Get_DefectiveComponentCount_Failure_If_12()
        {
            this.mockPersistentState.Setup(s => s.GetAddress(nameof(DefectiveComponentCounter.Manufacturer))).Returns(ManufacturerAddress);
            this.mockContractState.Setup(s => s.Message.Sender).Returns(ManufacturerAddress);

            var defectiveComponentsCounter = new DefectiveComponentCounter(this.mockContractState.Object, new byte[] { });

            this.mockPersistentState.Invocations.Clear();

            Assert.Throws<SmartContractAssertException>(() => defectiveComponentsCounter.GetDefectiveComponentsCount(12));

            this.mockPersistentState.Verify(s => s.GetArray<uint>("DefectiveComponentsCount"), Times.Never);
        }

        private static uint[] ToFixedWidthArray(uint[] items, int width = 12)
        {
            var tempComponents = new uint[width];
            items.Take(width).ToArray().CopyTo(tempComponents, 0);

            return tempComponents;
        }
    }
}
