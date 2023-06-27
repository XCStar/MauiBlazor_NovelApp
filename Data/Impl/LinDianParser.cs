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
    public class LinDianParser : BaseParser,IPageParser
    {
     

        public List<Chapter> ParseChapters(IHtmlDocument document)
        {
            var chapters=new List<Chapter>();
            var divNode=document.QuerySelector("div.section-box:nth-child(4)");
            if (divNode is null)
            {
                return chapters;
            }
            var linkNodes=divNode.QuerySelectorAll("li.book-item>a");
            if (linkNodes is null)
            {
                return chapters;
            }
            foreach (var link in linkNodes) 
            {
                var chapter=new Chapter();
                chapter.Name = link.TextContent;
                chapter.Url = link.Attributes["href"].Value;    
                
                chapters.Add(chapter);
            }
            return chapters;

        }

        public NovelInfo ParseNovel(IHtmlDocument document)
        {
            var novelInfo = new NovelInfo();
            var topNode= document.QuerySelector("div.top");
            if (topNode is null)
            {
                return novelInfo;
            }
            var titleNode=topNode.QuerySelector("h1");
            if (titleNode is null)
            {
                return novelInfo;
            }
            novelInfo.Name = titleNode.TextContent;
            var detialNode = topNode.QuerySelector("div");
            if (detialNode is null)
            {
                return novelInfo;
            }
            var pNodes = detialNode.QuerySelectorAll("p");
            if (pNodes is null)
            {
                return novelInfo;
            }
            novelInfo.Author= pNodes[0].TextContent.Replace("作&nbsp;&nbsp;者：", "");
            novelInfo.NovelType = pNodes[1].TextContent.Replace("类&nbsp;&nbsp;别：","");
            novelInfo.UpdateState= pNodes[2].TextContent.Replace("状&nbsp;&nbsp;态：","");
            novelInfo.UpdateTime = pNodes[4].TextContent.Replace("最后更新：", "");
            var descriptionNode = document.QuerySelector("div.m-desc.xs-show");
            if (descriptionNode is null)
            {
                return novelInfo;
            }
            novelInfo.Description=descriptionNode.TextContent;
            return novelInfo;
        }

        public NovelContent ParseNovelContent(IHtmlDocument document)
        {
           var novelContent=new NovelContent();
            var chapterNameNode = document.QuerySelector("div.reader-main>h1");
            if (chapterNameNode is not null)
            {
                novelContent.ChapterName= chapterNameNode.TextContent;
            }
            var contextNode = document.QuerySelector("div#content");
            if (contextNode is null)
            {
                return novelContent;
            }
            var otherNode = contextNode.QuerySelector("div.posterror");
            if (otherNode is not null)
            {
                otherNode.Remove();
            }
            novelContent.Content = contextNode.InnerHtml;
             var nextNode= document.QuerySelector("div.section-opt a:nth-child(5)");
            if (nextNode is not null)
            {

                if (nextNode.HasAttribute("href"))
                {
                    novelContent.Next = nextNode.GetAttribute("href");
                }
                if (nextNode.TextContent == "下一章")
                {
                    var prevNode= document.QuerySelector("div.section-opt a:nth-child(3)");
                    if (prevNode is not null)
                    {
                        novelContent.Next = prevNode.GetAttribute("href")+novelContent.Next;

                    }
                }
                
               
            }
            return novelContent;
        }

        public int ParseNovelPageCount(IHtmlDocument document)
        {
            return 1;
        }

        public NovelPageInfo ParseSearchUpdateInfo(IHtmlDocument document)
        {
            var novelPageInfo= new NovelPageInfo(); 
            var ulNode = document.QuerySelector("ul.txt-list.txt-list-row5");
            if (ulNode is null)
            {
                return novelPageInfo;
            }
            var liNodes= ulNode.QuerySelectorAll("li");
            if (liNodes is null)
            {
                return novelPageInfo;
            }
            novelPageInfo.Infos=new List<UpdateNovelInfo>();
            for (int i = 1; i < liNodes.Length; i++)
            {
                var spanNodes = liNodes[i].QuerySelectorAll("span") ;
                if (spanNodes is null || spanNodes.Length != 5)
                {
                    continue;
                }
                var updateInfo=new UpdateNovelInfo();
                updateInfo.NovelType = spanNodes[0].TextContent;
                var linkNode = spanNodes[1].QuerySelector("a");
                if (linkNode!=null)
                {
                    updateInfo.Name=linkNode.TextContent;
                    updateInfo.Url = linkNode.Attributes["href"].Value;
                }
                linkNode = spanNodes[2].QuerySelector("a");
                if (linkNode != null)
                {
                    updateInfo.LastChapter = linkNode.TextContent;
                    updateInfo.LastChapterUrl = linkNode.Attributes["href"].Value;
                }
                updateInfo.Author = spanNodes[3].TextContent;
                updateInfo.UpdateTime = spanNodes[4].TextContent;
                novelPageInfo.Infos.Add(updateInfo);
                
            }
            return novelPageInfo;
        }

        public NovelPageInfo ParseUpdateInfo(IHtmlDocument document)
        {
            var info = new NovelPageInfo();
            var liNodes = document.QuerySelectorAll("ul.txt-list.txt-list-row5 li");
            if (liNodes is null)
            {
                return info;
            }
            info.Infos = new List<UpdateNovelInfo>();
            foreach (var liNode in liNodes)
            {
                if (liNode.Children.Length > 4)
                {
                    var updateInfo= new UpdateNovelInfo();
                    updateInfo.NovelType = liNode.Children[0].TextContent;
                    var node = liNode.Children[1].QuerySelector("a");
                    updateInfo.Name = node?.TextContent;
                    updateInfo.Url= node?.Attributes["href"]?.Value;
                    node= liNode.Children[2].QuerySelector("a");
                    updateInfo.LastChapter= node?.TextContent;
                    updateInfo.LastChapterUrl = node?.Attributes["href"]?.Value;
                    updateInfo.Author = liNode.Children[3]?.TextContent;
                    updateInfo.UpdateTime = liNode.Children[4]?.TextContent;
                    info.Infos.Add(updateInfo);
                }

            }
            var pageNode = document.QuerySelector(".hd");
            var text = pageNode.TextContent;
            var match = Regex.Match(text, @"共 \d+ 部小说 当前：\d+/(\d+)");
            if (match.Success && match.Groups.Count > 1)
            {
                if (int.TryParse(match.Groups[1].Value, out int count))
                {
                    info.PageCount = count;
                }
                else
                {
                    info.PageCount = 1;
                }
            }
            return info;
        }
    }
}
