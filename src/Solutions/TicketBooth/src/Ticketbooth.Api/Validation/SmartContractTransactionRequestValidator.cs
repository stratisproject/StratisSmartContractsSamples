using FluentValidation;
using NBitcoin;
using Stratis.Bitcoin.Features.SmartContracts;
using Stratis.Bitcoin.Features.SmartContracts.ReflectionExecutor.Consensus.Rules;
using Ticketbooth.Api.Requests;

namespace Ticketbooth.Api.Validation
{
    public class SmartContractTransactionRequestValidator : AbstractValidator<SmartContractTransactionRequest>
    {
        public SmartContractTransactionRequestValidator(Network network)
        {
            RuleFor(request => request.WalletName).NotEmpty();
            RuleFor(request => request.AccountName).NotEmpty();
            RuleFor(request => request.Password).NotNull();
            RuleFor(request => request.GasPrice).InclusiveBetween(SmartContractMempoolValidator.MinGasPrice, SmartContractFormatLogic.GasPriceMaximum);
            RuleFor(request => request.Sender).IsAddress(network);
        }
    }
}
