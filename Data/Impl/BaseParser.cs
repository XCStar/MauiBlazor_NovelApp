using AngleSharp.Html.Dom;
using AngleSharp.Html.Parser;

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
