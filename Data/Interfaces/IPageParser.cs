using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using MauiApp3.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MauiApp3.Data.Interfaces
{
    public interface IPageParser
    {

        IHtmlDocument LoadDocument(string html);
        NovelPageInfo ParseUpdateInfo(IHtmlDocument document);
        NovelInfo ParseNovel(IHtmlDocument document);
        List<Chapter> ParseChapters(IHtmlDocument document);
        int ParseNovelPageCount(IHtmlDocument document);
        NovelContent ParseNovelContent(IHtmlDocument document);

    }
}
