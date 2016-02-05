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
    using Microsoft.Security.Application;

    class WebTemplateBase : RazorTemplateBase
    {
        public IHtmlString Html(string html)
        {
            return new HtmlString(html);
        }

        public string AttributeEncode(string text)
        {
            return string.IsNullOrEmpty(text)
                 ? string.Empty
                 : Encoder.HtmlAttributeEncode(text);
        }

        public string Encode(string text)
        {
            return string.IsNullOrEmpty(text)
                 ? string.Empty
                 : Elmah.Html.Encode(text).ToHtmlString();
        }

        public override void Write(object value)
        {
            if (value == null)
                return;
            base.Write(Elmah.Html.Encode(value).ToHtmlString());
        }

        public override object RenderBody()
        {
            return new HtmlString(base.RenderBody().ToString());
        }
    }
}
