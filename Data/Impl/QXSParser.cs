using AngleSharp.Html.Dom;
using MauiApp3.Common;
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
    public class QXSParser : BaseParser, IPageParser
    {
        public List<Chapter> ParseChapters(IHtmlDocument document)
        {
            var list= new List<Chapter>();
            var nodes = document.QuerySelectorAll("#chapterlist li");
            foreach (var node in nodes) 
            {
                var chapter = new Chapter();
                var aNode = node.QuerySelector("a");
                chapter.Name = aNode?.TextContent;
                chapter.Url = aNode?.Attributes["href"].Value;
                list.Add(chapter);
            }
            return list;
        }

        public NovelInfo ParseNovel(IHtmlDocument document)
        {
           var novelInfo=new NovelInfo();
            var node = document.QuerySelector(".info");
            if (node != null) 
            {
                novelInfo.Name = node.Children[0].TextContent;
                if (node.Children[1].Children.Count() > 3)
                {
                    var cNode = node.Children[1];
                    novelInfo.Author = cNode.Children[0].TextContent?.Replace("作者：", "");
                    novelInfo.NovelType = cNode.Children[1].TextContent;
                    novelInfo.UpdateState = cNode.Children[2].TextContent?.Replace("状态：", "");
                    var aNode = cNode.Children[3].QuerySelector("a");
                    novelInfo.LastChapter = aNode?.TextContent;
                    novelInfo.LastChapterUrl = aNode?.Attributes["href"]?.Value;
                }
            }
            var desNode = document.QuerySelector("#all");
            if (desNode == null)
            {
                desNode = document.QuerySelector("#shot");
            }
            novelInfo.Description =desNode?.TextContent?.Replace("[收起]","");
            return novelInfo;
        }

        public NovelContent ParseNovelContent(IHtmlDocument document)
        {
            var novelContent=new NovelContent();
            novelContent.ChapterName = document.QuerySelector(".title")?.TextContent;
            var node= document.QuerySelector("div.text");
            if (node != null&&node.Children.Count()>2) 
            {
                var child1 = node.Children[0];
                var child2 = node.Children[1];
                node.RemoveChild(child1);
                node.RemoveChild(child2);
            }
            
            novelContent.Content = node?.InnerHtml;
            if (!novelContent.Content.Contains("</br>"))
            {
                novelContent.Content = StringExtensions.HtmlFormat(novelContent.Content);
            }
            var pNode = document.QuerySelector(".navigator-nobutton ul");
            var nextNode = pNode.LastElementChild;
            var aNode=nextNode.QuerySelector("a");
            novelContent.Next = aNode?.Attributes["href"].Value;
            return novelContent;
        }

        public int ParseNovelPageCount(IHtmlDocument document)
        {
            var node = document.QuerySelector("#pageinput");
            var text = node.Parent.LastChild.TextContent;
            var match = Regex.Match(text, @"\d+/(\d+)");

            if (match.Success)
            {
                var page = match.Groups[match.Groups.Count - 1].Value;
                if (int.TryParse(page, out int total))
                {
                    return total;
                }
            }
            return 1;
        }

        public NovelPageInfo ParseSearchUpdateInfo(IHtmlDocument document)
        {

            var novelPageInfo = new NovelPageInfo();
            var nodes = document.QuerySelectorAll(".book_textList2 li");
            novelPageInfo.Infos = new List<UpdateNovelInfo>();
            foreach (var node in nodes)
            {

                if (node.Children.Count() > 4)
                {

                    var info = new UpdateNovelInfo();
                    info.NovelType = node.Children[0].TextContent;
                    info.Url = node.Children[1]?.Attributes["href"].Value;
                    var text = node.Children[1].TextContent;
                    info.Name = text;
                    info.UpdateTime = node.Children[2].TextContent;
                    info.LastChapter = node.Children[4]?.TextContent;
                    info.LastChapterUrl = node.Children[4]?.Attributes["href"]?.Value;
                    novelPageInfo.Infos.Add(info);
                }

            }
            novelPageInfo.PageCount = 1;
            return novelPageInfo;
        }

        public NovelPageInfo ParseUpdateInfo(IHtmlDocument document)
        {
            var novelPageInfo=new NovelPageInfo();
            var nodes = document.QuerySelectorAll(".book_textList2 li");
            novelPageInfo.Infos = new List<UpdateNovelInfo>();
            foreach ( var node in nodes ) 
            {
                
                if (node.Children.Count() > 4)
                {
                    var info = new UpdateNovelInfo();
                    info.NovelType = node.Children[0].TextContent;
                    info.Url = node.Children[1]?.Attributes["href"].Value;
                    var text = node.Children[1].TextContent;
                    info.Name = text;
                    info.UpdateTime = node.Children[2].TextContent;
                    info.LastChapter = node.Children[4]?.TextContent;
                    info.LastChapterUrl = node.Children[4]?.Attributes["href"]?.Value;
                    novelPageInfo.Infos.Add(info);
                }
               
            }
            novelPageInfo.PageCount = 1;
            var pagination = document.QuerySelector(".pagination span");
            if (pagination != null)
            {
                var pageStr = pagination.TextContent;
                var match = Regex.Match(pageStr,@"\d+/(\d+)");
                if (match.Success)
                {
                    var page = match.Groups[match.Groups.Count - 1].Value;
                    if (int.TryParse(page, out int total))
                    {
                        novelPageInfo.PageCount = total;
                    }
                }
            }
            return novelPageInfo;
        }
    }
}
