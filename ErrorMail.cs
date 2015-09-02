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

[assembly: System.Web.PreApplicationStartMethod(typeof(Elmah.Fabmail.ErrorTextFormatter), "Install")]

namespace Elmah.Fabmail
{
    #region Imports

    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Net.Mail;
    using System.Text;
    using System.Text.RegularExpressions;
    using Bootstrapper;
    using Elmah;
    using Mannex;
    using Mannex.Collections.Generic;
    using DiagnosticsCollectionSelector = System.Func<System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<string, StringValues>>, System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<string, StringValues>>>;

    #endregion

    public sealed class ErrorTextFormatter : Elmah.ErrorTextFormatter
    {
        public static void Install()
        {
            ErrorTextFormatterFactory.Current = () => new ErrorTextFormatter();
            App.OnModuleEvent(
                (m, h) => m.Mailing += h,
                (m, h) => m.Mailing -= h,
                h => new ErrorMailEventHandler((sender, args) => h(sender, args)),
                (ErrorMailModule m, ErrorMailEventArgs args) => OnErrorMailing(args.Error, args.Mail));
        }

        static void OnErrorMailing(Error error, MailMessage mail)
        {
            var attachments = mail.Attachments;
            var xml = ErrorXml.EncodeString(error);
            attachments.Add(Attachment.CreateAttachmentFromString(xml, "error.xml", Encoding.UTF8, "application/xml"));
            var json = ErrorJson.EncodeString(error);
            attachments.Add(Attachment.CreateAttachmentFromString(json, "error.json", Encoding.UTF8, "application/json"));
        }

        public ErrorMailOptions Options { get; set; }

        public override void Format(TextWriter writer, Error error)
        {
            try
            {
                var options = Options ?? DefaultOptions;
                writer.Write(new ErrorMailHtmlPage(error, options).TransformText());
            }
            catch (Exception e) // TODO Move to Bootstrapper
            {
                Trace.TraceError(e.ToString());
            }
        }

        public override string MimeType => "text/html";

        static ErrorTextFormatter()
        {
            ErrorMailConfig.Refreshed += delegate { _defaultOptions = null; };
        }

        static ErrorMailOptions _defaultOptions;
        static ErrorMailOptions DefaultOptions => _defaultOptions ?? (_defaultOptions = LoadDefaultOptions());

        static class ErrorMailOptionsSelectorSetter
        {
            public static readonly Action<ErrorMailOptions, DiagnosticsCollectionSelector> ServerVariables = (o, v) => o.ServerVariablesSelector = v;
            public static readonly Action<ErrorMailOptions, DiagnosticsCollectionSelector> Form            = (o, v) => o.FormSelector            = v;
            public static readonly Action<ErrorMailOptions, DiagnosticsCollectionSelector> QueryString     = (o, v) => o.QueryStringSelector     = v;
            public static readonly Action<ErrorMailOptions, DiagnosticsCollectionSelector> Cookies         = (o, v) => o.CookiesSelector         = v;
        }

        public static ErrorMailOptions LoadDefaultOptions() => ParseOptions(ErrorMailConfig.Entries);

        public static ErrorMailOptions ParseOptions(IEnumerable<KeyValuePair<string, string>> entries)
        {
            const string sectionName = "fabmail:";
            var section =
                from e in entries
                where e.Key.StartsWith(sectionName, StringComparison.OrdinalIgnoreCase)
                   && !string.IsNullOrEmpty(e.Value)
                group e.Value by e.Key.Substring(sectionName.Length);

            var config = section.ToDictionary(e => e.Key, e => e.AsEnumerable(),
                                              StringComparer.OrdinalIgnoreCase);

            var options = new ErrorMailOptions
            {
                DontSkipEmptyKeys    = config.Find("dont-skip-empty-keys"   )?.LastOrDefault()?.IsTruthy() ?? false,
                ErrorDetailUrlFormat = config.Find("error-detail-url-format")?.LastOrDefault(),
            };

            var passThruFilter = DiagnosticsCollectionSelectors.PassThru;

            var filterSettings =
                from p in new[]
                {
                    new { Prefix = "server-variables-", Setter = ErrorMailOptionsSelectorSetter.ServerVariables },
                    new { Prefix = "form-"            , Setter = ErrorMailOptionsSelectorSetter.Form            },
                    new { Prefix = "query-string-"    , Setter = ErrorMailOptionsSelectorSetter.QueryString     },
                    new { Prefix = "cookies-"         , Setter = ErrorMailOptionsSelectorSetter.Cookies         },
                    new { Prefix = string.Empty       , Setter = ErrorMailOptionsSelectorSetter.ServerVariables
                                                               + ErrorMailOptionsSelectorSetter.Form
                                                               + ErrorMailOptionsSelectorSetter.QueryString
                                                               + ErrorMailOptionsSelectorSetter.Cookies        },
                }
                let filters =
                    from e in new[]
                    {
                        new { Key = "keys"       , Filter = (Func<Regex, DiagnosticsCollectionSelector>) DiagnosticsCollectionSelectors.RegexKeys       },
                        new { Key = "not-keys"   , Filter = (Func<Regex, DiagnosticsCollectionSelector>) DiagnosticsCollectionSelectors.NotRegexKeys    },
                        new { Key = "secret-keys", Filter = (Func<Regex, DiagnosticsCollectionSelector>) DiagnosticsCollectionSelectors.RegexSecretKeys },
                    }
                    let pattern = config.Find(p.Prefix + e.Key)?.LastOrDefault()
                    where !string.IsNullOrEmpty(pattern)
                    select e.Filter(new Regex(pattern))
                select new
                {
                    p.Setter,
                    Filter = filters.Aggregate(passThruFilter, (a, s) => a.Then(s)),

                } into e
                where e.Filter != passThruFilter
                select e;

            foreach (var e in filterSettings)
                e.Setter(options, e.Filter);

            foreach (var key in config.Find("time-zone-id") ?? Enumerable.Empty<string>())
                options.TimeZoneIds.Add(key);

            return options;
        }
    }
}
