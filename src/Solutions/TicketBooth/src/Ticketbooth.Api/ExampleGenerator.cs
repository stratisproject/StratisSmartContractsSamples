using NBitcoin;
using System;

namespace Ticketbooth.Api
{
    public class ExampleGenerator
    {
        private readonly Network _network;

        public ExampleGenerator(Network network)
        {
            _network = network;
        }

        public string ValidAddress()
        {
            switch (_network.NetworkType)
            {
                case NetworkType.Mainnet:
                    return "CUtNvY1Jxpn4V4RD1tgphsUKpQdo4q5i54";
                case NetworkType.Testnet:
                    return "tNuiVJiEhvbQgXu4P32S4TnAVEG3kgLnu8";
                default:
                    throw new NotSupportedException($"Network type {Enum.GetName(typeof(NetworkType), _network.NetworkType)} not supported");
            }
        }
    }
}
