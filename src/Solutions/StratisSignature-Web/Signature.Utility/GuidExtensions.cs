using System;

namespace Signature.Utility
{
    public static class GuidExtensions
    {
        public static bool IsValidGuid(this string value)
        {
            var isGuid = Guid.TryParse(value, out _);
            if (isGuid && Guid.Parse(value) == Guid.Empty)
            {
                isGuid = false;
            }
            return isGuid;
        }

        public static bool IsValidGuid(this Guid value)
        {
            var isGuid = Guid.TryParse(value.ToString(), out _);
            if (isGuid && Guid.Parse(value.ToString()) == Guid.Empty)
            {
                isGuid = false;
            }
            return isGuid;
        }
    }
}
