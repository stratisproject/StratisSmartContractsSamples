using System;

namespace Signature.Utility
{
    /// <summary>
    /// Implements an array that represents a raw array, but dynamically defined.
    /// <para>Essentially extends <see cref="Array"/> class</para>
    /// </summary>
    public class DynamicArray
    {
        private readonly Type _elementType;
        private Array _array;

        /// <summary>
        /// Gets the internal array.
        /// </summary>
        public Array InternalArray { get { return _array; } }

        /// <summary>
        /// Gets the amount of elements in the <see cref="DynamicArray"/>.
        /// </summary>
        public int Length { get { return _array.Length; } }

        public DynamicArray(Type elementType, int length)
        {
            _elementType = elementType;
            _array = Array.CreateInstance(elementType, length);
        }

        public object this[int index]
        {
            get
            {
                if (index < 0 ||
                    index >= _array.Length)
                    throw new ArgumentOutOfRangeException("index");

                return _array.GetValue(index);
            }
            set
            {
                if (index < 0 ||
                    index >= _array.Length)
                    throw new ArgumentOutOfRangeException("index");

                _array.SetValue(value, index);
            }
        }

        /// <summary>
        /// Resizes the internal array to the new length. Existing elements will be copied up to the new length.
        /// <para>If the new length is less than current array, the elements beyond the new length will be lost.</para>
        /// </summary>
        /// <param name="new_length"></param>
        public void Resize(int new_length)
        {
            var newArray = Array.CreateInstance(_elementType, new_length);
            Array.Copy(_array, newArray, Math.Min(newArray.Length, _array.Length));
            _array = newArray;
        }
    }
}
