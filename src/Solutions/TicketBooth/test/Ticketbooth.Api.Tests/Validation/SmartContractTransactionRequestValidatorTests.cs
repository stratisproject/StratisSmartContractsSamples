using FluentValidation.TestHelper;
using Moq;
using NBitcoin;
using NUnit.Framework;
using Stratis.Bitcoin.Features.SmartContracts;
using Stratis.Bitcoin.Features.SmartContracts.ReflectionExecutor.Consensus.Rules;
using Stratis.Bitcoin.Features.Wallet.Models;
using Stratis.Bitcoin.Networks;
using System;
using System.Collections.Generic;
using Ticketbooth.Api.Validation;

namespace Ticketbooth.Api.Tests.Validation
{
    public class SmartContractTransactionRequestValidatorTests
    {
        private SmartContractTransactionRequestValidator _validator;

        [SetUp]
        public void SetUp()
        {
            _validator = new SmartContractTransactionRequestValidator(Mock.Of<Network>());
        }

        [Test]
        public void Validation_WalletName()
        {
            // Invalid
            _validator.ShouldHaveValidationErrorFor(validator => validator.WalletName, null as string);
            _validator.ShouldHaveValidationErrorFor(validator => validator.WalletName, string.Empty);
            _validator.ShouldHaveValidationErrorFor(validator => validator.WalletName, "    ");

            // Valid
            _validator.ShouldNotHaveValidationErrorFor(validator => validator.WalletName, "Hello world");
        }

        [Test]
        public void Validation_AccountName()
        {
            // Invalid
            _validator.ShouldHaveValidationErrorFor(validator => validator.AccountName, null as string);
            _validator.ShouldHaveValidationErrorFor(validator => validator.AccountName, string.Empty);
            _validator.ShouldHaveValidationErrorFor(validator => validator.AccountName, "    ");

            // Valid
            _validator.ShouldNotHaveValidationErrorFor(validator => validator.AccountName, "Hello world");
        }

        [Test]
        public void Validation_Outpoints()
        {
            // Invalid

            // Valid
            _validator.ShouldNotHaveValidationErrorFor(validator => validator.Outpoints, null as List<OutpointRequest>);
            _validator.ShouldNotHaveValidationErrorFor(validator => validator.Outpoints, new List<OutpointRequest>());
        }

        [Test]
        public void Validation_Password()
        {
            // Invalid
            _validator.ShouldHaveValidationErrorFor(validator => validator.Password, null as string);

            // Valid
            _validator.ShouldNotHaveValidationErrorFor(validator => validator.Password, string.Empty);
            _validator.ShouldNotHaveValidationErrorFor(validator => validator.Password, "    ");
            _validator.ShouldNotHaveValidationErrorFor(validator => validator.Password, "Hello world");
        }

        [TestCase("BitcoinMain")]
        [TestCase("StratisMain")]
        public void Validation_Sender(string networkName)
        {
            var network = networkName switch
            {
                "BitcoinMain" => Networks.Bitcoin.Mainnet(),
                "StratisMain" => Networks.Stratis.Mainnet(),
                _ => throw new ArgumentException(nameof(networkName))
            };
            _validator = new SmartContractTransactionRequestValidator(network);

            var validBitcoinMain = "16uZZyyewydFYnkMDZDYCUHXdkLTYJACxd";
            var validStratisMain = "SZ3fQbvv4N5W3Bi1WRj3ZuVcft7H9E3TQG";

            // Invalid
            _validator.ShouldHaveValidationErrorFor(validator => validator.Sender, null as string);
            _validator.ShouldHaveValidationErrorFor(
                validator => validator.Sender,
                network.GetType() == typeof(BitcoinMain) ? validStratisMain : validBitcoinMain);

            // Valid
            _validator.ShouldNotHaveValidationErrorFor(
                validator => validator.Sender,
                network.GetType() == typeof(BitcoinMain) ? validBitcoinMain : validStratisMain);
        }

        [Test]
        public void Validation_GasPrice()
        {
            // Invalid
            _validator.ShouldHaveValidationErrorFor(validator => validator.GasPrice, (ulong)0);
            _validator.ShouldHaveValidationErrorFor(validator => validator.GasPrice, SmartContractMempoolValidator.MinGasPrice - 1);
            _validator.ShouldHaveValidationErrorFor(validator => validator.GasPrice, SmartContractFormatLogic.GasPriceMaximum + 1);

            // Valid
            _validator.ShouldNotHaveValidationErrorFor(validator => validator.GasPrice, SmartContractMempoolValidator.MinGasPrice);
            _validator.ShouldNotHaveValidationErrorFor(validator => validator.GasPrice, SmartContractFormatLogic.GasPriceMaximum);
        }
    }
}
