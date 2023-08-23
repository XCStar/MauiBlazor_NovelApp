using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using AngleSharp.Text;
using MauiApp3.Data.Interfaces;
using MauiApp3.Model;
using Microsoft.Maui.Animations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MauiApp3.Data.Impl
{
    public class XS166Parser : BaseParser, IPageParser
    {
        public List<Chapter> ParseChapters(IHtmlDocument document)
        {
            var chapters=new List<Chapter>();
            var nodes = document.QuerySelectorAll("#chapterlist a").Skip(1);
            foreach (var node in nodes)
            {
                var chapter=new Chapter();
                chapter.Name = node.TextContent;
                chapter.Url = node.Attributes["href"].Value;
                chapters.Add(chapter);

            }
            return chapters;
        }

        public NovelInfo ParseNovel(IHtmlDocument document)
        {
            var novelInfo = new NovelInfo();
            var titleNode = document.QuerySelector(".title");
            novelInfo.Name=titleNode.TextContent;
            var detialNode = document.QuerySelector("#book_detail");
            if (detialNode.Children.Length > 3)
            {
                novelInfo.Author = detialNode?.Children[0]?.TextContent;
                novelInfo.NovelType = detialNode.Children[1]?.QuerySelector("span")?.TextContent;
                novelInfo.UpdateState = detialNode.Children[2]?.TextContent?.Replace("状态：", "");
                novelInfo.UpdateTime = detialNode.Children[3]?.TextContent?.Replace("更新：", "");
            }
            novelInfo.Description= document.QuerySelector(".review")?.TextContent; ;
           
            return novelInfo;
        }

        public NovelContent ParseNovelContent(IHtmlDocument document)
        {
            var content=new NovelContent();
            content.Content = document.QuerySelector("#chaptercontent")?.InnerHtml;
            content.ChapterName = document.QuerySelector("#top .title")?.TextContent;
            content.Next = document.QuerySelector("#pt_next")?.Attributes["href"].Value;
            if (!string.IsNullOrEmpty(content.Content))
            {
               content.Content= MauiApp3.Common.StringExtensions.HtmlForMat166(content.Content);
            }
            return content;
        }

        public int ParseNovelPageCount(IHtmlDocument document)
        {
            return 1;
        }

        public NovelPageInfo ParseSearchUpdateInfo(IHtmlDocument document)
        {
            var novelPageInfo=new NovelPageInfo();
            var nodes = document.QuerySelectorAll(".hot_sale");
            novelPageInfo.Infos = new List<UpdateNovelInfo>();
            foreach (var node in nodes)
            {
                if (node.Children.Length > 3)
                {
                    var info = new UpdateNovelInfo();
                    info.Name = node.Children[1]?.TextContent;
                    info.Url = node.Children[1]?.Attributes["href"].Value;
                    info.Author= node.Children[2].LastChild.TextContent;
                    info.LastChapter = node.Children[3].QuerySelector("a").TextContent;
                    info.UpdateState = node.Children[3].FirstChild.TextContent.Replace(" | 最近更新：", "");
                    info.UpdateTime = node.Children[3].LastChild.TextContent;
                    novelPageInfo.Infos.Add(info);
                    
                }
            }
            return novelPageInfo;
        }

        public NovelPageInfo ParseUpdateInfo(IHtmlDocument document)
        {
            var pageInfo=new NovelPageInfo();
            pageInfo.Infos = new List<UpdateNovelInfo>();
            var nodes = document.QuerySelectorAll("#main div.hot_sale");
            foreach ( var node in nodes ) 
            {
                var info = new UpdateNovelInfo();
                var detialNode = node.QuerySelector(".detail");
                if (detialNode != null)
                {
                    info.Name = detialNode.Children[0].TextContent;
                    info.Url = detialNode.Children[0].Attributes["href"].Value;
                   
                }
                info.UpdateTime= node.QuerySelector(".score")?.TextContent; ;
                var authorNode = node.QuerySelector(".author");
                if (authorNode != null)
                {
                    info.Author = authorNode.LastChild.TextContent;

                }
                info.Description = node.LastChild.TextContent;
                pageInfo.Infos.Add(info);
            }
            var pageNode = document.QuerySelector("#txtPage");
            pageInfo.PageCount = 1;
            if (pageNode!=null&&pageNode.HasAttribute("value"))
            {
                var valStr = pageNode.Attributes["value"].Value;
                if (!string.IsNullOrEmpty(valStr))
                {
                   var match= Regex.Match(valStr,@"\d+/(\d+)");
                   var count= match.Groups[match.Groups.Count - 1].Value;
                    if (int.TryParse(count, out int total))
                    {
                        pageInfo.PageCount = total;
                    }
                }
            }
         
            return pageInfo;
        }
    }
}
