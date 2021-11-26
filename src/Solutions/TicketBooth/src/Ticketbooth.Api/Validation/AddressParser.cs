using NBitcoin;
using System;

namespace Ticketbooth.Api.Validation
{
    public static class AddressParser
    {
        public static bool IsValidAddress(string address, Network network)
        {
            try
            {
                BitcoinAddress.Create(address, network);
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }
    }
}
