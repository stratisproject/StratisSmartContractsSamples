## Inspiration

The Sale Deed Registry, A based smart contract application was inspired by the "Indian" government. Currently, most of these aspects are done with the combination of tons of paperwork and some electronic. Though few states went ahead and implemented a fully electronic way of filing the submission for the "Sale Deed", however, it's taking a significant amount of time for the whole process to complete.

## Realtime Testing and Getting your hands dirty

Sale Deed Registry Console App Testing

https://github.com/ranjancse26/Stratis-SaleDeedRegistry/wiki/Sale-Deed-Registry---Console-Testing-Instructions

Sale Deed Registry Windows Form App Testing

https://github.com/ranjancse26/Stratis-SaleDeedRegistry/wiki/Sale-Deed-Registry---Windows-Form-Testing-Instructions

## What it does

The Sale Deed Registry as the name says, it's responsible for handling the sale deed fulfillment between the seller and the buyer. The main actors of the system are - Buyer, Seller, and Payee.

## How I built it

The heart of the "Sale Deed Registry" is based on the Stratis Smart Contract. 

In addition to maintaining the Sale Deed record or information about the property as an asset within the blockchain via Smart Contract, it's is also responsible for handling the fee payment and other necessary sale deed verification checks such as "Encumbrance" clearance, etc. The system is designed with the contract as a core requirement and the actors just operating the contract with the required state being maintained within the contract. All operations are executed or handled based on the qualified state.

The following are the "Sale Deed Registry" Smart Contract States

<ol>
<li>Init Application</li>
<li>State Review</li>
<li>Complete Review</li>
<li>Reject Application</li>
<li>ReApply</li>
<li>Pay Application Transfer Fee</li>
<li>Transfer Ownership</li>
</ol>

Here's the high-level overview of the Sale Deed Application Process coded on a Console App to demonstrate the end to end workflow.

The following is the Sale Deed Registry Console App code snippet. It shows the high-level actors and their interactions with the SaleDeedRegistry Stratis Smart Contract.

```
        /// <summary>
        /// The SaleRegistry Console Client Demonstrating the overall
        /// Workflow or Process of obtaining the ownership of the property.
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            try
            {
                string assetId = UniqueIdHelper.GenerateId();

                Payee payee = new Payee();
                Supervisor supervisor = new Supervisor(assetId);
                PropertyBuyer propertyBuyer = new PropertyBuyer();
                PropertySeller propertySeller = new PropertySeller();

                System.Console.WriteLine("Initiating the " +
                    "Sale Deed Application Process");

                supervisor.InitApplication().Wait();
                supervisor.StartTheReviewProcess().Wait();
                supervisor.CompleteTheReviewProcess().Wait();

                propertyBuyer.PayTransferFee(payee.GetPayee(), assetId).Wait();
                supervisor.TransferOwnership(propertySeller.GetOwnerAddress(),
                    propertyBuyer.GetBuyerAddress()).Wait();

                System.Console.WriteLine("Completed executing the " +
                    "Sale Deed Application Process");
            }
            catch(Exception ex)
            {
                System.Console.WriteLine(ex.ToString());
            }

            System.Console.ReadLine();
        }
```

Hash
```
7b4e7c5c75a20d0792500f19e72f763565a08cc25b04a61c8ec6d5ed4f971805
```

