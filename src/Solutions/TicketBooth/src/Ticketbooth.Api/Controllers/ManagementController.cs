using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NBitcoin;
using SmartContract.Essentials.Ciphering;
using SmartContract.Essentials.Randomness;
using Stratis.Bitcoin.Connection;
using Stratis.Bitcoin.Consensus;
using Stratis.Bitcoin.Features.SmartContracts.Models;
using Stratis.Bitcoin.Features.SmartContracts.ReflectionExecutor.Consensus.Rules;
using Stratis.Bitcoin.Features.SmartContracts.Wallet;
using Stratis.Bitcoin.Features.Wallet.Interfaces;
using Stratis.SmartContracts;
using Stratis.SmartContracts.CLR;
using Stratis.SmartContracts.Core.State;
using Swashbuckle.AspNetCore.Examples;
using System;
using System.Linq;
using System.Net.Mime;
using System.Threading.Tasks;
using Ticketbooth.Api.Requests;
using Ticketbooth.Api.Requests.Examples;
using Ticketbooth.Api.Responses.Examples;
using Ticketbooth.Api.Validation;
using static TicketContract;

namespace Ticketbooth.Api.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Consumes(MediaTypeNames.Application.Json)]
    [Produces(MediaTypeNames.Application.Json)]
    [Route("api/v{version:apiVersion}/ticketbooth")]
    public class ManagementController : Controller
    {
        private readonly IBroadcasterManager _broadcasterManager;
        private readonly ICipherFactory _cipherFactory;
        private readonly IConnectionManager _connectionManager;
        private readonly IConsensusManager _consensusManager;
        private readonly ILogger<ManagementController> _logger;
        private readonly ISerializer _serializer;
        private readonly ISmartContractTransactionService _smartContractTransactionService;
        private readonly IStateRepositoryRoot _stateRepositoryRoot;
        private readonly IStringGenerator _stringGenerator;
        private readonly Network _network;

        public ManagementController(
            IBroadcasterManager broadcasterManager,
            ICipherFactory cipherFactory,
            IConnectionManager connectionManager,
            IConsensusManager consensusManager,
            ILogger<ManagementController> logger,
            ISerializer serializer,
            ISmartContractTransactionService smartContractTransactionService,
            IStateRepositoryRoot stateRepositoryRoot,
            IStringGenerator stringGenerator,
            Network network)
        {
            _broadcasterManager = broadcasterManager;
            _cipherFactory = cipherFactory;
            _connectionManager = connectionManager;
            _consensusManager = consensusManager;
            _logger = logger;
            _serializer = serializer;
            _smartContractTransactionService = smartContractTransactionService;
            _stateRepositoryRoot = stateRepositoryRoot;
            _stringGenerator = stringGenerator;
            _network = network;
        }

        /// <summary>
        /// Creates a ticket contract.
        /// </summary>
        /// <param name="ticketContractCreateRequest">The ticket contract creation request</param>
        /// <returns>HTTP response</returns>
        /// <response code="201">Returns hash of the broadcast transaction</response>
        /// <response code="400">Invalid request</response>
        /// <response code="403">Node has no connections</response>
        /// <response code="500">Unexpected error occured</response>
        [HttpPost("")]
        [SwaggerRequestExample(typeof(TicketContractCreateRequest), typeof(TicketContractCreateRequestExample))]
        [SwaggerResponseExample(StatusCodes.Status201Created, typeof(TransactionHashResponseExample))]
        [ProducesResponseType(typeof(string), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Create(TicketContractCreateRequest ticketContractCreateRequest)
        {
            // check connections
            if (!_connectionManager.ConnectedPeers.Any())
            {
                _logger.LogTrace("No connected peers");
                return StatusCode(StatusCodes.Status403Forbidden, "Can't send transaction as the node requires at least one connection.");
            }

            var seatsBytes = _serializer.Serialize(ticketContractCreateRequest.Seats);
            var seatsParameter = $"{Serialization.TypeIdentifiers[typeof(byte[])]}#{Serialization.ByteArrayToHex(seatsBytes)}";
            var venueParameter = $"{Serialization.TypeIdentifiers[typeof(string)]}#{ticketContractCreateRequest.Venue}";

            // build transaction
            var createTxResponse = _smartContractTransactionService.BuildCreateTx(new BuildCreateContractTransactionRequest
            {
                AccountName = ticketContractCreateRequest.AccountName,
                Amount = "0",
                ContractCode = "4D5A90000300000004000000FFFF0000B800000000000000400000000000000000000000000000000000000000000000000000000000000000000000800000000E1FBA0E00B409CD21B8014CCD21546869732070726F6772616D2063616E6E6F742062652072756E20696E20444F53206D6F64652E0D0D0A2400000000000000504500004C0102009C9A3CE00000000000000000E00022200B013000001E00000002000000000000023C0000002000000040000000000010002000000002000004000000000000000400000000000000006000000002000000000000030040850000100000100000000010000010000000000000100000000000000000000000B03B00004F000000000000000000000000000000000000000000000000000000004000000C000000943B00001C0000000000000000000000000000000000000000000000000000000000000000000000000000000000000000200000080000000000000000000000082000004800000000000000000000002E74657874000000081C000000200000001E000000020000000000000000000000000000200000602E72656C6F6300000C000000004000000002000000200000000000000000000000000000400000420000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000E43B0000000000004800000002000500D0280000C41200000100000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000133005009B000000010000110203280400000A02280500000A046F0100002B0A02068E691F41FE0216FE0172010000701F418C0A000001280700000A280800000A068E698D040000020B160C2B2207081203FE150400000212030608A3030000027D0400000409A4040000020817D60C08068E6932D8021204FE15050000021204057D090000041104280200002B0202280A00000A6F0B00000A280D000006020728030000062A4602280C00000A72450000706F0300002B2A4A02280C00000A7245000070036F0E00000A2A4602280C00000A72550000706F0F00000A2A4A02280C00000A7255000070036F1000000A2A4602280C00000A72690000706F0F00000A2A0013300300290000000200001102280C00000A7269000070036F1000000A021200FE15070000021200037D0E00000406280400002B2A4602280C00000A727F0000706F0F00000A2A0013300300290000000300001102280C00000A727F000070036F1000000A021200FE15080000021200037D0F00000406280500002B2A4602280C00000A72A50000706F1100000A2A0013300300290000000400001102280C00000A72A5000070036F1200000A021200FE15090000021200037D1000000406280600002B2A4602280C00000A72DD0000706F1300000A2A4A02280C00000A72DD000070036F1400000A2A0000133002001B000000050000110228040000060A062C0F02281500000A6F1600000A06FE052A162A001330040046010000060000110202280A00000A6F0B00000A02280C000006281700000A72E9000070280800000A02022804000006166AFE017233010070280800000A0202281500000A6F1600000A0E05FE057269010070280800000A02280500000A036F0700002B0A0228020000060B02078E69068E69FE0172A7010070280800000A160D2B7E1204FE15040000021613052B330207098F040000027B040000040611058F040000027B04000004281B0000062C0C061105A30400000213042B0D110517D613051105068E6932C6020211047B04000004281A00000616FE0172DF010070280800000A07098F0400000206098F040000027B050000047D050000040917D60D09078E693F79FFFFFF02072803000006020E0528050000061206FE15060000021206047D0A0000041206057D0B00000412060E047D0C00000412060E057D0D00000411060C0208280800002B2A00001330030072000000000000000202280A00000A6F0B00000A02280C000006281700000A720B020070280800000A02022804000006166AFE03724D020070280800000A0202281500000A6F1600000A022804000006FE0516FE01728B020070280800000A0202022802000006281C000006280300000602166A28050000062A0000133003003B000000070000110202280E00000672C3020070280800000A020328180000060A0202067B04000004281A00000616FE0172DF020070280800000A020628190000062A2A0203041428130000062A0000133005005101000008000011020414FE0372FD020070280800000A0202280A0000062C060514FE032B0117721B030070280800000A0202280E00000672C3020070280800000A02280500000A036F0900002B0A0228020000060B150C160D2B1D0207098F040000027B0400000406281B0000062C04090C2B0A0917D60D09078E6932DD020816FE0416FE0172DF020070280800000A02020708A30400000228190000067253030070280800000A0202280A00000A6F1900000A07088F040000027B05000004FE0516FE01727D030070280800000A02280A00000A6F1900000A07088F040000027B05000004362A0202280A00000A6F0B00000A02280A00000A6F1900000A07088F040000027B05000004DB281A00000A2607088F0400000202280A00000A6F0B00000A7D0600000407088F04000002047D0700000407088F04000002057D0800000402072803000006020708A304000002280A00002B2AF60202280A00000A6F0B00000A02280C000006281700000A729F030070280800000A0202280E00000616FE0172EF030070280800000A020328070000062AF60202280A00000A6F0B00000A02280C000006281700000A7209040070280800000A0202280E00000616FE0172EF030070280800000A020328090000062AF60202280A00000A6F0B00000A02280C000006281700000A7271040070280800000A0202280E00000616FE0172EF030070280800000A0203280B0000062A001330040025010000080000110202280E00000672C3020070280800000A0202281500000A6F1600000A022808000006D7022804000006FE0572E3040070280800000A02280500000A036F0900002B0A0228020000060B150C160D2B1D0207098F040000027B0400000406281B0000062C04090C2B0A0917D60D09078E6932DD020816FE0416FE0172DF020070280800000A0202280A00000A6F0B00000A07088F040000027B06000004281700000A7223050070280800000A07088F040000027B0500000402280600000636250202280A00000A6F0B00000A07088F040000027B05000004022806000006DB281A00000A2607088F040000027E1B00000A7D0600000407088F04000002147D0700000407088F04000002147D0800000402072803000006020708A304000002280A00002B2A00000013300300460000000900001102280500000A036F0900002B0A0228020000060B160C2B1D0708A3040000020D02097B0400000406281B0000062C02092A0817580C08078E6932DD1204FE150400000211042A46037B060000047E1B00000A281700000A2A52037B020000042C0A037B0300000416FE012A172A7E037B02000004047B02000004330F037B03000004047B03000004FE012A162A00000013300200490000000A000011160A2B3D03068F04000002166A7D0500000403068F040000027E1B00000A7D0600000403068F04000002147D0700000403068F04000002147D080000040617D60A06038E6932BD032A00000042534A4201000100000000000C00000076342E302E33303331390000000005006C000000B4050000237E0000200600004405000023537472696E677300000000640B00005C05000023555300C0100000100000002347554944000000D0100000F401000023426C6F620000000000000002000001571DA201090A000000FA013300160000010000001100000009000000100000001C0000001E0000001B00000001000000030000000A00000001000000070000000D0000000100000002000000070000000A00000000000F02010000000000060089014E030600A9014E03060075013B030F006E0300000A004F04F1030A002901F1030A00E903F10306001F0131020A001A03F10306001C0031020600E501310206005D0431020A00A900F1030A005001F1030600020531020A00F001F1030A009D04F103000000003C00000000000100010001001000010000001500010001000A0110003E040000210002001D000A01100081040000210004001D000A011000D7010000210009001D000A011000E904000021000A001D000A0110008D00000021000E001D000A011000A903000021000F001D000A0110000B050000210010001D005680450020010600AA02200106001303230106003E042601060066002A010600E903FD0006008F042D010600C5022D010600100131010600100131010600FF023101060015012A010600D8002A010600DB042A010600D5042A010600690234015020000000008618350337010100F72000000000860808044001040009210000000081081404460104001C21000000008108C600BC0005002E21000000008108D4004D01050041210000000081086C00BC00060054210000000081087B004D0106008921000000008108AD04BC0007009C21000000008108C4044D010700D121000000008108450252010800E4210000000081086502560108001922000000008108EB02540009002B22000000008108F5025B0109004022000000008108380252010A006822000000008600EC0061010A00BC23000000008600BE0006000F003C2400000000860032056B010F008324000000008600DD01710110009024000000008600DD0179011200ED250000000086008A004D0115002B26000000008600B8034D0116006926000000008600080556011700A8260000000086006D0483011800DC270000000081007B04890119002E28000000008100F60090011A004028000000008100350496011B00552800000000810001029C011C0078280000000081002004A4011E00000001003D0100000200910300000300020100000100D10100000100D10100000100D10100000100D10100000100D10100000100D101000001009C03000002000C01000003000903000004001A0100000500E200000001007D03000001007D03000002009604000001007D0300000200960400000300D802000001009E0000000100CB03000001008502000001007D03000001007D03000001008804000001004304000001001600000002002200000001002D04090035030100110035030600190035030A00290035031E002900260324004900EE0429005900480437002900E2043D002900EC0143002900B2004F006900B10254002900610159007100F6045E007100FF046B00710028007200710032007700710021029100710029029B007100DB03A6007100E603AC002900F701B7008100A602BC0039002605D10049006404ED006900C701BC002900BC02F5003900A102FD00080004001B012E000B00C0012E001300C9012E001B00E80110007D0087009600B300C000DE00E30001010E010200010000002504AD010000D800B30100009300B3010000C804B30100006902B7010000F902BB0100003C02B70102000200030001000300030002000400050001000500050002000600070001000700070002000800090001000900090002000A000B0001000B000B0002000C000D0001000D000D0002000E000F000480000000000000000000000000000000004F04000004000000000000000000000012014F00000000000100020001000000000000000000F10300000000030002000400020005000200060002000700020008000200090002000D00320013004A001B0066001300820013008C001300A1000D0066001300D90031003200130066000000005469636B6574436F6E74726163745F315F305F3000736561743100496E7433320073656174320047657455496E7436340053657455496E743634003C4D6F64756C653E004D41585F53454154530053797374656D2E507269766174652E436F72654C6962005072696365006765745F52656C65617365466565007365745F52656C65617365466565005365745469636B657452656C656173654665650072656C6561736546656500494D657373616765006765745F4D65737361676500456E6453616C65006765745F456E644F6653616C65007365745F456E644F6653616C6500656E644F6653616C6500426567696E53616C65004973417661696C61626C650076656E75654E616D650073686F774E616D650054696D650074696D650056616C7565547970650049536D617274436F6E7472616374537461746500736D617274436F6E74726163745374617465004950657273697374656E745374617465006765745F50657273697374656E7453746174650044656275676761626C6541747472696275746500436F6D70696C6174696F6E52656C61786174696F6E734174747269627574650052756E74696D65436F6D7061746962696C697479417474726962757465006765745F56616C75650076616C75650056656E7565005265736572766500537472696E67004C6F670049426C6F636B006765745F426C6F636B005365617473417265457175616C00536D617274436F6E74726163742E646C6C00476574426F6F6C00536574426F6F6C0053797374656D006765745F53616C654F70656E006765745F526571756972654964656E74697479566572696669636174696F6E007365745F526571756972654964656E74697479566572696669636174696F6E00726571756972654964656E74697479566572696669636174696F6E005A65726F006765745F4E756D626572006765745F53656E646572005472616E7366657200437573746F6D65724964656E74696669657200637573746F6D65724964656E746966696572006765745F4F776E6572007365745F4F776E6572004F7267616E69736572006F7267616E69736572004C6574746572004953657269616C697A6572006765745F53657269616C697A6572002E63746F720053797374656D2E446961676E6F73746963730053797374656D2E52756E74696D652E436F6D70696C6572536572766963657300446562756767696E674D6F64657300736561744964656E74696669657242797465730073656174734279746573007469636B6574734279746573004E6F526566756E64426C6F636B73005365744E6F52656C65617365426C6F636B73006E6F52656C65617365426C6F636B730047657441646472657373005365744164647265737300537472617469732E536D617274436F6E747261637473006765745F5469636B657473007365745F5469636B6574730052657365745469636B657473007469636B65747300497344656661756C7453656174007365617400466F726D617400536D617274436F6E7472616374004F626A65637400546F5374727563740052656C656173655469636B65740053656C6563745469636B6574007469636B6574005365637265740073656372657400495472616E73666572526573756C74006765745F4E6F526566756E64426C6F636B436F756E74007365745F4E6F526566756E64426C6F636B436F756E7400416D6F756E74004173736572740053686F7700546F4172726179004765744172726179005365744172726179005365744964656E74697479566572696669636174696F6E506F6C696379006F705F457175616C69747900436865636B417661696C6162696C697479000043430061006E006E006F0074002000680061006E0064006C00650020006D006F007200650020007400680061006E0020007B0030007D00200073006500610074007300000F5400690063006B00650074007300001345006E0064004F006600530061006C0065000015520065006C00650061007300650046006500650000254E006F0052006500660075006E00640042006C006F0063006B0043006F0075006E007400003752006500710075006900720065004900640065006E00740069007400790056006500720069006600690063006100740069006F006E00000B4F0077006E006500720000494F006E006C007900200063006F006E007400720061006300740020006F0077006E00650072002000630061006E00200062006500670069006E00200061002000730061006C0065000035530061006C0065002000630075007200720065006E0074006C007900200069006E002000700072006F0067007200650073007300003D530061006C00650020006D007500730074002000660069006E00690073006800200069006E002000740068006500200066007500740075007200650000375300650061007400200065006C0065006D0065006E007400730020006D00750073007400200062006500200065007100750061006C00002B49006E00760061006C0069006400200073006500610074002000700072006F007600690064006500640000414F006E006C007900200063006F006E007400720061006300740020006F0077006E00650072002000630061006E00200065006E0064002000730061006C006500003D530061006C00650020006E006F0074002000630075007200720065006E0074006C007900200069006E002000700072006F00670072006500730073000037530061006C006500200063006F006E007400720061006300740020006E006F0074002000660075006C00660069006C006C0065006400001B530061006C00650020006E006F00740020006F00700065006E00001D530065006100740020006E006F007400200066006F0075006E006400001D49006E00760061006C00690064002000730065006300720065007400003749006E00760061006C0069006400200063007500730074006F006D006500720020006900640065006E0074006900660069006500720000295400690063006B006500740020006E006F007400200061007600610069006C00610062006C00650000214E006F007400200065006E006F007500670068002000660075006E0064007300004F4F006E006C007900200063006F006E007400720061006300740020006F0077006E00650072002000630061006E0020007300650074002000720065006C00650061007300650020006600650065000019530061006C00650020006900730020006F00700065006E0000674F006E006C007900200063006F006E007400720061006300740020006F0077006E00650072002000630061006E00200073006500740020006E006F002000720065006C006500610073006500200062006C006F0063006B00730020006C0069006D006900740000714F006E006C007900200063006F006E007400720061006300740020006F0077006E00650072002000630061006E00200073006500740020006900640065006E007400690074007900200076006500720069006600690063006100740069006F006E00200070006F006C00690063007900003F53007500720070006100730073006500640020006E006F00200072006500660075006E006400200062006C006F0063006B0020006C0069006D0069007400003559006F007500200064006F0020006E006F00740020006F0077006E002000740068006900730020007400690063006B006500740000000000CB68C784244A2B4A96CE11884488B29E000420010108032000010520010111110D07051D110C1D111008111011140520010112190420001225083001011D1E001D05040A01110C0500020E0E1C05200201020E06300101011E00040A0111140420001235042000111D0420001239073001011D1E000E040A011110062002010E123D0420010B0E052002010E0B040701111C040A01111C0407011120040A011120042001020E0407011124052002010E02040A011124052001111D0E062002010E111D0307010B04200012410320000B1007071D11101D1110111808111008111807000202111D111D040A0111180407011110090704110C1D11100808073001011E001D050720021245111D0B0306111D0C0705110C1D1110081110111003070108087CEC85D7BEA7798E04410000000206080206030306110C02060B03061D0502060E0206020820030112191D050E0520001D1110062001011D1110042001010B03200002042001010205200101111D092005011D050E0E0B0B052001021D05072002011D051D05092003011D051D051D05052001011D0506200111101D0505200102111005200102110C07200202110C110C0820011D11101D11100528001D11100328000B03280002042800111D0801000800000000001E01000100540216577261704E6F6E457863657074696F6E5468726F77730108010002000000000000000000000000000000000000000010000000000000000000000000000000D83B00000000000000000000F23B0000002000000000000000000000000000000000000000000000E43B0000000000000000000000005F436F72446C6C4D61696E006D73636F7265652E646C6C0000000000FF2500200010000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000003000000C000000043C00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000",
                FeeAmount = "0",
                GasLimit = SmartContractFormatLogic.GasLimitMaximum,
                GasPrice = ticketContractCreateRequest.GasPrice,
                Outpoints = ticketContractCreateRequest.Outpoints,
                Parameters = new string[] { seatsParameter, venueParameter },
                Password = ticketContractCreateRequest.Password,
                Sender = ticketContractCreateRequest.Sender,
                WalletName = ticketContractCreateRequest.WalletName,
            });

            if (!createTxResponse.Success)
            {
                return StatusCode(StatusCodes.Status400BadRequest, createTxResponse.Message);
            }

            // broadcast transaction
            var transaction = _network.CreateTransaction(createTxResponse.Hex);
            await _broadcasterManager.BroadcastTransactionAsync(transaction);
            var transactionBroadCastEntry = _broadcasterManager.GetTransaction(transaction.GetHash()); // check if transaction was added to mempool

            if (transactionBroadCastEntry?.State == Stratis.Bitcoin.Features.Wallet.Broadcasting.State.CantBroadcast)
            {
                _logger.LogError("Exception occurred: {0}", transactionBroadCastEntry.ErrorMessage);
                return StatusCode(StatusCodes.Status500InternalServerError, transactionBroadCastEntry.ErrorMessage);
            }

            var transactionHash = transaction.GetHash().ToString();
            return Created($"/api/smartContracts/receipt?txHash={transactionHash}", transactionHash);
        }

        /// <summary>
        /// Sets the release fee for the ticket contract. The fee is taken from the refund amount when a ticket is released back to the contract.
        /// </summary>
        /// <param name="address">The ticket contract address</param>
        /// <param name="setTicketReleaseFeeRequest">The set ticket release fee request</param>
        /// <returns>HTTP response</returns>
        /// <response code="201">Returns hash of the broadcast transaction</response>
        /// <response code="400">Invalid request</response>
        /// <response code="403">Node has no connections</response>
        /// <response code="404">Contract does not exist</response>
        /// <response code="409">Sale is active</response>
        /// <response code="500">Unexpected error occured</response>
        [HttpPost("{address}/TicketReleaseFee")]
        [SwaggerRequestExample(typeof(SetTicketReleaseFeeRequest), typeof(SetTicketReleaseFeeRequestExample))]
        [SwaggerResponseExample(StatusCodes.Status201Created, typeof(TransactionHashResponseExample))]
        [ProducesResponseType(typeof(string), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> SetTicketReleaseFee(string address, SetTicketReleaseFeeRequest setTicketReleaseFeeRequest)
        {
            // validate address
            if (!AddressParser.IsValidAddress(address, _network))
            {
                return StatusCode(StatusCodes.Status400BadRequest, $"Invalid address {address}");
            }

            // check contract exists
            var numericAddress = address.ToUint160(_network);
            if (!_stateRepositoryRoot.IsExist(numericAddress))
            {
                return StatusCode(StatusCodes.Status404NotFound, $"No smart contract found at address {address}");
            }

            if (HasActiveSale(numericAddress))
            {
                return StatusCode(StatusCodes.Status409Conflict, "Sale is currently active");
            }

            // check connections
            if (!_connectionManager.ConnectedPeers.Any())
            {
                _logger.LogTrace("No connected peers");
                return StatusCode(StatusCodes.Status403Forbidden, "Can't send transaction as the node requires at least one connection.");
            }

            var feeParameter = $"{Serialization.TypeIdentifiers[typeof(ulong)]}#{setTicketReleaseFeeRequest.Fee}";

            // build transaction
            var callTxResponse = _smartContractTransactionService.BuildCallTx(new BuildCallContractTransactionRequest
            {
                AccountName = setTicketReleaseFeeRequest.AccountName,
                Amount = "0",
                ContractAddress = address,
                FeeAmount = "0",
                GasLimit = SmartContractFormatLogic.GasLimitMaximum,
                GasPrice = setTicketReleaseFeeRequest.GasPrice,
                MethodName = nameof(TicketContract.SetTicketReleaseFee),
                Outpoints = setTicketReleaseFeeRequest.Outpoints,
                Parameters = new string[] { feeParameter },
                Password = setTicketReleaseFeeRequest.Password,
                Sender = setTicketReleaseFeeRequest.Sender,
                WalletName = setTicketReleaseFeeRequest.WalletName,
            });

            if (!callTxResponse.Success)
            {
                return StatusCode(StatusCodes.Status400BadRequest, callTxResponse.Message);
            }

            // broadcast transaction
            var transaction = _network.CreateTransaction(callTxResponse.Hex);
            await _broadcasterManager.BroadcastTransactionAsync(transaction);
            var transactionBroadCastEntry = _broadcasterManager.GetTransaction(transaction.GetHash()); // check if transaction was added to mempool

            if (transactionBroadCastEntry?.State == Stratis.Bitcoin.Features.Wallet.Broadcasting.State.CantBroadcast)
            {
                _logger.LogError("Exception occurred: {0}", transactionBroadCastEntry.ErrorMessage);
                return StatusCode(StatusCodes.Status500InternalServerError, transactionBroadCastEntry.ErrorMessage);
            }

            var transactionHash = transaction.GetHash().ToString();
            return Created($"/api/smartContracts/receipt?txHash={transactionHash}", transactionHash);
        }

        /// <summary>
        /// Sets the number of blocks before the end of sale, where a ticket can no longer be released back to the contract.
        /// </summary>
        /// <param name="address">The ticket contract address</param>
        /// <param name="setTicketReleaseFeeRequest">The set no release blocks request</param>
        /// <returns>HTTP response</returns>
        /// <response code="201">Returns hash of the broadcast transaction</response>
        /// <response code="400">Invalid request</response>
        /// <response code="403">Node has no connections</response>
        /// <response code="404">Contract does not exist</response>
        /// <response code="409">Sale is active</response>
        /// <response code="500">Unexpected error occured</response>
        [HttpPost("{address}/NoReleaseBlocks")]
        [SwaggerRequestExample(typeof(SetNoReleaseBlocksRequest), typeof(SetNoReleaseBlocksRequestExample))]
        [SwaggerResponseExample(StatusCodes.Status201Created, typeof(TransactionHashResponseExample))]
        [ProducesResponseType(typeof(string), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> SetNoReleaseBlocks(string address, SetNoReleaseBlocksRequest setNoReleaseBlocksRequest)
        {
            // validate address
            if (!AddressParser.IsValidAddress(address, _network))
            {
                return StatusCode(StatusCodes.Status400BadRequest, $"Invalid address {address}");
            }

            // check contract exists
            var numericAddress = address.ToUint160(_network);
            if (!_stateRepositoryRoot.IsExist(numericAddress))
            {
                return StatusCode(StatusCodes.Status404NotFound, $"No smart contract found at address {address}");
            }

            if (HasActiveSale(numericAddress))
            {
                return StatusCode(StatusCodes.Status409Conflict, "Sale is currently active");
            }

            // check connections
            if (!_connectionManager.ConnectedPeers.Any())
            {
                _logger.LogTrace("No connected peers");
                return StatusCode(StatusCodes.Status403Forbidden, "Can't send transaction as the node requires at least one connection.");
            }

            var countParameter = $"{Serialization.TypeIdentifiers[typeof(ulong)]}#{setNoReleaseBlocksRequest.Count}";

            // build transaction
            var callTxResponse = _smartContractTransactionService.BuildCallTx(new BuildCallContractTransactionRequest
            {
                AccountName = setNoReleaseBlocksRequest.AccountName,
                Amount = "0",
                ContractAddress = address,
                FeeAmount = "0",
                GasLimit = SmartContractFormatLogic.GasLimitMaximum,
                GasPrice = setNoReleaseBlocksRequest.GasPrice,
                MethodName = nameof(TicketContract.SetNoReleaseBlocks),
                Outpoints = setNoReleaseBlocksRequest.Outpoints,
                Parameters = new string[] { countParameter },
                Password = setNoReleaseBlocksRequest.Password,
                Sender = setNoReleaseBlocksRequest.Sender,
                WalletName = setNoReleaseBlocksRequest.WalletName,
            });

            if (!callTxResponse.Success)
            {
                return StatusCode(StatusCodes.Status400BadRequest, callTxResponse.Message);
            }

            // broadcast transaction
            var transaction = _network.CreateTransaction(callTxResponse.Hex);
            await _broadcasterManager.BroadcastTransactionAsync(transaction);
            var transactionBroadCastEntry = _broadcasterManager.GetTransaction(transaction.GetHash()); // check if transaction was added to mempool

            if (transactionBroadCastEntry?.State == Stratis.Bitcoin.Features.Wallet.Broadcasting.State.CantBroadcast)
            {
                _logger.LogError("Exception occurred: {0}", transactionBroadCastEntry.ErrorMessage);
                return StatusCode(StatusCodes.Status500InternalServerError, transactionBroadCastEntry.ErrorMessage);
            }

            var transactionHash = transaction.GetHash().ToString();
            return Created($"/api/smartContracts/receipt?txHash={transactionHash}", transactionHash);
        }

        /// <summary>
        /// Sets whether the venue has an identity verification policy, meaning it requires proof of identity upon entry.
        /// </summary>
        /// <param name="address">The ticket contract address</param>
        /// <param name="setIdentityVerificationPolicyRequest">The set identity verification policy request</param>
        /// <returns>HTTP response</returns>
        /// <response code="201">Returns hash of the broadcast transaction</response>
        /// <response code="400">Invalid request</response>
        /// <response code="403">Node has no connections</response>
        /// <response code="404">Contract does not exist</response>
        /// <response code="409">Sale is active</response>
        /// <response code="500">Unexpected error occured</response>
        [HttpPost("{address}/IdentityVerificationPolicy")]
        [SwaggerRequestExample(typeof(SetIdentityVerificationPolicyRequest), typeof(SetIdentityVerificationPolicyRequestExample))]
        [SwaggerResponseExample(StatusCodes.Status201Created, typeof(TransactionHashResponseExample))]
        [ProducesResponseType(typeof(string), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> SetIdentityVerificationPolicy(string address, SetIdentityVerificationPolicyRequest setIdentityVerificationPolicyRequest)
        {
            // validate address
            if (!AddressParser.IsValidAddress(address, _network))
            {
                return StatusCode(StatusCodes.Status400BadRequest, $"Invalid address {address}");
            }

            // check contract exists
            var numericAddress = address.ToUint160(_network);
            if (!_stateRepositoryRoot.IsExist(numericAddress))
            {
                return StatusCode(StatusCodes.Status404NotFound, $"No smart contract found at address {address}");
            }

            if (HasActiveSale(numericAddress))
            {
                return StatusCode(StatusCodes.Status409Conflict, "Sale is currently active");
            }

            // check connections
            if (!_connectionManager.ConnectedPeers.Any())
            {
                _logger.LogTrace("No connected peers");
                return StatusCode(StatusCodes.Status403Forbidden, "Can't send transaction as the node requires at least one connection.");
            }

            var requireIdentityVerificationParameter = $"{Serialization.TypeIdentifiers[typeof(bool)]}#{setIdentityVerificationPolicyRequest.RequireIdentityVerification}";

            // build transaction
            var callTxResponse = _smartContractTransactionService.BuildCallTx(new BuildCallContractTransactionRequest
            {
                AccountName = setIdentityVerificationPolicyRequest.AccountName,
                Amount = "0",
                ContractAddress = address,
                FeeAmount = "0",
                GasLimit = SmartContractFormatLogic.GasLimitMaximum,
                GasPrice = setIdentityVerificationPolicyRequest.GasPrice,
                MethodName = nameof(TicketContract.SetIdentityVerificationPolicy),
                Outpoints = setIdentityVerificationPolicyRequest.Outpoints,
                Parameters = new string[] { requireIdentityVerificationParameter },
                Password = setIdentityVerificationPolicyRequest.Password,
                Sender = setIdentityVerificationPolicyRequest.Sender,
                WalletName = setIdentityVerificationPolicyRequest.WalletName,
            });

            if (!callTxResponse.Success)
            {
                return StatusCode(StatusCodes.Status400BadRequest, callTxResponse.Message);
            }

            // broadcast transaction
            var transaction = _network.CreateTransaction(callTxResponse.Hex);
            await _broadcasterManager.BroadcastTransactionAsync(transaction);
            var transactionBroadCastEntry = _broadcasterManager.GetTransaction(transaction.GetHash()); // check if transaction was added to mempool

            if (transactionBroadCastEntry?.State == Stratis.Bitcoin.Features.Wallet.Broadcasting.State.CantBroadcast)
            {
                _logger.LogError("Exception occurred: {0}", transactionBroadCastEntry.ErrorMessage);
                return StatusCode(StatusCodes.Status500InternalServerError, transactionBroadCastEntry.ErrorMessage);
            }

            var transactionHash = transaction.GetHash().ToString();
            return Created($"/api/smartContracts/receipt?txHash={transactionHash}", transactionHash);
        }

        /// <summary>
        /// Begins a ticket sale. Contract policies must be set while the contract is idle.
        /// </summary>
        /// <param name="address">The ticket contract address</param>
        /// <param name="beginSaleRequest">The begin sale request</param>
        /// <returns>HTTP response</returns>
        /// <response code="201">Returns hash of the broadcast transaction</response>
        /// <response code="400">Invalid request</response>
        /// <response code="403">Node has no connections</response>
        /// <response code="404">Contract does not exist</response>
        /// <response code="409">Sale is active</response>
        /// <response code="500">Unexpected error occured</response>
        [HttpPost("{address}/BeginSale")]
        [SwaggerRequestExample(typeof(BeginSaleRequest), typeof(BeginSaleRequestExample))]
        [SwaggerResponseExample(StatusCodes.Status201Created, typeof(TransactionHashResponseExample))]
        [ProducesResponseType(typeof(string), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> BeginSale(string address, BeginSaleRequest beginSaleRequest)
        {
            // validate address
            if (!AddressParser.IsValidAddress(address, _network))
            {
                return StatusCode(StatusCodes.Status400BadRequest, $"Invalid address {address}");
            }

            // check end of sale is in future
            if (beginSaleRequest.Details.EndOfSale <= (ulong)_consensusManager.Tip.Height)
            {
                return StatusCode(StatusCodes.Status400BadRequest, $"End of sale must be in the future. Consensus height is at {_consensusManager.Tip.Height}");
            }

            // check contract exists
            var numericAddress = address.ToUint160(_network);
            if (!_stateRepositoryRoot.IsExist(numericAddress))
            {
                return StatusCode(StatusCodes.Status404NotFound, $"No smart contract found at address {address}");
            }

            if (HasActiveSale(numericAddress))
            {
                return StatusCode(StatusCodes.Status409Conflict, "Sale is currently active");
            }

            // check connections
            if (!_connectionManager.ConnectedPeers.Any())
            {
                _logger.LogTrace("No connected peers");
                return StatusCode(StatusCodes.Status403Forbidden, "Can't send transaction as the node requires at least one connection.");
            }

            // retrieve tickets
            var serializedTickets = _stateRepositoryRoot.GetStorageValue(numericAddress, _serializer.Serialize(nameof(TicketContract.Tickets)));
            var tickets = _serializer.ToArray<Ticket>(serializedTickets);

            if (beginSaleRequest.SeatPrices.Length != tickets.Length)
            {
                return StatusCode(StatusCodes.Status400BadRequest, "Must supply prices for all seats");
            }

            foreach (var seatPrice in beginSaleRequest.SeatPrices)
            {
                for (int i = 0; i < tickets.Length; i++)
                {
                    if (tickets[i].Seat.Equals(seatPrice.Seat))
                    {
                        tickets[i].Price = seatPrice.Price;
                    }
                }
            }

            var ticketsParameter = $"{Serialization.TypeIdentifiers[typeof(byte[])]}#{Serialization.ByteArrayToHex(_serializer.Serialize(tickets))}";
            var showNameParameter = $"{Serialization.TypeIdentifiers[typeof(string)]}#{beginSaleRequest.Details.Name}";
            var organiserParameter = $"{Serialization.TypeIdentifiers[typeof(string)]}#{beginSaleRequest.Details.Organiser}";
            var timeParameter = $"{Serialization.TypeIdentifiers[typeof(ulong)]}#{beginSaleRequest.Details.Time}";
            var endOfSaleParameter = $"{Serialization.TypeIdentifiers[typeof(ulong)]}#{beginSaleRequest.Details.EndOfSale}";

            // build transaction
            var callTxResponse = _smartContractTransactionService.BuildCallTx(new BuildCallContractTransactionRequest
            {
                AccountName = beginSaleRequest.AccountName,
                Amount = "0",
                ContractAddress = address,
                FeeAmount = "0",
                GasLimit = SmartContractFormatLogic.GasLimitMaximum,
                GasPrice = beginSaleRequest.GasPrice,
                MethodName = nameof(TicketContract.BeginSale),
                Outpoints = beginSaleRequest.Outpoints,
                Parameters = new string[] { ticketsParameter, showNameParameter, organiserParameter, timeParameter, endOfSaleParameter },
                Password = beginSaleRequest.Password,
                Sender = beginSaleRequest.Sender,
                WalletName = beginSaleRequest.WalletName,
            });

            if (!callTxResponse.Success)
            {
                return StatusCode(StatusCodes.Status400BadRequest, callTxResponse.Message);
            }

            // broadcast transaction
            var transaction = _network.CreateTransaction(callTxResponse.Hex);
            await _broadcasterManager.BroadcastTransactionAsync(transaction);
            var transactionBroadCastEntry = _broadcasterManager.GetTransaction(transaction.GetHash()); // check if transaction was added to mempool

            if (transactionBroadCastEntry?.State == Stratis.Bitcoin.Features.Wallet.Broadcasting.State.CantBroadcast)
            {
                _logger.LogError("Exception occurred: {0}", transactionBroadCastEntry.ErrorMessage);
                return StatusCode(StatusCodes.Status500InternalServerError, transactionBroadCastEntry.ErrorMessage);
            }

            var transactionHash = transaction.GetHash().ToString();
            return Created($"/api/smartContracts/receipt?txHash={transactionHash}", transactionHash);
        }

        /// <summary>
        /// Resets a ticket contract, making it idle.
        /// </summary>
        /// <param name="address">The ticket contract address</param>
        /// <param name="endSaleRequest">The end sale request</param>
        /// <returns>HTTP response</returns>
        /// <response code="201">Returns hash of the broadcast transaction</response>
        /// <response code="400">Invalid request</response>
        /// <response code="403">Node has no connections</response>
        /// <response code="404">Contract does not exist</response>
        /// <response code="409">Sale is not currently active or has not ended</response>
        /// <response code="500">Unexpected error occured</response>
        [HttpPost("{address}/EndSale")]
        [SwaggerRequestExample(typeof(EndSaleRequest), typeof(EndSaleRequestExample))]
        [SwaggerResponseExample(StatusCodes.Status201Created, typeof(TransactionHashResponseExample))]
        [ProducesResponseType(typeof(string), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> EndSale(string address, EndSaleRequest endSaleRequest)
        {
            // validate address
            if (!AddressParser.IsValidAddress(address, _network))
            {
                return StatusCode(StatusCodes.Status400BadRequest, $"Invalid address {address}");
            }

            // check contract exists
            var numericAddress = address.ToUint160(_network);
            if (!_stateRepositoryRoot.IsExist(numericAddress))
            {
                return StatusCode(StatusCodes.Status404NotFound, $"No smart contract found at address {address}");
            }

            if (!HasSaleEnded(numericAddress))
            {
                return StatusCode(StatusCodes.Status409Conflict, "Sale is not currently active or has not ended");
            }

            // check connections
            if (!_connectionManager.ConnectedPeers.Any())
            {
                _logger.LogTrace("No connected peers");
                return StatusCode(StatusCodes.Status403Forbidden, "Can't send transaction as the node requires at least one connection.");
            }

            // build transaction
            var callTxResponse = _smartContractTransactionService.BuildCallTx(new BuildCallContractTransactionRequest
            {
                AccountName = endSaleRequest.AccountName,
                Amount = "0",
                ContractAddress = address,
                FeeAmount = "0",
                GasLimit = SmartContractFormatLogic.GasLimitMaximum,
                GasPrice = endSaleRequest.GasPrice,
                MethodName = nameof(TicketContract.EndSale),
                Outpoints = endSaleRequest.Outpoints,
                Parameters = Array.Empty<string>(),
                Password = endSaleRequest.Password,
                Sender = endSaleRequest.Sender,
                WalletName = endSaleRequest.WalletName,
            });

            if (!callTxResponse.Success)
            {
                return StatusCode(StatusCodes.Status400BadRequest, callTxResponse.Message);
            }

            // broadcast transaction
            var transaction = _network.CreateTransaction(callTxResponse.Hex);
            await _broadcasterManager.BroadcastTransactionAsync(transaction);
            var transactionBroadCastEntry = _broadcasterManager.GetTransaction(transaction.GetHash()); // check if transaction was added to mempool

            if (transactionBroadCastEntry?.State == Stratis.Bitcoin.Features.Wallet.Broadcasting.State.CantBroadcast)
            {
                _logger.LogError("Exception occurred: {0}", transactionBroadCastEntry.ErrorMessage);
                return StatusCode(StatusCodes.Status500InternalServerError, transactionBroadCastEntry.ErrorMessage);
            }

            var transactionHash = transaction.GetHash().ToString();
            return Created($"/api/smartContracts/receipt?txHash={transactionHash}", transactionHash);
        }

        /// <summary>
        /// Checks if a contract has an active sale
        /// </summary>
        private bool HasActiveSale(uint160 contractAddress)
        {
            var endOfSale = FetchEndOfSale(contractAddress);
            return endOfSale != default;
        }

        /// <summary>
        /// Checks if a contract has an active sale that is ended
        /// </summary>
        private bool HasSaleEnded(uint160 contractAddress)
        {
            var endOfSale = FetchEndOfSale(contractAddress);
            return endOfSale != default && (ulong)_consensusManager.Tip.Height >= endOfSale;
        }

        private ulong FetchEndOfSale(uint160 contractAddress)
        {
            var serializedValue = _stateRepositoryRoot.GetStorageValue(contractAddress, _serializer.Serialize("EndOfSale"));
            return _serializer.ToUInt64(serializedValue);
        }
    }
}
