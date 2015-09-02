namespace Elmah.Fabmail
{
    #region Imports

    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Linq;
    using System.Text.RegularExpressions;
    using Mannex.Collections.Generic;
    using DiagnosticsCollectionSelector = System.Func<System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<string, StringValues>>, System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<string, StringValues>>>;

    #endregion

    public static class DiagnosticsCollectionSelectors
    {
        // ReSharper disable once PossibleMultipleEnumeration
        public static DiagnosticsCollectionSelector PassThru = collection => collection;

        public static NameValueCollection ToNameValueCollection(
            this IEnumerable<KeyValuePair<string, StringValues>> collection)
        {
            var output = new NameValueCollection();
            foreach (var e in from e in collection
                              from v in e.Value
                              select e.Key.AsKeyTo(e.Value))
                output.Add(e.Key, e.Value);
            return output;
        }

        public static IEnumerable<KeyValuePair<string, StringValues>> AsEnumerable(this NameValueCollection collection)
        {
            return
                from i in Enumerable.Range(0, collection.Count)
                select collection.GetKey(i).AsKeyTo(new StringValues(collection.GetValues(i)));
        }

        public static Func<IEnumerable<KeyValuePair<string, StringValues>>, IEnumerable<KeyValuePair<string, StringValues>>> RegexKeys(Regex regex)    => RegexKeys(regex, false);
        public static Func<IEnumerable<KeyValuePair<string, StringValues>>, IEnumerable<KeyValuePair<string, StringValues>>> NotRegexKeys(Regex regex) => RegexKeys(regex, true);

        public static Func<IEnumerable<KeyValuePair<string, StringValues>>, IEnumerable<KeyValuePair<string, StringValues>>> RegexKeys(Regex regex, bool exclude) =>
            collection => from e in collection
                          where regex.IsMatch(e.Key) == !exclude
                          select e;

        static readonly StringValues DefaultSecretMask = new StringValues("*****");

        public static Func<IEnumerable<KeyValuePair<string, StringValues>>, IEnumerable<KeyValuePair<string, StringValues>>> RegexSecretKeys(Regex regex) => RegexSecretKeys(regex, null);

        public static Func<IEnumerable<KeyValuePair<string, StringValues>>, IEnumerable<KeyValuePair<string, StringValues>>> RegexSecretKeys(Regex regex, string mask) =>
            RegexSecretKeys(regex.IsMatch,
                            (_, vs) => vs.Count > 1
                                     ? new StringValues(Enumerable.Repeat(mask, vs.Count).ToArray())
                                     : mask == null ? DefaultSecretMask : new StringValues(mask));

        public static Func<IEnumerable<KeyValuePair<string, StringValues>>, IEnumerable<KeyValuePair<string, StringValues>>> RegexSecretKeys(Func<string, bool> keyPredicate, Func<StringValues, StringValues> selector) =>
            RegexSecretKeys(keyPredicate, (_, s) => selector(s));

        public static Func<IEnumerable<KeyValuePair<string, StringValues>>, IEnumerable<KeyValuePair<string, StringValues>>> RegexSecretKeys(Func<string, bool> keyPredicate, Func<string, StringValues, StringValues> selector) =>
            collection => from e in collection
                          select keyPredicate(e.Key)
                               ? e.Key.AsKeyTo(selector(e.Key, e.Value))
                               : e;
    }
}