using System;
using System.Collections.Concurrent;
using System.Data;
using System.Linq;
using System.Reflection;
using Signature.Utility.Mapping.Internal;

namespace Signature.Utility.Mapping
{
    public class SqlMapper
    {
        private static readonly ConcurrentDictionary<Type, ClassBinding> _bindings = new ConcurrentDictionary<Type, ClassBinding>();

        /// <summary>
        /// Adds a given type to the mapper.
        /// </summary>
        /// <typeparam name="T">DTO to add</typeparam>
        public static void Add<T>()
        {
            var type = typeof(T);
            if (_bindings.ContainsKey(type))
                return;

            var constructorInfo = type.GetConstructors().Where((a) => a.GetParameters().Length == 0).FirstOrDefault();
            if (constructorInfo == null)
                throw new ArgumentException($"There is no default public constructor for the type {type.FullName}");

            var classBinding = new ClassBinding(type, constructorInfo);

            foreach (var property in type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                var ignoreAttribute = property.GetCustomAttribute<SqlIgnoreAttribute>();
                if (ignoreAttribute != null)
                    continue;

                var name = property.GetCustomAttribute<SqlColumnNameAttribute>()?.Column ?? property.Name;

                classBinding.AddPropertyBinding(new ColumnPropertyBinding(property, name));
            }

            _bindings[type] = classBinding;
        }

        /// <summary>
        /// Maps the columns of a query result to a given DTO.
        /// </summary>
        /// <typeparam name="T">DTO</typeparam>
        /// <param name="reader">The data reader</param>
        /// <exception cref="SqlModelException"></exception>
        public static T Map<T>(IDataRecord reader, bool addIfNotExist = true)
        {
            if (!_bindings.TryGetValue(typeof(T), out var binding))
            {
                if (!addIfNotExist)
                    throw new ArgumentException($"The given type '{typeof(T).FullName}' is not added to the mapper.");

                Add<T>();
                return Map<T>(reader, addIfNotExist);
            }

            return (T)binding.Map(reader);
        }
    }
}
