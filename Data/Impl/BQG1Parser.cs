using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using MauiApp3.Data.Interfaces;
using MauiApp3.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MauiApp3.Data.Impl
{
    public class BQG1Parser : BaseParser, IPageParser
    {
        public List<Chapter> ParseChapters(IHtmlDocument document)
        {
            var list = new List<Chapter>();
            var nodes = document.QuerySelector("#list dl");
            int count = 0;
            foreach (var chapterNode in nodes.Children)
            {
                if (chapterNode.NodeName == "DT")
                {
                    count++;
                }
                if (count == 2)
                {
                    var node = chapterNode.QuerySelector("a");
                    if (node is not null)
                    {
                        var chapter = new Chapter();
                        chapter.Url = node.Attributes["href"].Value;
                        chapter.Name = node.Text();
                        list.Add(chapter);
                    }
                   
                }
            }
            return list;
        }

        public NovelContent ParseNovelContent(IHtmlDocument document)
        {
            var novelContent = new NovelContent();
            var chapterNode = document.QuerySelector("div.bookname>h1");
            if (chapterNode != null)
            {
                novelContent.ChapterName = chapterNode.Text();
            }
            var conentNode = document.QuerySelector("#content");
            if (conentNode != null)
            {
                novelContent.Content = conentNode.TextContent;
            }
            var nextNode = document.QuerySelector("#wrapper > div.content_read > div > div.bottem2 > a:nth-child(4)");
            if (nextNode != null)
            {
                novelContent.Next = nextNode.Attributes["href"].Value;
            }
            return novelContent;
        }

        public int ParseNovelPageCount(IHtmlDocument document)
        {
            return 2;
        }

        public NovelInfo ParseNovel(IHtmlDocument document)
        {
            var novelInfo = new NovelInfo();
            var infoNode = document.QuerySelector("#info");
            if (infoNode != null)
            {
                var titleNode=infoNode.QuerySelector("h1");
                if (titleNode != null)
                {
                    novelInfo.Name = titleNode.TextContent;
                }
                var pNodes = infoNode.QuerySelectorAll("p");
                if (pNodes.Count() == 4)
                {
                    novelInfo.Author = pNodes[0].Text().Replace("作者：","");
                    novelInfo.UpdateTime = pNodes[2].Text().Replace("最后更新：","");
                    var aNode = pNodes[3].QuerySelector("a");
                    if (aNode != null)
                    {
                        novelInfo.LastChapter =aNode.TextContent;
                        novelInfo.LastChapterUrl = aNode.Attributes["href"].Value;
                    }
                   
                }
            }
            var descriptionNode = document.QuerySelector("#intro");
            if (descriptionNode is not null)
            {
                novelInfo.Description= descriptionNode.TextContent;
            }
            return novelInfo;
        }
        public NovelPageInfo ParseUpdateInfo(IHtmlDocument document)
        {
            var novelPageInfo = new NovelPageInfo();
            novelPageInfo.Infos = new List<UpdateNovelInfo>();
            var itemNodes = document.QuerySelectorAll("div#novelslist1 > div:nth-child(1)>ul>li");
            ParseNodeInfo(novelPageInfo, itemNodes);
            itemNodes = document.QuerySelectorAll("div#novelslist2 > div:nth-child(1)>ul>li");
            ParseNodeInfo(novelPageInfo, itemNodes);
            itemNodes = document.QuerySelectorAll("div#novelslist2 >div.content.border>ul>li");
            ParseNodeInfo(novelPageInfo, itemNodes);
            novelPageInfo.CurrentPage = 1;
            novelPageInfo.PageCount = 1;

            return novelPageInfo;
        }

        private static void ParseNodeInfo(NovelPageInfo novelPageInfo, IHtmlCollection<AngleSharp.Dom.IElement> itemNodes)
        {
            if (itemNodes != null && itemNodes.Any())
            {
               
                foreach (var item in itemNodes)
                {
                    var novelInfo = new UpdateNovelInfo();
                    var aNode = item.QuerySelector("a");
                    if (aNode != null)
                    {
                        novelInfo.Url = aNode.Attributes["href"].Value;
                        novelInfo.Name = aNode.Text();
                        novelInfo.Author = item.TextContent;
                        novelPageInfo.Infos.Add(novelInfo);
                    }
                }
            }
        }

        public NovelPageInfo ParseSearchUpdateInfo(IHtmlDocument document)
        {
            var pageInfo= new NovelPageInfo(); 
            var liNodes = document.QuerySelectorAll("#newscontent ul li");
            pageInfo.Infos = new List<UpdateNovelInfo>();
            foreach (var liNode in liNodes)
            {
               
                var spanNodes=liNode.QuerySelectorAll("span");
                if (spanNodes.Count() == 5)
                {
                    var info = new UpdateNovelInfo();
                    info.NovelType = spanNodes[0].Text();

                    var nameNode = spanNodes[1].QuerySelector("a");
                    if (nameNode != null)
                    {
                        info.Name=nameNode.Text(); 
                        info.Url = nameNode.Attributes["href"].Value;
                    }
                    var infoNode = spanNodes[2].QuerySelector("a");
                    if (infoNode != null)
                    {
                        info.LastChapter = infoNode.TextContent;
                        info.LastChapterUrl = infoNode.Attributes["href"].Value;
                    }
                    info.Author = spanNodes[3].Text();
                    info.UpdateTime = spanNodes[4].Text();
                    pageInfo.Infos.Add(info);
                }
               
            }
            return pageInfo;
        }
    }
}
