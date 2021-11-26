using System;
using System.Security.Cryptography;
using System.Text;

namespace Signature.Utility
{
    public static class Sha256Extensions
    {
        public static string Sha256Hash(this byte[] bytes)
        {
            StringBuilder sb = new StringBuilder();

            using (SHA256 hash = SHA256.Create())
            {
                Byte[] result = hash.ComputeHash(bytes);

                foreach (Byte b in result)
                    sb.Append(b.ToString("x2"));
            }

            return sb.ToString();
        }

        public static string Sha256Hash(this string input)
        {
            return Sha256Hash(Encoding.UTF8.GetBytes(input));
        }
    }
}
