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
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Threading.Tasks;
using Ticketbooth.Api.Requests;
using Ticketbooth.Api.Requests.Examples;
using Ticketbooth.Api.Responses;
using Ticketbooth.Api.Responses.Examples;
using Ticketbooth.Api.Tools;
using Ticketbooth.Api.Validation;
using static TicketContract;

namespace Ticketbooth.Api.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Consumes(MediaTypeNames.Application.Json)]
    [Produces(MediaTypeNames.Application.Json)]
    [Route("api/v{version:apiVersion}/ticketbooth")]
    public class PublicController : Controller
    {
        private readonly IBroadcasterManager _broadcasterManager;
        private readonly ICipherFactory _cipherFactory;
        private readonly IConnectionManager _connectionManager;
        private readonly IConsensusManager _consensusManager;
        private readonly ILogger<PublicController> _logger;
        private readonly ISerializer _serializer;
        private readonly ISmartContractTransactionService _smartContractTransactionService;
        private readonly IStateRepositoryRoot _stateRepositoryRoot;
        private readonly IStringGenerator _stringGenerator;
        private readonly Network _network;

        public PublicController(
            IBroadcasterManager broadcasterManager,
            ICipherFactory cipherFactory,
            IConnectionManager connectionManager,
            IConsensusManager consensusManager,
            ILogger<PublicController> logger,
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
        /// Retrieves the ticket data from the ticket contract.
        /// </summary>
        /// <param name="address">The ticket contract address</param>
        /// <returns>HTTP response</returns>
        /// <response code="200">Retrieves the ticket data</response>
        /// <response code="400">Invalid request</response>
        /// <response code="404">Contract does not exist</response>
        /// <response code="500">Unexpected error occured</response>
        [HttpGet("{address}/Tickets")]
        [SwaggerResponseExample(StatusCodes.Status200OK, typeof(TicketsResponseExample))]
        [ProducesResponseType(typeof(Ticket[]), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetTickets(string address)
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

            // retrieve value
            var tickets = RetrieveTickets(numericAddress);
            return Ok(tickets);
        }

        /// <summary>
        /// Retrieves the release fee for the ticket contract.
        /// </summary>
        /// <param name="address">The ticket contract address</param>
        /// <returns>HTTP response</returns>
        /// <response code="200">Retrieves the ticket release fee, in CRS strats</response>
        /// <response code="400">Invalid request</response>
        /// <response code="404">Contract does not exist</response>
        /// <response code="500">Unexpected error occured</response>
        [HttpGet("{address}/TicketReleaseFee")]
        [SwaggerResponseExample(StatusCodes.Status200OK, typeof(PriceResponseExample))]
        [ProducesResponseType(typeof(ulong), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetTicketReleaseFee(string address)
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

            // retrieve value
            var serializedValue = _stateRepositoryRoot.GetStorageValue(numericAddress, _serializer.Serialize("ReleaseFee"));
            return Ok(_serializer.ToUInt64(serializedValue));
        }

        /// <summary>
        /// Retrieves the number of blocks before the end of sale, where a ticket can no longer be released back to the contract.
        /// </summary>
        /// <param name="address">The ticket contract address</param>
        /// <returns>HTTP response</returns>
        /// <response code="200">Retrieves the no release block count</response>
        /// <response code="400">Invalid request</response>
        /// <response code="404">Contract does not exist</response>
        /// <response code="500">Unexpected error occured</response>
        [HttpGet("{address}/NoReleaseBlocks")]
        [SwaggerResponseExample(StatusCodes.Status200OK, typeof(ArbitraryBlockCountResponseExample))]
        [ProducesResponseType(typeof(ulong), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetNoReleaseBlocks(string address)
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

            // retrieve value
            var serializedNoReleaseBlockCount = _stateRepositoryRoot.GetStorageValue(numericAddress, _serializer.Serialize("NoRefundBlockCount"));
            var noReleaseBlockCount = _serializer.ToUInt64(serializedNoReleaseBlockCount);
            return Ok(noReleaseBlockCount);
        }

        /// <summary>
        /// Retrieves whether the venue has an identity verification policy, meaning it requires proof of identity upon entry.
        /// </summary>
        /// <param name="address">The ticket contract address</param>
        /// <returns>HTTP response</returns>
        /// <response code="200">Retrieves whether the venue requires ID</response>
        /// <response code="400">Invalid request</response>
        /// <response code="404">Contract does not exist</response>
        /// <response code="500">Unexpected error occured</response>
        [HttpGet("{address}/IdentityVerificationPolicy")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetIdentityVerificationPolicy(string address)
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

            var requiresIdentityVerification = RetrieveIdentityVerificationPolicy(numericAddress);
            return Ok(requiresIdentityVerification);
        }

        /// <summary>
        /// Requests to purchase a ticket from the ticket contract. Tickets can only be purchased while a sale is active.
        /// </summary>
        /// <param name="address">The ticket contract address</param>
        /// <param name="reserveTicketRequest">The reserve ticket request</param>
        /// <returns>HTTP response</returns>
        /// <response code="201">Returns ticket reservation details</response>
        /// <response code="400">Invalid request</response>
        /// <response code="403">Node has no connections</response>
        /// <response code="404">Contract does not exist</response>
        /// <response code="409">Sale is not currently open</response>
        /// <response code="500">Unexpected error occured</response>
        [HttpPost("{address}/ReserveTicket")]
        [SwaggerRequestExample(typeof(ReserveTicketRequest), typeof(ReserveTicketRequestExample))]
        [SwaggerResponseExample(StatusCodes.Status201Created, typeof(TicketReservationDetailsResponseExample))]
        [ProducesResponseType(typeof(TicketReservationDetailsResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ReserveTicket(string address, ReserveTicketRequest reserveTicketRequest)
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

            // check for state of ticket
            var ticket = FindTicket(numericAddress, reserveTicketRequest.Seat);
            if (ticket.Equals(default(Ticket)))
            {
                return StatusCode(StatusCodes.Status400BadRequest, $"Invalid seat {reserveTicketRequest.Seat.ToDisplayString()}");
            }

            // check contract state
            if (!HasOpenSale(numericAddress))
            {
                return StatusCode(StatusCodes.Status409Conflict, "Sale is not currently open");
            }

            if (ticket.Address != Address.Zero)
            {
                return StatusCode(StatusCodes.Status400BadRequest, $"Ticket for seat {reserveTicketRequest.Seat.ToDisplayString()} not available to purchase");
            }

            var requiresIdentityVerification = RetrieveIdentityVerificationPolicy(numericAddress);
            if (requiresIdentityVerification && string.IsNullOrWhiteSpace(reserveTicketRequest.CustomerName))
            {
                return StatusCode(StatusCodes.Status400BadRequest, $"Customer name is required");
            }

            // check connections
            if (!_connectionManager.ConnectedPeers.Any())
            {
                _logger.LogTrace("No connected peers");
                return StatusCode(StatusCodes.Status403Forbidden, "Can't send transaction as the node requires at least one connection.");
            }

            var seatBytes = _serializer.Serialize(reserveTicketRequest.Seat);
            var seatParameter = $"{Serialization.TypeIdentifiers[typeof(byte[])]}#{Serialization.ByteArrayToHex(seatBytes)}";

            var secret = _stringGenerator.CreateUniqueString(15);
            CbcResult secretCipherResult;
            using (var cipherProvider = _cipherFactory.CreateCbcProvider())
            {
                secretCipherResult = cipherProvider.Encrypt(secret);
            }

            var secretParameter = $"{Serialization.TypeIdentifiers[typeof(byte[])]}#{Serialization.ByteArrayToHex(secretCipherResult.Cipher)}";

            CbcResult customerNameCipherResult = null;
            string customerNameParameter = null;
            if (requiresIdentityVerification)
            {
                using (var cipherProvider = _cipherFactory.CreateCbcProvider())
                {
                    customerNameCipherResult = cipherProvider.Encrypt(reserveTicketRequest.CustomerName);
                }

                customerNameParameter = $"{Serialization.TypeIdentifiers[typeof(byte[])]}#{Serialization.ByteArrayToHex(customerNameCipherResult.Cipher)}";
            }

            // build transaction
            var parameterList = new List<string> { seatParameter, secretParameter };
            if (customerNameParameter != null)
            {
                parameterList.Add(customerNameParameter);
            }

            var callTxResponse = _smartContractTransactionService.BuildCallTx(new BuildCallContractTransactionRequest
            {
                AccountName = reserveTicketRequest.AccountName,
                Amount = StratoshisToStrats(ticket.Price),
                ContractAddress = address,
                FeeAmount = "0",
                GasLimit = SmartContractFormatLogic.GasLimitMaximum,
                GasPrice = reserveTicketRequest.GasPrice,
                MethodName = nameof(TicketContract.Reserve),
                Outpoints = reserveTicketRequest.Outpoints,
                Parameters = parameterList.ToArray(),
                Password = reserveTicketRequest.Password,
                Sender = reserveTicketRequest.Sender,
                WalletName = reserveTicketRequest.WalletName,
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
            var cbcSecretValues = new CbcSecret
            {
                Plaintext = secret,
                Key = secretCipherResult.Key,
                IV = secretCipherResult.IV
            };
            var cbcCustomerValues = requiresIdentityVerification
                ? new CbcValues
                {
                    Key = customerNameCipherResult.Key,
                    IV = customerNameCipherResult.IV
                }
                : null;

            return Created(
                $"/api/smartContracts/receipt?txHash={transactionHash}",
                new TicketReservationDetailsResponse
                {
                    TransactionHash = transactionHash,
                    Secret = cbcSecretValues,
                    CustomerName = cbcCustomerValues
                });
        }

        /// <summary>
        /// Requests a refund for a ticket, which will be issued if the no release block limit is not yet reached.
        /// </summary>
        /// <param name="address">The ticket contract address</param>
        /// <param name="releaseTicketRequest">The release ticket request</param>
        /// <returns>HTTP response</returns>
        /// <response code="201">Returns hash of the broadcast transaction</response>
        /// <response code="400">Invalid request</response>
        /// <response code="403">Node has no connections</response>
        /// <response code="404">Contract does not exist</response>
        /// <response code="409">Ticket release is not available</response>
        /// <response code="500">Unexpected error occured</response>
        [HttpPost("{address}/ReleaseTicket")]
        [SwaggerRequestExample(typeof(ReleaseTicketRequest), typeof(ReleaseTicketRequestExample))]
        [SwaggerResponseExample(StatusCodes.Status201Created, typeof(TransactionHashResponseExample))]
        [ProducesResponseType(typeof(string), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ReleaseTicket(string address, ReleaseTicketRequest releaseTicketRequest)
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

            // check for state of ticket
            var ticket = FindTicket(numericAddress, releaseTicketRequest.Seat);
            if (ticket.Equals(default(Ticket)))
            {
                return StatusCode(StatusCodes.Status400BadRequest, $"Invalid seat {releaseTicketRequest.Seat.ToDisplayString()}");
            }

            // check state of contract
            if (!HasOpenSale(numericAddress))
            {
                return StatusCode(StatusCodes.Status409Conflict, "Sale is not currently open");
            }

            if (!IsRefundAvailable(numericAddress))
            {
                return StatusCode(StatusCodes.Status409Conflict, "Ticket release is no longer available");
            }

            // verify ownership
            if (ticket.Address == Address.Zero || ticket.Address != releaseTicketRequest.Sender.ToAddress(_network))
            {
                return StatusCode(StatusCodes.Status400BadRequest, $"Ticket for seat {releaseTicketRequest.Seat.ToDisplayString()} not owned by {releaseTicketRequest.Sender}");
            }

            // check connections
            if (!_connectionManager.ConnectedPeers.Any())
            {
                _logger.LogTrace("No connected peers");
                return StatusCode(StatusCodes.Status403Forbidden, "Can't send transaction as the node requires at least one connection.");
            }

            var seatBytes = _serializer.Serialize(releaseTicketRequest.Seat);
            var seatParameter = $"{Serialization.TypeIdentifiers[typeof(byte[])]}#{Serialization.ByteArrayToHex(seatBytes)}";

            // build transaction
            var callTxResponse = _smartContractTransactionService.BuildCallTx(new BuildCallContractTransactionRequest
            {
                AccountName = releaseTicketRequest.AccountName,
                Amount = "0",
                ContractAddress = address,
                FeeAmount = "0",
                GasLimit = SmartContractFormatLogic.GasLimitMaximum,
                GasPrice = releaseTicketRequest.GasPrice,
                MethodName = nameof(TicketContract.ReleaseTicket),
                Outpoints = releaseTicketRequest.Outpoints,
                Parameters = new string[] { seatParameter },
                Password = releaseTicketRequest.Password,
                Sender = releaseTicketRequest.Sender,
                WalletName = releaseTicketRequest.WalletName,
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

        private string StratoshisToStrats(ulong amount)
        {
            var paddedValue = amount.ToString().PadLeft(9, '0');
            return paddedValue.Insert(paddedValue.Length - 8, ".");
        }

        private bool RetrieveIdentityVerificationPolicy(uint160 contractAddress)
        {
            var serializedIdentityVerificationPolicy = _stateRepositoryRoot.GetStorageValue(contractAddress, _serializer.Serialize("RequireIdentityVerification"));
            return _serializer.ToBool(serializedIdentityVerificationPolicy);
        }

        private Ticket[] RetrieveTickets(uint160 contractAddress)
        {
            var serializedValue = _stateRepositoryRoot.GetStorageValue(contractAddress, _serializer.Serialize(nameof(TicketContract.Tickets)));
            return _serializer.ToArray<Ticket>(serializedValue);
        }

        private Ticket FindTicket(uint160 contractAddress, Seat seat)
        {
            var tickets = RetrieveTickets(contractAddress);
            return tickets.FirstOrDefault(ticket => ticket.Seat.Equals(seat));
        }

        private bool HasOpenSale(uint160 contractAddress)
        {
            var endOfSale = FetchEndOfSale(contractAddress);
            return endOfSale != default && (ulong)_consensusManager.Tip.Height < endOfSale;
        }

        private ulong FetchEndOfSale(uint160 contractAddress)
        {
            var serializedValue = _stateRepositoryRoot.GetStorageValue(contractAddress, _serializer.Serialize("EndOfSale"));
            return _serializer.ToUInt64(serializedValue);
        }

        private ulong FetchNoRefundBlockCount(uint160 contractAddress)
        {
            var serializedValue = _stateRepositoryRoot.GetStorageValue(contractAddress, _serializer.Serialize("NoRefundBlockCount"));
            return _serializer.ToUInt64(serializedValue);
        }

        private bool IsRefundAvailable(uint160 contractAddress)
        {
            var endOfSale = FetchEndOfSale(contractAddress);
            var noRefundBlockCount = FetchNoRefundBlockCount(contractAddress);
            var endOfRefunds = endOfSale - noRefundBlockCount;
            return endOfSale != default && (ulong)_consensusManager.Tip.Height < endOfRefunds;
        }
    }
}
