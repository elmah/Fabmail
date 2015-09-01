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
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Net.Mail;
    using System.Text;
    using Bootstrapper;
    using Elmah;
    using Mannex;
    using Mannex.Collections.Generic;

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

        static ErrorTextFormatter()
        {
            ErrorMailConfig.Refreshed += delegate { _defaultOptions = null; };
        }

        static ErrorMailOptions _defaultOptions;
        static ErrorMailOptions DefaultOptions => _defaultOptions ?? (_defaultOptions = LoadDefaultOptions());

        public static ErrorMailOptions LoadDefaultOptions()
        {
            const string sectionName = "mail:";
            var entries =
                from e in ErrorMailConfig.Entries
                where e.Key.StartsWith(sectionName, StringComparison.OrdinalIgnoreCase)
                select e.Key.Substring(sectionName.Length).AsKeyTo(e.Value.Replace("-", string.Empty))
                into e
                group e.Value by e.Key;

            var config = entries.ToDictionary(e => e.Key.ToLowerInvariant(),
                e => e.AsEnumerable(),
                StringComparer.OrdinalIgnoreCase);

            var options = new ErrorMailOptions
            {
                DontSkipEmptyKeys =
                    config.Find(nameof(ErrorMailOptions.DontSkipEmptyKeys))?.LastOrDefault()?.IsTruthy() ?? false,
                ErrorDetailUrlFormat = config.Find(nameof(ErrorMailOptions.ErrorDetailUrlFormat))?.LastOrDefault(),
            };

            foreach (var key in config.Find(nameof(ErrorMailOptions.SkippedKeys), Enumerable.Empty<string>()))
                options.SkippedKeys.Add(key);

            foreach (var key in config.Find(nameof(ErrorMailOptions.TimeZoneId), Enumerable.Empty<string>()))
                options.TimeZoneIds.Add(key);

            return null;
        }

        public override void Format(TextWriter writer, Error error)
        {
            try
            {
                writer.Write(new ErrorMailHtmlPage(error, Options).TransformText());
            }
            catch (Exception e) // TODO Move to Bootstrapper
            {
                Trace.TraceError(e.ToString());
            }
        }

        public override string MimeType => "text/html";
    }
}
