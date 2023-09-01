using AngleSharp.Browser;
using AngleSharp.Dom;
using AngleSharp.Html.Dom;
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
    public class FSParser : BaseParser, IPageParser
    {
        public List<Chapter> ParseChapters(IHtmlDocument document)
        {
            var list=new List<Chapter>();
            var nodes = document.QuerySelectorAll(".chapter").LastOrDefault().QuerySelectorAll("li");
            foreach (var node in nodes) 
            {
                var chapter = new Chapter();
                var aNode=node.QuerySelector("a");
                if (aNode != null) 
                {
                    chapter.Url = aNode.Attributes["href"].Value;
                    chapter.Name = aNode.TextContent;
                }

                list.Add(chapter);
            }
            return list;
        }

        public NovelInfo ParseNovel(IHtmlDocument document)
        {
            var novelInfo=new NovelInfo();

            var node = document.QuerySelector("div .block_txt2");
            if (node != null)
            {
                if (node.Children.Length > 4)
                {
                    novelInfo.Name = node.Children[0].TextContent;
                    novelInfo.Author = node.Children[1].TextContent;
                    novelInfo.NovelType = node.Children[2].FirstChild?.TextContent;
                    novelInfo.UpdateState = node.Children[2].LastChild?.TextContent;
                    novelInfo.UpdateTime = node.Children[3].TextContent?.Replace("更新：", "");
                    var aNode = node.Children[4].QuerySelector("a");
                    if (aNode != null)
                    {
                        novelInfo.LastChapter=aNode.TextContent;
                        novelInfo.LastChapterUrl = aNode.Attributes["href"].Value;
                    }
                }

               
            }
            novelInfo.Description = document.QuerySelector(".intro_info")?.TextContent;
            return novelInfo;
        }

        public NovelContent ParseNovelContent(IHtmlDocument document)
        {
           var novelContent=new NovelContent();
            var titleNode = document.QuerySelector(".nr_title");
            novelContent.ChapterName = titleNode?.TextContent;
            var node = document.QuerySelector("#nr1");
            if (node.ChildElementCount > 2)
            {
                var length = node.Children.Length - 1;
                for (int i =length;i>length-2;i--)
                {
                    node.Children[i].Remove();
                }
            }
            novelContent.Content =node?.InnerHtml;
            novelContent.Next = document.QuerySelector(".c_tips")?.LastElementChild?.Attributes["href"].Value;
            return novelContent;
        }

        public int ParseNovelPageCount(IHtmlDocument document)
        {
            
            var node = document.QuerySelectorAll("div.listpage > span.middle option");
            if (node == null)
            {
                return 1;
            }
            return node.Length;
        }

        public NovelPageInfo ParseSearchUpdateInfo(IHtmlDocument document)
        {
            var novelPageInfo = new NovelPageInfo();

            novelPageInfo.Infos = new List<UpdateNovelInfo>();
            var nodes = document.QuerySelectorAll(".nbox");
            foreach (var node in nodes)
            {
                var info = new UpdateNovelInfo();
                if (node.Children.Length > 2)
                {
                    var aNode = node.Children[0].QuerySelector("a");
                    info.Name = aNode.TextContent;
                    info.Url = aNode.Attributes["href"].Value;
                    info.Description = node.Children[1].TextContent;
                    info.Author = node.Children[2].FirstChild?.TextContent;
                    info.NovelType = node.Children[2].QuerySelector(".ncate")?.TextContent;
                    info.UpdateState = node.Children[2].LastChild?.TextContent;

                }
                novelPageInfo.Infos.Add(info);
            }

            return novelPageInfo;
        }

        public NovelPageInfo ParseUpdateInfo(IHtmlDocument document)
        {
            var novelPageInfo=new NovelPageInfo();

            novelPageInfo.Infos = new List<UpdateNovelInfo>();
            var nodes = document.QuerySelectorAll(".nbox");
            foreach (var node in nodes)
            {
                var info = new UpdateNovelInfo();
                if (node.Children.Length >2)
                {
                    var aNode=node.Children[0].QuerySelector("a");
                    info.Name = aNode.TextContent;
                    info.Url = aNode.Attributes["href"].Value;
                    info.Description = node.Children[1].TextContent;
                    info.Author= node.Children[2].FirstChild?.TextContent;
                    info.NovelType = node.Children[2].QuerySelector(".ncate")?.TextContent;
                    info.UpdateState = node.Children[2].LastChild?.TextContent;

                }
             

                novelPageInfo.Infos.Add(info);
            }
            novelPageInfo.PageCount = 1;
            var pageNode = document.QuerySelector(".page").Children.Last();
            if (pageNode != null)
            {

                if (pageNode.HasAttribute("herf"))
                {
                    var href = pageNode.Attributes["href"].Value;
                    var match = Regex.Match(href, "/sort-\\d+-(\\d+)/");
                    if (match.Success)
                    {
                        var pageCount = match.Groups[match.Groups.Count - 1].Value;
                        if (int.TryParse(pageCount, out int count))
                        {
                            novelPageInfo.PageCount = count;
                        }
                    }

                }
            }
            return novelPageInfo;
        }
    }
}
