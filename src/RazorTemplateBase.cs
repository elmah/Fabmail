#region Copyright (c) 2004 Atif Aziz. All rights reserved.
//
// ELMAH - Error Logging Modules and Handlers for ASP.NET
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

namespace Elmah
{
    using System.Text;

    // Adapted from RazorTemplateBase.cs[1]
    // Microsoft Public License (Ms-PL)[2]
    //
    // [1] http://razorgenerator.codeplex.com/SourceControl/changeset/view/964fcd1393be#RazorGenerator.Templating%2fRazorTemplateBase.cs
    // [2] http://razorgenerator.codeplex.com/license

    class RazorTemplateBase
    {
        string _content;
        private readonly StringBuilder _generatingEnvironment = new StringBuilder();

        public RazorTemplateBase Layout { get; set; }

        public virtual void Execute() {}

        public void WriteLiteral(string textToAppend)
        {
            if (string.IsNullOrEmpty(textToAppend))
                return;
            _generatingEnvironment.Append(textToAppend); ;
        }

        public virtual void Write(object value)
        {
            if (value == null)
                return;
            WriteLiteral(value.ToString());
        }

        public virtual object RenderBody()
        {
            return _content;
        }

        public virtual string TransformText()
        {
            Execute();
            
            if (Layout != null)
            {
                Layout._content = _generatingEnvironment.ToString();
                return Layout.TransformText();
            }

            return _generatingEnvironment.ToString();
        }

        public static HelperResult RenderPartial<T>() where T : RazorTemplateBase, new()
        {
            return new HelperResult(writer =>
            {
                var t = new T();
                writer.Write(t.TransformText());
            });
        }
    }
}
#region License, Terms and Author(s)
//
// ELMAH - Error Logging Modules and Handlers for ASP.NET
// Copyright (c) 2004-9 Atif Aziz. All rights reserved.
//
//  Author(s):
//
//      Atif Aziz, http://www.raboof.com
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

namespace Elmah
{
    #region Imports

    using System;
    using System.Globalization;
    using System.IO;

    #endregion

    /// <summary>
    /// Represents the result of a helper action as an HTML-encoded string.
    /// </summary>

    // See http://msdn.microsoft.com/en-us/library/system.web.webpages.helperresult.aspx

    sealed class HelperResult : IHtmlString
    {
        private readonly Action<TextWriter> _action;

        public HelperResult(Action<TextWriter> action)
        {
            if (action == null) throw new ArgumentNullException("action");
            _action = action;
        }

        public string ToHtmlString()
        {
            using (var writer = new StringWriter(CultureInfo.InvariantCulture))
            {
                WriteTo(writer);
                return writer.ToString();
            }
        }

        public override string ToString()
        {
            return ToHtmlString();
        }
        
        public void WriteTo(TextWriter writer)
        {
            _action(writer);
        }
    }
}
#region License, Terms and Author(s)
//
// ELMAH - Error Logging Modules and Handlers for ASP.NET
// Copyright (c) 2004-9 Atif Aziz. All rights reserved.
//
//  Author(s):
//
//      Atif Aziz, http://www.raboof.com
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

namespace Elmah
{
    using Microsoft.Security.Application;

    // http://msdn.microsoft.com/en-us/library/system.web.ihtmlstring.aspx

    interface IHtmlString
    {
        string ToHtmlString();
    }

    sealed class HtmlString : IHtmlString
    {
        readonly string _html;
        public HtmlString(string html) { _html = html ?? string.Empty; }
        public string ToHtmlString() { return _html; }
        public override string ToString() { return ToHtmlString(); }
    }

    static class Html
    {
        public static readonly IHtmlString Empty = new HtmlString(string.Empty);

        public static IHtmlString Raw(string input)
        {
            return string.IsNullOrEmpty(input) ? Empty : new HtmlString(input);
        }

