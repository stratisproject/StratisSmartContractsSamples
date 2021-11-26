using System;

namespace Signature.Utility
{
    /// <summary>
    /// Binds the property value to another column name, instead of using the name of the property.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class SqlColumnNameAttribute : Attribute
    {
        public string Column { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="column">The name of the column in SqlDataReader</param>
        public SqlColumnNameAttribute(string column)
        {
            Column = column;
        }
    }
}
