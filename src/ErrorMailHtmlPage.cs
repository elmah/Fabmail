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
    using System.Text.RegularExpressions;
    using Bootstrapper;

    partial class ErrorMailHtmlPage
    {
        static readonly ErrorMailOptions DefaultOptions = new ErrorMailOptions
        {
            ServerVariablesSelector = DiagnosticsCollectionSelectors.NotRegexKeys(new Regex("^ALL_(HTTP|RAW)$")),
            TimeZoneIds =
            {
                "UTC",
                "Central Standard Time",
                "W. Europe Standard Time",
                "GMT Standard Time",
                "Russian Standard Time",
                "Singapore Standard Time",
            }
        };

        public ErrorMailHtmlPage(Error error, ErrorMailOptions options = null)
        {
            Error = error;
            if (error.Exception != null)
                ErrorLogEntry = LoggedException.RecallErrorLogEntry(error.Exception);
            Options = options ?? DefaultOptions;
        }

        Error Error { get; }
        ErrorLogEntry ErrorLogEntry { get; }
        ErrorMailOptions Options { get; }

        Uri TryGetErrorDetailUrl()
        {
            var format = Options.ErrorDetailUrlFormat;
            Uri url;
            return format != null && Uri.TryCreate(Regex.Replace(format, @":id\b", Uri.EscapeDataString(ErrorLogEntry.Id)), UriKind.Absolute, out url)
                 ? url : null;
        }

        static string MarkupStackTrace(string text)
        {
            return StackTraceFormatter.FormatHtml(text, new StackTraceHtmlFragments
            {
                BeforeType          = "<span style='color: #00008B'>", AfterType          = "</span>",
                BeforeMethod        = "<span style='color: #008B8B"
                                    + "; font-weight: bolder'>"      , AfterMethod        = "</span>",
                BeforeParameterType = "<span style='color: #00008B'>", AfterParameterType = "</span>",
                BeforeParameterName = "<span style='color: #666'>"   , AfterParameterName = "</span>",
                BeforeFile          = "<span style='color: #8B008B'>", AfterFile          = "</span>",
                BeforeLine          = "<span style='color: #8B008B'>", AfterLine          = "</span>",
            });
        }
    }
}