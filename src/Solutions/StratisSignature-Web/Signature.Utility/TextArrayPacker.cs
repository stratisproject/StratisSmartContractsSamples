using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Signature.Utility
{
    public static class TextArrayPacker
    {
        public static string Pack<T>(IEnumerable<T> items)
        {
            var sb = new StringBuilder(256);

            foreach (var item in items)
            {
                if (sb.Length > 0)
                    sb.Append(',');

                sb.Append(item.ToString());
            }

            return sb.ToString();
        }

        private static void Unpack(Type elementType, string text, Action<object> cb)
        {
            // validate the type to make sure that it has Parse method
            var parseMethod = elementType.GetMethods(BindingFlags.Public | BindingFlags.Static).Where((method) =>
            {
                if (method.ReturnType != elementType)
                    return false;

                var parameters = method.GetParameters();
                if (parameters.Length != 1 ||
                    parameters[0].ParameterType != typeof(string))
                    return false;

                return true;
            }).FirstOrDefault();

            if (parseMethod == null)
                throw new ArgumentException("The provided type does not contain a proper static Parse method.");

            var sb = new StringBuilder();

            for (int i = 0; i < text.Length + 1; ++i) // + 1 is detected and "text" is not accessed if >= text.Length
            {
                var ch = i < text.Length ? text[i] : ' ';

                if (i < text.Length &&
                    char.IsWhiteSpace(ch))
                    continue;

                if (i >= text.Length ||
                    ch == ',')
                {
                    if (sb.Length > 0)
                    {
                        object value;

                        try
                        {
                            value = parseMethod.Invoke(null, new object[] { sb.ToString() });
                        }
                        catch (TargetInvocationException ex)
                        {
                            throw ex.InnerException;
                        }

                        cb(value); // invoke callback
                        sb.Clear();
                    }
                }
                else
                    sb.Append(ch);
            }
        }

        public static object Unpack(Type elementType, string text)
        {
            var array = new DynamicArray(elementType, 16);
            int count = 0;

            Unpack(elementType, text, obj =>
            {
                if (count + 1 >= array.Length)
                    array.Resize(array.Length * 2);

                array[count++] = obj;
            });

            array.Resize(count);
            return array.InternalArray;
        }

        public static T[] Unpack<T>(string text)
        {
            var result = new List<T>();
            Unpack(typeof(T), text, obj => result.Add((T)obj));
            return result.ToArray();
        }
    }
}
