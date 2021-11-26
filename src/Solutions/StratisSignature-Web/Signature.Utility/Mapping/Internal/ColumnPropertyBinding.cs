using System.Reflection;

namespace Signature.Utility.Mapping.Internal
{
    internal class ColumnPropertyBinding
    {
        public PropertyInfo PropertyInfo { get; private set; }

        public string ColumnName { get; private set; }

        public ColumnPropertyBinding(PropertyInfo propertyInfo, string columnName)
        {
            PropertyInfo = propertyInfo;
            ColumnName = columnName;
        }
    }
}
