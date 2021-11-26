using Stratis.SmartContracts;
using System;
using System.Collections.Generic;

namespace Ticketbooth.Api.Validation
{
    public static class Serialization
    {
        public static readonly Dictionary<Type, int> TypeIdentifiers = new Dictionary<Type, int>()
        {
            { typeof(bool), 1 },
            { typeof(byte), 2 },
            { typeof(char), 3 },
            { typeof(string), 4 },
            { typeof(uint), 5 },
            { typeof(int), 6 },
            { typeof(ulong), 7 },
            { typeof(long), 8 },
            { typeof(Address), 9 },
            { typeof(byte[]), 10 }
        };

        public static string ByteArrayToHex(byte[] value)
        {
            return BitConverter.ToString(value).Replace("-", string.Empty);
        }
    }
}
