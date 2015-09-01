// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Elmah.Fabmail
{
    using System;
    using System.Collections;
    using System.Collections.Generic;

    /// <summary>
    /// Represents zero/null, one, or many strings in an efficient way.
    /// </summary>
    public struct StringValues : IList<string>
    {
        static readonly string[] EmptyArray = new string[0];
        public static readonly StringValues Empty = new StringValues(EmptyArray);

        readonly string _value;
        readonly string[] _values;

        public StringValues(string value)
        {
            _value = value;
            _values = null;
        }

        public StringValues(string[] values)
        {
            _value = null;
            _values = values;
        }

        public static implicit operator StringValues(string value) => new StringValues(value);
        public static implicit operator StringValues(string[] values) => new StringValues(values);
        public static implicit operator string (StringValues values) => values.GetStringValue();
        public static implicit operator string[] (StringValues value) => value.GetArrayValue();

        public int Count => _values?.Length ?? (_value != null ? 1 : 0);

        bool ICollection<string>.IsReadOnly => true;

        string IList<string>.this[int index]
        {
            get { return this[index]; }
            set { throw new NotSupportedException(); }
        }

        public string this[int key] => _values != null
                                     ? _values[key] // may throw
                                     : key == 0 && _value != null
                                     ? _value
                                     : EmptyArray[0]; // throws

        public override string ToString() => GetStringValue() ?? string.Empty;

        string GetStringValue() => _values == null     ? _value
                                 : _values.Length == 0 ? null
                                 : _values.Length == 1 ? _values[0]
                                 : string.Join(",", _values);

        public string[] ToArray() => GetArrayValue() ?? EmptyArray;
        string[] GetArrayValue() => _value != null ? new[] { _value } : _values;

        int IList<string>.IndexOf(string item)
        {
            var index = 0;
            foreach (var value in this)
            {
                if (string.Equals(value, item, StringComparison.Ordinal))
                    return index;
                index += 1;
            }
            return -1;
        }

        bool ICollection<string>.Contains(string item) => ((IList<string>)this).IndexOf(item) >= 0;

        void ICollection<string>.CopyTo(string[] array, int arrayIndex) => CopyTo(array, arrayIndex);

        void CopyTo(string[] array, int arrayIndex)
        {
            for (var i = 0; i < Count; i++)
                array[arrayIndex + i] = this[i];
        }

        void ICollection<string>.Add(string item)         { throw new NotSupportedException(); }
        void IList<string>.Insert(int index, string item) { throw new NotSupportedException(); }
        bool ICollection<string>.Remove(string item)      { throw new NotSupportedException(); }
        void IList<string>.RemoveAt(int index)            { throw new NotSupportedException(); }
        void ICollection<string>.Clear()                  { throw new NotSupportedException(); }

        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable<string>)this).GetEnumerator();

        IEnumerator<string> IEnumerable<string>.GetEnumerator()
        {
            var count = Count;
            if (count == 0)
                yield break;
            if (_values == null)
                yield return _value;
            else
                for (var i = 0; i < count; i++)
                    yield return _values[i];
        }

        public static bool IsNullOrEmpty(StringValues value)
        {
            return value._values == null
                 ? string.IsNullOrEmpty(value._value)
                 : value._values.Length == 0
                   || value._values.Length == 1
                   && string.IsNullOrEmpty(value._values[0]);
        }

        public static StringValues Concat(StringValues values1, StringValues values2)
        {
            var count1 = values1.Count;
            var count2 = values2.Count;

            if (count1 == 0)
                return values2;

            if (count2 == 0)
                return values1;

            var combined = new string[count1 + count2];
            values1.CopyTo(combined, 0);
            values2.CopyTo(combined, count1);

            return new StringValues(combined);
        }
    }
}
