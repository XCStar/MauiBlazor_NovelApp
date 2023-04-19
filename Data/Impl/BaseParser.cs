using AngleSharp.Html.Dom;
using AngleSharp.Html.Parser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MauiApp3.Data.Impl
{
    public class BaseParser
    {
        public IHtmlDocument LoadDocument(string html)
        {
            if (!string.IsNullOrEmpty(html))
            {
                var parser = new HtmlParser();
                var document = parser.ParseDocument(html);
                return document;
            }
            return null;
        }
    }
}
