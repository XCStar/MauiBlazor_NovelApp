using AngleSharp.Html.Dom;
using MauiApp3.Data.Interfaces;
using MauiApp3.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MauiApp3.Data.Impl
{
    public class BookBenParser : BaseParser, IPageParser
    {
        public List<Chapter> ParseChapters(IHtmlDocument document)
        {
            var chapters=new List<Chapter>();   
            var liNodes = document.QuerySelectorAll("#readlist li");
            foreach (var node in liNodes)
            {
                var chapter=new Chapter();
                var aNode= node.QuerySelector("a");
                if (aNode is not null)
                {
                    chapter.Url = aNode.Attributes["href"].Value;
                    chapter.Name = aNode.TextContent;
                }
                chapters.Add(chapter);
            }
            return chapters;
        }

        public NovelInfo ParseNovel(IHtmlDocument document)
        {
            var novelInfo = new NovelInfo();
            var nameNode= document.QuerySelector("div.caption");
            if (nameNode is not null&&nameNode.HasChildNodes)
            {

                novelInfo.Name = nameNode.Children.LastOrDefault().TextContent;
            }
            var node = document.QuerySelector("div.detail");
            var pNodes = node.QuerySelectorAll("p");
            if (pNodes.Count() > 4)
            {
                novelInfo.Author = pNodes[0].QuerySelector("a")?.TextContent;
                novelInfo.NovelType = pNodes[1].TextContent?.Replace("类别：", "");
                novelInfo.UpdateState = pNodes[2].TextContent?.Replace("状态：", "");
                novelInfo.WordCount = pNodes[3].TextContent?.Replace("字数：", "");
                novelInfo.UpdateTime = pNodes[4].TextContent?.Replace("更新：", "");
            }
            var desNode = document.QuerySelector(".intro");
            novelInfo.Description = desNode?.TextContent?.Replace("br","\r\n") ;
            return novelInfo;
        }

        public NovelContent ParseNovelContent(IHtmlDocument document)
        {
            var novelContent = new NovelContent();
            novelContent.ChapterName = document.QuerySelector("#headline").TextContent;
            novelContent.Content = document.QuerySelector("#content").InnerHtml;
            var nextNode = document.QuerySelector("#next_url");
            novelContent.Next = nextNode?.Attributes["href"]?.Value;
            return novelContent;
        }

        public int ParseNovelPageCount(IHtmlDocument document)
        {
            var node = document.QuerySelector("#indexselect");
            return node.Children.Length;
        }

        public NovelPageInfo ParseSearchUpdateInfo(IHtmlDocument document)
        {
            var novelPageInfo = new NovelPageInfo();
            novelPageInfo.Infos = new List<UpdateNovelInfo>();
            var liNodes = document.QuerySelectorAll("div.mod>ul li");
            foreach (var item in liNodes)
            {
                var info = new UpdateNovelInfo();
                var aNode = item.QuerySelector("a");
                if (aNode != null)
                {
                    info.Url = aNode.Attributes["href"].Value;
                    if (aNode.HasChildNodes && aNode.ChildNodes.Length > 3)
                    {
                        info.NovelType = aNode.ChildNodes[0].TextContent;
                        info.Name = aNode.ChildNodes[1].TextContent;
                        info.Author = aNode.ChildNodes[3].TextContent;
                    }
                }
                novelPageInfo.Infos.Add(info);
            }
            return novelPageInfo;
        }

        public NovelPageInfo ParseUpdateInfo(IHtmlDocument document)
        {
            var novelPageInfo = new NovelPageInfo();
            novelPageInfo.Infos = new List<UpdateNovelInfo>();
            var liNodes = document.QuerySelectorAll("div.mod>ul li");
            foreach (var item in liNodes)
            {
                var info = new UpdateNovelInfo();
                var aNode = item.QuerySelector("a");
                if (aNode != null)
                {
                    info.Url = aNode.Attributes["href"].Value;
                    if (aNode.HasChildNodes && aNode.ChildNodes.Length > 3)
                    {
                        info.NovelType = aNode.ChildNodes[0].TextContent;
                        info.Name = aNode.ChildNodes[1].TextContent;
                        info.Author = aNode.ChildNodes[3].TextContent;
                    }
                }
                novelPageInfo.Infos.Add(info);
            }
            var pageNode = document.QuerySelector("span#pagenum");
            if (pageNode is not null)
            {
                var text = pageNode.TextContent;
                var match = Regex.Match(text, @"\d+\s+?/\s+?(\d+)");
                if (match != null && match.Groups.Count > 1)
                {
                    if (int.TryParse(match.Groups[1].Value, out int count))
                    {
                        novelPageInfo.PageCount = count;
                    }
                    else
                    {
                        novelPageInfo.PageCount = 1;
                    }

                }
            }
            return novelPageInfo;
        }
    }
}
