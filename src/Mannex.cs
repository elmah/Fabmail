#region License, Terms and Author(s)
//
// Mannex - Extension methods for .NET
// Copyright (c) 2009 Atif Aziz. All rights reserved.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//    http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//
#endregion

// ReSharper disable CheckNamespace
// ReSharper disable PartialTypeWithSinglePart

namespace Mannex
{
    using System.Text.RegularExpressions;

    static partial class StringExtensions
    {
        static readonly Regex TruthinessRegex = new Regex(@"\A\s*(?i:true|1|on|yes)\s*\z",
                                                    RegexOptions.CultureInvariant);

        /// <summary>
        /// Returns <c>true</c> if the string value reads (excluding leading
        /// and trailing white space) either <c>"true"</c>, <c>"1"</c>, 
        /// <c>"on"</c> or <c>"yes"</c>. Otherwise it returns <c>false</c>.
        /// </summary>

        public static bool IsTruthy(this string value)
        {
            return value != null && TruthinessRegex.IsMatch(value);
        }
    }
}

namespace Mannex.Collections.Generic
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;

    /// <summary>
    /// Extension methods for pairing keys and values as 
    /// <see cref="KeyValuePair{TKey,TValue}"/>.
    /// </summary>

    static partial class PairingExtensions
    {
        /// <summary>
        /// Pairs a value with a key.
        /// </summary>

        public static KeyValuePair<TKey, TValue> AsKeyTo<TKey, TValue>(this TKey key, TValue value)
        {
            return new KeyValuePair<TKey, TValue>(key, value);
        }
    }

    /// <summary>
    /// Extension methods for <see cref="Dictionary{TKey,TValue}"/>.
    /// </summary>

    static partial class DictionaryExtensions
    {
        /// <summary>
        /// Finds the value for a key, returning the default value for 
        /// <typeparamref name="TKey"/> if the key is not present.
        /// </summary>

        [DebuggerStepThrough]
        public static TValue Find<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey key)
        {
            return Find(dict, key, default(TValue));
        }

        /// <summary>
        /// Finds the value for a key, returning a given default value for 
        /// <typeparamref name="TKey"/> if the key is not present.
        /// </summary>

        [DebuggerStepThrough]
        public static TValue Find<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey key, TValue @default)
        {
            if (dict == null) throw new ArgumentNullException("dict");
            TValue value;
            return dict.TryGetValue(key, out value) ? value : @default;
        }
    }
}