ByteCode
```
4D5A90000300000004000000FFFF0000B800000000000000400000000000000000000000000000000000000000000000000000000000000000000000800000000E1FBA0E00B409CD21B8014CCD21546869732070726F6772616D2063616E6E6F742062652072756E20696E20444F53206D6F64652E0D0D0A2400000000000000504500004C010200D62684F20000000000000000E00022200B013000001200000002000000000000EA310000002000000040000000000010002000000002000004000000000000000400000000000000006000000002000000000000030040850000100000100000000010000010000000000000100000000000000000000000983100004F000000000000000000000000000000000000000000000000000000004000000C0000007C3100001C0000000000000000000000000000000000000000000000000000000000000000000000000000000000000000200000080000000000000000000000082000004800000000000000000000002E74657874000000F0110000002000000012000000020000000000000000000000000000200000602E72656C6F6300000C000000004000000002000000140000000000000000000000000000400000420000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000CC3100000000000048000000020005008C230000F00D000001000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000003E0203280500000A020428030000062A4602280600000A72010000706F0700000A2A4A02280600000A7201000070036F0800000A2A7202280600000A721B000070038C08000001280900000A6F0A00000A2A7602280600000A721B000070038C08000001280900000A046F0B00000A2A5E02280600000A723500007003280900000A6F0C00000A2A6202280600000A723500007003280900000A046F0D00000A2A5E02280600000A726500007003280900000A6F0700000A2A6202280600000A726500007004280900000A036F0800000A2A5E02280600000A728B00007003280900000A6F0E00000A2A6202280600000A728B00007003280900000A046F0F00000A2A9602030528050000060203052809000006020517280B000006021772B1000070280D0000062A000000133003002000000001000011021200FE15040000021200037D090000041200047D0A00000406280100002B2A133003002C000000020000110205280A0000060A020617FE0172C9000070281100000A020518280B000006021872E7000070280D0000062A1330030064000000030000110205280A0000060A020528080000060B020703281200000A72C9000070281100000A02057201010070281300000A72C9000070281100000A020618FE0172C9000070281100000A0205172807000006020519280B00000602197203010070280D0000062A5602031C280B000006021C7223010070280D0000062A56020317280B00000602177235010070280D0000062A133003005E00000002000011020E04166AFE03724B010070281100000A0202280200000604281200000A7297010070281100000A0205280A0000060A020619FE0172C5010070281100000A02040E04281400000A2602051A280B000006021A7215020070280D0000062A00001330030097000000040000110205280A0000060A020304281500000A7239020070281100000A02061AFE017287020070281100000A02057201010070281300000A72CD020070281100000A020528060000060B02077205030070281100000A0204052809000006021202FE15050000021202057D0D0000041202037D0B0000041202047D0C00000408280200002B02051B280B000006021B7235030070280D0000062A0042534A4201000100000000000C00000076342E302E33303331390000000005006C0000005C040000237E0000C80400008C04000023537472696E6773000000005409000048030000235553009C0C0000100000002347554944000000AC0C00004401000023426C6F620000000000000002000001571DA201090A000000FA013300160000010000000E000000050000000D0000001300000023000000150000000700000004000000040000000100000001000000020000000100000002000000030000000200000000001C020100000000000600A1011B030600D1011B0306008D0108030F003B0300000A00C101FA030A001804FA030A000A01FA030A00DF03FA0306004A023E02060000013E020A003101FA03060011023E0206003F043E020A004604FA030000000015000000000001000100010010002604000019000100010002010000DC0000002500010014000A0110009C0200002900090014000A0110008C02000029000B00140006061E00BE0056809A00C1005680EF03C10056805D04C10056807E01C1005680AE00C1005680A500C10056809100C10006007201BE0006007102C50006004502C80006008902C80006004B00C50050200000000086180203CC00010060200000000086087303D400030072200000000086088403D900030085200000000086003D00DF000400A2200000000086004800E5000500C0200000000086005B0039000700D82000000000810071003E000800F120000000008600C4021B000A000921000000008100D502E5000B002221000000008600560144000D003A21000000008100670149000E0053210000000086006102EC0010007C21000000008100F501F5001300A8210000000086006003EC001500E0210000000086004A03EC00180050220000000086004F02FC001B0066220000000086006904FC001C007C22000000008600BE0001011D00E822000000008600A902EC002100000001001E0100000200950300000100EF0100000100B70300000100B70300000200530000000100530000000100530000000200870000000100530000000100E70300000200530000000100530000000100530000000200780100000100A20300000200C40300000300530000000100EE00000002007D0200000100A20300000200C40300000300530000000100A20300000200C40300000300530000000100530000000100530000000100C40300000200950300000300530000000400D80000000100E60200000200F402000003005300090002030100110002030600190002030A002900020306003100020310003100420116005900D1031B005900DC032100610011042800590004022E0059000E02330059002E023900590036023E0059000100440059000B00490031001802540031005604640041007104700061007D0478003100BB027E0041007D047000090008009B0009000C00A00009001000A50009001400AA0009001800AF0009001C00B40009002000B9002E000B0010012E00130019012E001B00380143002300A0004F0060006A00860002000100000088030B010200020003000100030003000480000000000000000000000000000000001804000004000000000000000000000092002600000000000100020001000000000000000000FA030000000003000200040002000500020021005B0021008D0000000047657455496E7433320053657455496E743332003C4D6F64756C653E0076616C75655F5F0053797374656D2E507269766174652E436F72654C696200476574417373657449640053657441737365744964006173736574496400476574456E63756D6272616E6365436C656172656400536574456E63756D6272616E6365436C6561726564006973436C65617265640052656A6563746564004E6F745374617274656400417070726F76656400506169645472616E73666572466565005061794170706C69636174696F6E5472616E73666572466565006665650050726F70657274795374617465547970650070726F70657274795374617465547970650056616C7565547970650049536D617274436F6E7472616374537461746500736D617274436F6E74726163745374617465004950657273697374656E745374617465006765745F50657273697374656E7453746174650047657450726F706572747953746174650053657450726F7065727479537461746500737461746500526576696577436F6D706C6574650044656275676761626C6541747472696275746500436F6D70696C6174696F6E52656C61786174696F6E73417474726962757465004465706C6F794174747269627574650052756E74696D65436F6D7061746962696C6974794174747269627574650076616C756500446F53746174654C6F6767696E6700476574537472696E6700536574537472696E67004C6F6700536D617274436F6E74726163742E646C6C00476574426F6F6C00536574426F6F6C0053797374656D0046726F6D00456E756D0052656A6563744170706C69636174696F6E00496E69744170706C69636174696F6E004465736372697074696F6E006465736372697074696F6E00546F0050757263686173654C6F67496E666F0053746174654C6F67496E666F005472616E736665724F776E657273686970005472616E736665720047657450726F70657274794F776E65720053657450726F70657274794F776E65720070726F70657274794F776E65720070726F70657274794275796572002E63746F720053797374656D2E446961676E6F73746963730053797374656D2E52756E74696D652E436F6D70696C6572536572766963657300446562756767696E674D6F64657300436F6D706C65746552657669657750726F6365737300537461727452657669657750726F63657373006765745F506179656541646472657373007365745F506179656541646472657373007061796565416464726573730070726F70657274794F776E657241646472657373006F776E6572416464726573730062757965724164647265737300476574416464726573730053657441646472657373006164647265737300496E50726F677265737300537472617469732E536D617274436F6E74726163747300466F726D617400536D617274436F6E74726163740053616C65446565645265676973747279436F6E7472616374004F626A65637400495472616E73666572526573756C740041737365727400556E6465725265766965770052654170706C79006F705F457175616C697479006F705F496E657175616C69747900000019500061007900650065004100640064007200650073007300001941007300730065007400490064005B007B0030007D005D00002F45006E00630075006D006200720061006E006300650043006C00650061007200650064005B007B0030007D005D000025500072006F00700065007200740079004F0077006E00650072005B007B0030007D005D000025500072006F0070006500720074007900530074006100740065005B007B0030007D005D00001749006E002D00500072006F0067007200650073007300011D41007300730065007200740020006600610069006C00650064002E00001955006E0064006500720020005200650076006900650077000001001F520065007600690065007700200043006F006D0070006C006500740065000011520065006A0065006300740065006400001549006E00500072006F0067007200650073007300004B4600650065002000630061006E006E006F00740020006200650020006C0065007300730020007400680061006E0020006F007200200065007100750061006C00200074006F0020003000002D500061007900650065002000610064006400720065007300730020006D00690073006D006100740063006800004F54006800650020005300740061007400650020006900730020006E006F007400200069006E00200052006500760069006500770043006F006D0070006C0065007400650020006D006F00640065000023500061006900640020005400720061006E0073006600650072002000460065006500004D54006800650020006F0077006E0065007200200061006E00640020007400680065002000620075007900650072002000630061006E006E006F0074002000620065002000730061006D00650000455300740061007400650020006E006F007400200065007100750061006C00200074006F00200050006100690064005400720061006E0073006600650072004600650065000037410073007300650074004900640020006900730020006E006F007400200073006500740020006F007200200065006D00700074007900002F45006E00630075006D006200720061006E006300650020006E006F007400200063006C0065006100720065006400001141007000700072006F00760065006400000032D6285EB201D24987F58EE3617460B70004200101080320000105200101111105200101121D042000122D05200111210E062002010E11210500020E0E1C0420010E0E052002010E0E042001020E052002010E02042001090E052002010E09040701111006300101011E00040A0111100307010905200201020E0507020911210700020211211121050002020E0E072002123911210B06070309021114040A011114087CEC85D7BEA7798E04000000000401000000040200000004030000000404000000040500000004060000000206090306110C02060E0306112107200201121D112104200011210520010111210520010E11210620020111210E08200301112111210E06200201110C0E042001010E09200401112111210E0B04280011210801000800000000001E01000100540216577261704E6F6E457863657074696F6E5468726F77730108010002000000000000000000000000000000000000000010000000000000000000000000000000C03100000000000000000000DA310000002000000000000000000000000000000000000000000000CC310000000000000000000000005F436F72446C6C4D61696E006D73636F7265652E646C6C0000000000FF250020001000000000000000000000000000000000003000000C000000EC3100000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
```

