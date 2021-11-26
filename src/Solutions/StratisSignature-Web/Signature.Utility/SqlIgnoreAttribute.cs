using System;

namespace Signature.Utility
{ 
    /// <summary>
    /// This member is ignored by SqlMapper.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class SqlIgnoreAttribute : Attribute
    {
    }
}
