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
    #region Imports

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using DiagnosticsCollectionSelector = System.Func<System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<string, StringValues>>, System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<string, StringValues>>>;

    #endregion

    public class ErrorMailOptions
    {
        public bool DontSkipEmptyKeys          { get; set; }
        public string ErrorDetailUrlFormat     { get; set; }

        public ICollection<string> TimeZoneIds { get; } = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        public string TimeZoneId
        {
            get { return TimeZoneIds.FirstOrDefault(); }
            set { TimeZoneIds.Clear(); if (value != null) TimeZoneIds.Add(value); }
        }

        public DiagnosticsCollectionSelector ServerVariablesSelector { get; set; }
        public DiagnosticsCollectionSelector FormSelector            { get; set; }
        public DiagnosticsCollectionSelector QueryStringSelector     { get; set; }
        public DiagnosticsCollectionSelector CookiesSelector         { get; set; }
    }
}