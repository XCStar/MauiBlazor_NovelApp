using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using AngleSharp.Html.Parser;
using MauiApp3.Data.Interfaces;
using MauiApp3.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MauiApp3.Data.Impl
{
    public class SoduParser :BaseParser, IPageParser
    {
       
        public List<Chapter> ParseChapters(IHtmlDocument document)
        {
            var Chapters = new List<Chapter>();
            //parse chapters
            var chapterNodes = document.QuerySelectorAll("div.list.mt10 li");
            foreach (var chapterNode in chapterNodes)
            {
                var chapter = new Chapter();
                var aNode = chapterNode.Children[0];
                chapter.Name = aNode.TextContent;
                chapter.Url = aNode.Attributes["href"].Value;
                Chapters.Add(chapter);
            }
            return Chapters;

        }

        public NovelContent ParseNovelContent(IHtmlDocument document)
        {
            var novelContent = new NovelContent();
            var titleNode = document.QuerySelector("h1.title");
            if (titleNode != null)
            {
                novelContent.ChapterName = titleNode.TextContent;
            }
            var node = document.QuerySelector("#pb_next").Attributes["href"];
            if (node != null)
            {
                novelContent.Next = node.Value;
            }
            novelContent.Content = document.QuerySelector("div.articlecon").InnerHtml;
            return novelContent;
        }

        public int ParseNovelPageCount(IHtmlDocument document)
        {
            var optionNodes = document.QuerySelectorAll("select.select option");
            var value = optionNodes[optionNodes.Length - 1].TextContent;
            var total = Regex.Match(value, @"\d+").Value;
            if (int.TryParse(total, out int v))
            {
                return v;
            }
            return 0;
        }
        public NovelInfo ParseNovel(IHtmlDocument document)
        {
            var novelInfo = new NovelInfo();
            if (document == null)
            {
                return novelInfo;
            }
            var infoNode = document.QuerySelector("div.info");
            novelInfo.Name = infoNode.Children[0].TextContent;
            novelInfo.Author = infoNode.Children[1].QuerySelector("a").TextContent;
            novelInfo.NovelType = infoNode.Children[2].QuerySelector("a").TextContent;
            novelInfo.LastChapter = infoNode.Children[3].TextContent;
            novelInfo.UpdateTime = infoNode.Children[4].TextContent;
            novelInfo.Description = document.QuerySelector("div.con").TextContent;
            return novelInfo;
        }

        public NovelPageInfo ParseUpdateInfo(IHtmlDocument document)
        {
            var pageInfo = new NovelPageInfo();
            pageInfo.Infos = new List<UpdateNovelInfo>();
            var list = document.QuerySelectorAll("div.list>ul>li");
            if (list.Length > 0)
            {
                //get update info
                foreach (var novel in list)
                {
                    var novelInfo = new UpdateNovelInfo();
                    var nameNode = novel.Children[0];
                    novelInfo.Name = nameNode.TextContent;
                    novelInfo.Url = nameNode.Attributes["href"].Value;
                    var chapterNode = novel.Children[1];
                    novelInfo.LastChapter = chapterNode.TextContent;
                    var timeNode = novel.Children[2];
                    novelInfo.UpdateTime = timeNode.TextContent;
                    pageInfo.Infos.Add(novelInfo);

                }
            }
            var optionNodes = document.QuerySelectorAll("select.select option");
            if (optionNodes.Length > 0)
            {
                var text = optionNodes.Last().TextContent;
                var val = Regex.Match(text, @"\d+").Value;
                if (int.TryParse(val, out int v))
                {
                    pageInfo.PageCount = v;
                }
            }
            return pageInfo;
        }
    }
}
