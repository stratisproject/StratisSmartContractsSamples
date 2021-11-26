using System;

namespace SaleDeedRegistry.Lib.Helpers
{
    /// <summary>
    /// Reused code from https://madskristensen.net/blog/generate-unique-strings-and-numbers-in-c/
    /// </summary>
    public static class UniqueIdHelper
    {
        public static string GenerateId()
        {
            long i = 1;
            foreach (byte b in Guid.NewGuid().ToByteArray())
            {
                i *= ((int)b + 1);
            }
            return string.Format("{0:x}", i - DateTime.Now.Ticks);
        }
    }
}