### Sale Deed Registry Solution

Below is the snapshot of the SaleDeedRegistry .NET Solution. It's coded using VS 2019.

![alt text](https://raw.githubusercontent.com/ranjancse26/Stratis-SaleDeedRegistry/master/SaleDeedRegistry-Images/SaleDeedRegistry-Solution-Collapsed.png "SaleDeedRegistry Solution")

**SaleDeedRegistry.ApiClient** - A lightweight library that deals with the SaleDeedRegistry Smart Contract communication via the Swagger based contract API. Any application that consumes the contract will make use of this library and interact or programmatically execute the contract operations with ease.

**SaleDeedRegistry.Console** - It's a console app or a sample that's used for demonstrating the SaleDeedRegistry contract capabilities. The console app is designed with minimal code by fully utilizing the custom libraries coded as part of the solution.

**SaleDeedRegistry.Contract** - The Stratis based Smart Contract Library consists of the SaleDeedRegistry contract.

**SaleDeedRegistry.Desktop** - The Sale Deed Registry Desktop Application that deals with the Property Management and Sale Deed Registration. The Government Supervisor who's responsible for handling the Sale Deed and property ownership will make use of this desktop app and perform all the SaleDeedRegistry Smart Contract operations with ease.

**SaleDeedRegistry.Domain** - The Domain library consists of all the SaleDeedRegistry domain entities. The domain gives a complete overview of the domain entities that are being used to fulfill the Sale Deed transaction.

**SaleDeedRegistry.Lib** - It's a class library, the core or the heart of our solution. This library has all the required or necessary things for interacting with the SaleDeedRegistry smart contract. Actors and those responsible for performing the contract operation. It also consists of a helper class that deals with the generation of unique id and reading the configuration items.

**SaleDeedRegistry.Tests** - The SaleDeedRegistry smart contract unit test project coding using xunit and moq. It has all the facts for testing the contract functionalities. It's a complete unit test solution that executes all of the contract operations and verifies the expectation.

**SaleDeedRegistry.Wallet** - The Stratis Wallet Solution, it's a Windows Form desktop solution that helps one to perform the Stratis Wallet operations with ease via the GUI.

### State-Based Smart Contract

We are dealing with the property sale deed transaction and it involves the government officials or supervisors to handle necessary aspects like the review, confirm and handle application fee payment and finally, it will lead to the transfer of ownership. It's a transaction between the property buyer and seller or the owner.

The SaleDeedRegistry smart contract a state-driven one. The contact operations are executed by verifying the application state. The application also utilizes and navigates between the state and performs all the Sale Deed related contract operations with ease. At a given point for time, based on the Asset that involves in the Sale Deed transaction, we'll always have a specific application state.

![alt text](https://raw.githubusercontent.com/ranjancse26/Stratis-SaleDeedRegistry/master/SaleDeedRegistry-Images/SaleDeedApplicationProcess.png "Sale Deed Application Process")

![alt text](https://raw.githubusercontent.com/ranjancse26/Stratis-SaleDeedRegistry/master/SaleDeedRegistry-Images/SaleDeedApplicationStates.png "Sale Deed Application State")

Below are the property states defined within the contract.

```
    /// <summary>
    /// Set the appropriate property state
    /// </summary>
    public enum PropertyStateType : uint
    {
        NotStarted = 0,
        InProgress = 1,
        UnderReview = 2,
        ReviewComplete = 3,
        PaidTransferFee = 4,
        Approved = 5,
        Rejected = 6
    }
```

### Design Principles / Design Patterns

Here's you will see the details about the well-known design patterns and principles implemented in the SaleDeedRegistry solution. 

1) **Domain Driven Design (DDD)** - The Sale Deed Registry functionalities are coded with the DDD in mind. We have created necessary domain entities that will help in fulfilling the Sale Deed handling with ease. The DDD allows one to easily enhance or extend the solution.

2) **SOLID principles** - The SaleDeedRegistry solution is coded with the SOLID principles in mind. The libraries and applications are coded with the SOLID principles to take advantage of the code readability, easy to extend and fix issues. 

3) **Facade Pattern** - We have a Sale Deed Facade responsible for handling or executing the SaleDeedRegistry contract operations. Internally, it's making use of the Stratis Contract Swagger API. The Facade simplifies or hides the contract calls. In addition, it also deals with the Stratis Receipt API calls and hides all the complexities of dealing with the subsystem interaction. The Facade lets the consumers to easily interact with the SaleDeedRegistry contract. It also helps the developers to extend the solution with ease.

