#region Copyright (c) 2015 Atif Aziz. All rights reserved.
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

namespace Elmah.Fabmail
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Linq;
    using Mannex.Collections.Generic;

    public class ErrorMailOptions
    {
        public bool DontSkipEmptyKeys          { get; set; }
        public ICollection<string> SkippedKeys { get; } = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        public string ErrorDetailUrlFormat     { get; set; }

        public ICollection<string> TimeZoneIds { get; } = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        public string TimeZoneId
        {
            get { return TimeZoneIds.FirstOrDefault(); }
            set { TimeZoneIds.Clear(); if (value != null) TimeZoneIds.Add(value); }
        }

        public Func<NameValueCollection, IEnumerable<KeyValuePair<string, StringValues>>> ServerVariablesSelector { get; set; }
        public Func<NameValueCollection, IEnumerable<KeyValuePair<string, StringValues>>> FormSelector            { get; set; }
        public Func<NameValueCollection, IEnumerable<KeyValuePair<string, StringValues>>> QueryStringSelector     { get; set; }
        public Func<NameValueCollection, IEnumerable<KeyValuePair<string, StringValues>>> CookiesSelector         { get; set; }

        public Func<NameValueCollection, IEnumerable<KeyValuePair<string, StringValues>>>
            DefaultDiagnosticsCollectionSelector = collection =>
                from i in Enumerable.Range(0, collection.Count)
                select collection.GetKey(i)
                                 .AsKeyTo(new StringValues(collection.GetValues(i)));
    }
}