        public static IHtmlString Encode(object input)
        {
            IHtmlString html;
            return null != (html = input as IHtmlString)
                 ? html
                 : input == null
                 ? Empty
                 : Raw(Encoder.HtmlEncode(input.ToString()));
        }
    }
}
/*
#region License, Terms and Author(s)
//
// ELMAH - Error Logging Modules and Handlers for ASP.NET
// Copyright (c) 2004-9 Atif Aziz. All rights reserved.
//
//  Author(s):
//
//      Atif Aziz, http://www.raboof.com
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
    using System.Globalization;
    using System.IO;
    using System.Text;
    using System.Web;
    using Microsoft.Security.Application;
    using Encoder = Microsoft.Security.Application.Encoder;

    // Adapted from RazorTemplateBase.cs[1]
    // Microsoft Public License (Ms-PL)[2]
    //
    // [1] http://razorgenerator.codeplex.com/SourceControl/changeset/view/964fcd1393be#RazorGenerator.Templating%2fRazorTemplateBase.cs
    // [2] http://razorgenerator.codeplex.com/license

    class RazorTemplateBase
    {
        string _content;
        private readonly StringBuilder _generatingEnvironment = new StringBuilder();

        public RazorTemplateBase Layout { get; set; }

        public virtual void Execute() { }

        public void WriteLiteral(string textToAppend)
        {
            if (string.IsNullOrEmpty(textToAppend))
                return;
            _generatingEnvironment.Append(textToAppend); ;
        }

        public virtual void Write(object value)
        {
            if (value == null)
                return;
            WriteLiteral(value.ToString());
        }

        public virtual object RenderBody()
        {
            return _content;
        }

        public virtual string TransformText()
        {
            Execute();

            if (Layout != null)
            {
                Layout._content = _generatingEnvironment.ToString();
                return Layout.TransformText();
            }

            return _generatingEnvironment.ToString();
        }

        public static HelperResult RenderPartial<T>() where T : RazorTemplateBase, new()
        {
            return new HelperResult(writer =>
            {
                var t = new T();
                writer.Write(t.TransformText());
            });
        }
    }
}
#region License, Terms and Author(s)
//
// ELMAH - Error Logging Modules and Handlers for ASP.NET
// Copyright (c) 2004-9 Atif Aziz. All rights reserved.
//
//  Author(s):
//
//      Atif Aziz, http://www.raboof.com
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
    /// <summary>
    /// Represents the result of a helper action as an HTML-encoded string.
    /// </summary>

    // See http://msdn.microsoft.com/en-us/library/system.web.webpages.helperresult.aspx

    sealed class HelperResult : IHtmlString
    {
        private readonly Action<TextWriter> _action;

        public HelperResult(Action<TextWriter> action)
        {
            if (action == null) throw new ArgumentNullException("action");
            _action = action;
        }

        public string ToHtmlString()
        {
            using (var writer = new StringWriter(CultureInfo.InvariantCulture))
            {
                WriteTo(writer);
                return writer.ToString();
            }
        }

        public override string ToString()
        {
            return ToHtmlString();
        }
        
        public void WriteTo(TextWriter writer)
        {
            _action(writer);
        }
    }

    #region License, Terms and Author(s)
//
// ELMAH - Error Logging Modules and Handlers for ASP.NET
// Copyright (c) 2004-9 Atif Aziz. All rights reserved.
//
//  Author(s):
//
//      Atif Aziz, http://www.raboof.com
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

// http://msdn.microsoft.com/en-us/library/system.web.ihtmlstring.aspx

    interface IHtmlString
    {
        string ToHtmlString();
    }

    sealed class HtmlString : IHtmlString
    {
        readonly string _html;
        public HtmlString(string html) { _html = html ?? string.Empty; }
        public string ToHtmlString() { return _html; }
        public override string ToString() { return ToHtmlString(); }
    }

    static class Html
    {
        public static readonly IHtmlString Empty = new HtmlString(string.Empty);

        public static IHtmlString Raw(string input)
        {
            return string.IsNullOrEmpty(input) ? Empty : new HtmlString(input);
        }

        public static IHtmlString Encode(object input)
        {
            IHtmlString html;
            return null != (html = input as IHtmlString)
                ? html
                : input == null
                    ? Empty
                    : Raw(Encoder.HtmlEncode(input.ToString()));
        }
    }
}
*/