4) **Command Pattern** - The Command Pattern is being implemented for interacting with the SaleDeedRegistry Smart Contract. The contract is driven by the State and hence the command pattern suits them best in performing the contract actions. Basically, we are encapsulating the Sale Deed Request object and then executing it to perform a specific operation based on a given state.

### Compilation of the Sale Deed Registry Smart Contract

We'll be using the Stratis.SmartContracts.Tools.Sct.exe tool to validate and generate the Smart Contract Bytes.

![alt text](https://raw.githubusercontent.com/ranjancse26/Stratis-SaleDeedRegistry/master/SaleDeedRegistry-Images/SaleDeedRegistry-Compile-Bytes.png "Smart Contract Compile")

### Sale Deed Registry Smart Contract Actors

The high-level actors of the Sale Deed Registry Contract Systems are 

<ol>
<li>Payee</li>
<li>Property Buyer</li>
<li>Property Seller</li>
<li>Supervisor</li>
</ol>

![alt text](https://raw.githubusercontent.com/ranjancse26/Stratis-SaleDeedRegistry/master/SaleDeedRegistry-Images/SaleDeed-ConsoleApp-Actors.png "Actors")

### Unit Tests

The SaleDeedRegistry smart contract functionalities are unit tested using xunit and moq libraries. 

![alt text](https://raw.githubusercontent.com/ranjancse26/Stratis-SaleDeedRegistry/master/SaleDeedRegistry-Images/UnitTests.png "Unit Tests")

The Unit Test consists of a Test Class - "SaleDeedContractTest" having a collection of Facts that's responsible for performing the contract operation and verifies or validates the results.

```
        public SaleDeedContractTest()
        {
            transferResult = new Mock<ITransferResult>();
            mockContractLogger = new Mock<IContractLogger>();
            mockPersistentState = new Mock<IPersistentState>();
            mockContractState = new Mock<ISmartContractState>();
            mockInternalExecutor = new Mock<IInternalTransactionExecutor>();

            this.mockContractState.Setup(s => s.PersistentState).Returns(this.mockPersistentState.Object);
            this.mockContractState.Setup(s => s.ContractLogger).Returns(this.mockContractLogger.Object);
            this.mockContractState.Setup(x => x.InternalTransactionExecutor).Returns(mockInternalExecutor.Object);

            assetId = UniqueIdHelper.GenerateId(); 
            this.sender = "0x0000000000000000000000000000000000000001".HexToAddress();
            this.buyer = "0x0000000000000000000000000000000000000002".HexToAddress();
            this.propertyOwner = "0x0000000000000000000000000000000000000003".HexToAddress();
            this.payee = "0x0000000000000000000000000000000000000004".HexToAddress();
        }
```

Below is the code snippet that demonstrates how one can handle the contract Init operation.

```
        [Fact]
        public void SaleDeed_Init_Application()
        {
            this.mockContractState.Setup(s => s.PersistentState).Returns(this.mockPersistentState.Object);
            SaleDeedRegistryContract saleDeedRegistry =
                new SaleDeedRegistryContract(mockContractState.Object, payee);

            Address address = "0x0000000000000000000000000000000000000002".HexToAddress();
            this.mockPersistentState.Setup(s => s.GetUInt32($"PropertyState[" + assetId + "]"))
             .Returns((uint)PropertyStateType.InProgress);

            saleDeedRegistry.InitApplication(propertyOwner, buyer, assetId);

            // Get the Property State and validate
            uint state = saleDeedRegistry.GetPropertyState(assetId);
            Assert.True(state == (uint)PropertyStateType.InProgress);
        }
```

### SaleDeedRegistry Contract Deployment

Let's see how one can easily compile and deploy the Stratis Smart Contract. Here's it's more specific to the compilation of the SaleDeedRegistry smart contract and it's deployment.

We'll be making use of a Statis tool - Stratis.SmartContracts.Tools.Sct.exe for validating and generating the Byte Code for our smart contract.

```
C:\Program Files\cirrus-core-hackathon\resources\sdk>Stratis.SmartContracts.Tools.Sct.exe validate E:\GitCode\Stratis\StratisSmartContractsSamples-master\src\SaleDeedRegistry\SaleDeedRegistry.Contract\Contracts\SaleDeedRegistry.cs -sb
```

Below is what you should see

![alt text](https://raw.githubusercontent.com/ranjancse26/Stratis-SaleDeedRegistry/master/SaleDeedRegistry-Images/SaleDeed-SmartContract-Compilation.png "Smart Contract Compilation")

Copy the ByteCode and let's deploy the same on to the Stratis Cirrus Core.

1) Generating an Address - This is the first step that one has to do. The SaleDeedRegistry smart contract depends on the Payee Address. The Payee here is the government for which they will create a unique address for the first where the citizens will use to for paying the fee payments etc.

Here's how one can generate a unique address.

![alt text](
https://raw.githubusercontent.com/ranjancse26/Stratis-SaleDeedRegistry/master/SaleDeedRegistry-Images/Generate-Address.png "Generate Address")

Copy the generated address and remember to use that one as a Payee Address.

2) On the Stratis Cirrus Core, there's an option of creating a contract. Click on that button and then use the ByteCode for deployment. Please make sure to set the Payee Address as shown below under parameters.

![alt text](
https://raw.githubusercontent.com/ranjancse26/Stratis-SaleDeedRegistry/master/SaleDeedRegistry-Images/CreatingSmartContract.png "Contract Deployment")

Specify the password, if it's default then use - Stratis. If everything goes well, you should see the Receipt with no errors.

![alt text](
https://raw.githubusercontent.com/ranjancse26/Stratis-SaleDeedRegistry/master/SaleDeedRegistry-Images/CreatingSmartContract-Receipt.png "Contract Receipt")


### Testing the Console App

Let's take a look into how to test the SaleDeedRegistry contract using a console app. Below is the console app code snippet.

![alt text](
https://raw.githubusercontent.com/ranjancse26/Stratis-SaleDeedRegistry/master/SaleDeedRegistry-Images/Console/ConsoleApp-Code.png "SaleDeedRegistry Console App")

**Please note - The Console App config needs to be updated for Contract Address and Sender Address. The Buyer, Owner and Payee Address also needs to be created.**

Testing the console app is a really simple thing. That's because of the way how it's coded with "Actors". One just needs to set the application configuration and then hit a breakpoint to debug or directly run the same. You should be able to see the SaleDeedRegistry contract operations being called and the Receipt Response is being displayed on the console. If everything goes well, you should be able to see the application state set to - "Approved". This means the sale deed registry operations have been successfully executed and the ownership has to transfer from the seller to buyer.

Here is the application configuration. There are default app settings keys and there are important configurable elements like Sender, Buyer, Owner/Seller, Payee, and Contract Address that one needs to correctly as per the contract deployment and the wallet address creation for Buyer, Seller, and the Payee.

```
<?xml version="1.0" encoding="utf-8" ?>
<configuration>
  <appSettings>
    
    <add key="WalletName" value="Hackathon_1"/>
    <add key="WalletPassword" value="stratis"/>
    <add key="SmartContractBaseUrl" value="http://localhost:37223"/>

    <add key="GasPrice" value="100"/>
    <add key="GasLimit" value="100000"/>
    <add key="GasFee" value="0.01"/>
    <add key="Amount" value="0"/>

    <add key="ApplicationFee" value="2000"/>

    <add key="SenderAddress" value="CUtNvY1Jxpn4V4RD1tgphsUKpQdo4q5i54"/>
    <add key="BuyerAddress" value="CfW5AzGYA77DpWvdhPcfXvL1hUdkmjZ7PB" />
    <add key="OwnerAddress" value="CdazH6VZkoKMg8FQogvLG4xNwFGDa9gwFQ"/>
    <add key="PayeeAddress" value="CUtNvY1Jxpn4V4RD1tgphsUKpQdo4q5i54"/>
    <add key="ContractAddress" value="CLsDoVPG2zbDWCYvQmdaj5DBcg7BiRu1mV" />
  
  </appSettings>
</configuration>
```

### Sale Deed Registry - Windows Form Desktop Solution

The Desktop solution is designed for handling the property management and sale deed registration. The application is a lightweight solution that interacts with the SaleDeedRegistry smart contract via the Contract Swagger API. 

Below is the application main screen.

![alt text](
https://raw.githubusercontent.com/ranjancse26/Stratis-SaleDeedRegistry/master/SaleDeedRegistry-Images/WinForm/MainForm.png "SaleDeedRegistry Main Screen")

Below is the application settings that one needs to be aware of. **Please note - The Sender, Payee and the Contract address needs to be updated.**. 

The Application Fee is nothing but the Sale Deed Registration fee, it's a configurable one.

```
  <appSettings>
    <add key="WalletName" value="Hackathon_1" />
    <add key="WalletPassword" value="stratis" />
    <add key="SmartContractBaseUrl" value="http://localhost:37223" />

    <add key="GasPrice" value="100" />
    <add key="GasLimit" value="100000" />
    <add key="GasFee" value="0.01" />
    <add key="Amount" value="0" />

    <add key="ApplicationFee" value="2000" />

    <add key="BuyerAddress" value="" />
    <add key="OwnerAddress" value="" />
    <add key="SenderAddress" value="CUtNvY1Jxpn4V4RD1tgphsUKpQdo4q5i54" />
    <add key="PayeeAddress" value="CUtNvY1Jxpn4V4RD1tgphsUKpQdo4q5i54" />
    <add key="ContractAddress" value="CLsDoVPG2zbDWCYvQmdaj5DBcg7BiRu1mV" />
  </appSettings>  
```

The following are the application workflow or steps that one needs to take care of performing a successful sale deed registration.

1. Create Person Info
2. Create Asset and make a note on the Asset Id.
3. Create Signature - One for the Property Owner and the Other for the Supervisor who is handling the Asset creation.
4. Make sure to create the Buyer, Seller, Payee Address.

Once the above steps are taken care of, you are good to perform the Sale Deed Registration.

Below is the screen for creating person info.

![alt text](
https://raw.githubusercontent.com/ranjancse26/Stratis-SaleDeedRegistry/master/SaleDeedRegistry-Images/WinForm/CreateOrUpdatePerson.png
 "Create or Update Person Info")

Two important things you may not be familiar with. i.e Aaddhar and PAN. Here's the definition of the same.

**Aaddhar** - An unique identity. The Unique Identification Authority of India (UIDAI) has mandated to issue an easily verifiable 12 digit random number as Unique Identity - Aadhaar to all Residents of India. Ex: 499118665246

**PAN** - Permanent Account Number. Used for tax purposes. A permanent account number (PAN) is a ten-character alphanumeric identifier, issued in the form of a laminated "PAN card", by the Indian Income Tax Department, to any "person" who applies for it or to whom the department allots the number without an application. Reference - https://en.wikipedia.org/wiki/Permanent_account_number. Ex: AAAPL1234C

Once the personal information has been created. The next thing to do is the Asset Creation.

Below is the screenshot for creating a new Asset.

![alt text](
https://raw.githubusercontent.com/ranjancse26/Stratis-SaleDeedRegistry/master/SaleDeedRegistry-Images/WinForm/CreateAsset.png
 "Create Asset Info")

Below is the screenshot for performing the Sale Deed Registration.

**Please Note - You need to have the Asset Id, Buyer and the Seller/Owner Waller Address to perform this operation**

![alt text](
https://raw.githubusercontent.com/ranjancse26/Stratis-SaleDeedRegistry/master/SaleDeedRegistry-Images/WinForm/SaleDeedRegistry.png
 "Sale Deed Registration")

### Sale Deed Registry Library

The SaleDeedRegistry.Lib is a library that's being utilized by the Console and Windows App.

Below is the class diagram of the Sale Deed Registry operations by following the Command Design Pattern.

![alt text](
https://raw.githubusercontent.com/ranjancse26/Stratis-SaleDeedRegistry/master/SaleDeedRegistry-Images/SaleDeed-Lib-ClassDiagram.png
 "Sale Deed Registration Command Class Diagram")

Below is the class diagram the Actors involved.

![alt text](
https://raw.githubusercontent.com/ranjancse26/Stratis-SaleDeedRegistry/master/SaleDeedRegistry-Images/SaleDeed-Lib-Actor-ClassDiagram.png
 "Sale Deed Registration Actor Class Diagram")

## Sale Deed Registry Wallet 

The SaleDeedRegistry.Wallet is a Windows Forms project created for managing the Stratis Wallet using the Stratis Node V1 Wallet API. It's used for creating a new wallet, loading the loading to make sure it's good. One can also restore wallet or check balance. Verify the transaction by checking the history and then filtering the same. A lot many things can be done using this wallet management app.

Below are some of the application screenshots. 

**Wallet Management Main Screen**

![alt text](
https://raw.githubusercontent.com/ranjancse26/Stratis-SaleDeedRegistry/master/SaleDeedRegistry-Images/Wallet/WalletMainScreen.png
 "Wallet Main Screen")

**Creating a new Mnuemonic**

![alt text](
https://raw.githubusercontent.com/ranjancse26/Stratis-SaleDeedRegistry/master/SaleDeedRegistry-Images/Wallet/CreateMnumonic.png
 "Create Mnumonic")

**Creating a new Wallet**

![alt text](
https://raw.githubusercontent.com/ranjancse26/Stratis-SaleDeedRegistry/master/SaleDeedRegistry-Images/Wallet/CreateWallet.png
 "Create Wallet")

**Load Wallet**

![alt text](
https://raw.githubusercontent.com/ranjancse26/Stratis-SaleDeedRegistry/master/SaleDeedRegistry-Images/Wallet/LoadWallet.png
 "Load Wallet")

**Check Balance**

![alt text](
https://raw.githubusercontent.com/ranjancse26/Stratis-SaleDeedRegistry/master/SaleDeedRegistry-Images/Wallet/CheckBalance.png
 "Check Balance")

**Wallet Info**

![alt text](
https://raw.githubusercontent.com/ranjancse26/Stratis-SaleDeedRegistry/master/SaleDeedRegistry-Images/Wallet/WalletInfo.png
 "Wallet Info")

**Recover Wallet**

![alt text](
https://raw.githubusercontent.com/ranjancse26/Stratis-SaleDeedRegistry/master/SaleDeedRegistry-Images/Wallet/RecoverWallet.png
 "Recover Wallet")

**Wallet Transaction History**

![alt text](
https://raw.githubusercontent.com/ranjancse26/Stratis-SaleDeedRegistry/master/SaleDeedRegistry-Images/Wallet/WalletTransactionHistory.png
 "Wallet Transaction History")

## Interesting Code

Now you will see some of the most interesting Smart Contract aspects.

Below is the code snippet of try getting the receipt information. It's making use of a pooling approach to constantly try to get the receipt information with a specific timeout.

```
        /// <summary>
        /// Try getting the receipt response until the specified timeout
        /// </summary>
        /// <param name="saleRegistryFacade"></param>
        /// <param name="transactionId"></param>
        /// <param name="waitTimeInMinutes"></param>
        /// <returns>ReceiptResponse</returns>
        public async Task<ReceiptResponse> TryReceiptResponse(string transactionId,
            int waitTimeInMinutes = 10, int sleepTime = 1000)
        {
            DateTime dateTime = DateTime.Now;
            DateTime dateTimeMax = dateTime.AddMinutes(waitTimeInMinutes);
            ReceiptResponse receiptInfo = null;

            // Wait until you get a Receipt Info
            while (dateTime < dateTimeMax)
            {
                receiptInfo = await GetReceiptInfo(transactionId);
                if (receiptInfo != null)
                {
                    if(receiptInfo.success)
                        break;
                    if (receiptInfo.error != null)
                        throw new ApplicationException(receiptInfo.error.ToString());
                }
                Thread.Sleep(sleepTime);
            }

            return receiptInfo;
        }

        /// <summary>
        /// Get the V1 Receipt Response
        /// </summary>
        /// <param name="txId">Transaction Id</param>
        /// <returns>HttpResponseMessage</returns>
        public async Task<ReceiptResponse> GetReceiptInfo(string txId)
        {
            using (var httpClient = new HttpClient())
            {
                using (var request = new HttpRequestMessage(new HttpMethod("GET"), 
                    $"{baseUrl}/api/SmartContracts/receipt?txHash={txId}"))
                {
                    request.Headers.TryAddWithoutValidation("accept", "application/json");
                    var response = await httpClient.SendAsync(request);

                    if (response.IsSuccessStatusCode)
                    {
                        string responseJson = await response.Content.ReadAsStringAsync();
                        var receiptResponse = JsonConvert.DeserializeObject<ReceiptResponse>(responseJson);
                        return receiptResponse;
                    }
                }
                return null;
            }
        }
```

Below is the code snippet on how we get the SaleDeedRegistry Smart Contract Application State based on the specified Asset Id.

```
        /// <summary>
        /// Get the Property State
        /// </summary>
        /// <param name="requestEntity">RequestEntity</param>
        /// <returns>HttpResponseMessage</returns>
        public async Task<HttpResponseMessage> GetPropertyState(SaleDeedRegistryRequest requestEntity)
        {
            using (var httpClient = new HttpClient())
            {
                using (var httpRequest = new HttpRequestMessage(new HttpMethod("POST"), 
                   $"{baseUrl}/api/contract/{contractAddress}/method/GetPropertyState"))
                {
                    SetRequestHeader(requestEntity, httpRequest);

                    httpRequest.Content = new StringContent("{  \"assetId\": \"{0}\"}".Replace("{0}", requestEntity.AssetId));
                    httpRequest.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                    var response = await httpClient.SendAsync(httpRequest);                   
                    return response;
                }
            }
        }
```

## Challenges I ran into

The challenging aspect is to deal with the multiple states and making sure the async operations have been successfully completed for handling other contract operations or activities. Since there wasn't a notification mechanism to understand whether or not the operation has been completed, the polling approach was taken into consideration. However, not to run into the infinite looping problem, there was a need to introduce a configurable timeout for checking the transaction/operation completeness.  

Designing a single contact to handle the "Asset" specific persistent state was also a challenging aspect and was addressed using a property as a dictionary approach.

## Accomplishments that I'm proud of

I am really happy to successfully handle the Smart Contract operations including the state or persistence management. I got an assurance from the Indian Government about experimenting with the smart contracts see how it really works for the public.

## What I learned

I learned how to code a Stratis based Smart contract with ease. Generally the smart contract programming is not so easy and yet times it's a great challenge as it requires a different mindset, a completely different use case and the most challenging aspect of it is to understand on how the contracts can be built by using limited resources, data types, etc.

## What's next for Sale Deed Registry

The next big thing which I will be doing is, to give a public presentation and demo to the "Indian Government" in-ordination with the NIC (National Informatics Center). I wish to get as much feedback and then improve or extend the same. 
