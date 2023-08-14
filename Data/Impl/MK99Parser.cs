using AngleSharp.Html.Dom;
using MauiApp3.Data.Interfaces;
using MauiApp3.Model;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace MauiApp3.Data.Impl
{
    public class MK99Parser : BaseParser, IPageParser
    {
        public List<Chapter> ParseChapters(IHtmlDocument document)
        {
            var nodes = document.QuerySelectorAll(".book_last dd");
            var chapters = new List<Chapter>();

            foreach (var node in nodes)
            {
                var chapter = new Chapter();
                var aNode=node.QuerySelector("a");
                chapter.Name = aNode.TextContent;
                chapter.Url = aNode.Attributes["href"].Value;
                chapters.Add(chapter);
            }
            return chapters;
        }

        public NovelInfo ParseNovel(IHtmlDocument document)
        {
            var novelInfo=new NovelInfo();
            var node = document.QuerySelector(".book_box dl");
            if (node != null) 
            {
                novelInfo.Name = node.Children[0].TextContent;
                novelInfo.Author = node.Children[1]?.Children[0]?.TextContent?.Replace("作者：","");
                novelInfo.NovelType = node.Children[1]?.Children[1]?.TextContent?.Replace("分类：", "") ;
                novelInfo.UpdateState = node.Children[2]?.Children[0]?.TextContent.Replace("状态：", "");
                novelInfo.WordCount = node.Children[2]?.Children[1]?.TextContent.Replace("字数：", "");
                novelInfo.UpdateTime = node.Children[3]?.TextContent?.Replace("更新时间：", "");

            }
            novelInfo.Description = document.QuerySelector(".book_about dd").TextContent; ;
            return novelInfo;
        }

        public NovelContent ParseNovelContent(IHtmlDocument document)
        {
            var novelContent=new NovelContent();

            novelContent.ChapterName = document.QuerySelector(".title").TextContent;

            var contentNode= document.QuerySelector("#chaptercontent");
            var html = contentNode.InnerHtml;
            var childCount= contentNode.ChildElementCount;
            if (childCount > 3)
            {
                for (int i = 0; i < 3; i++)
                {
                    contentNode.RemoveChild(contentNode.ChildNodes[i]);
                }
            }
        
            novelContent.Content =contentNode.InnerHtml?
                .Replace("（本章未完，请点击下一页继续阅读）","")
                .Replace("手机版阅读网址：","")
                .Replace("wap.99mk.org","");
            novelContent.Next = document.QuerySelector("#pb_next").Attributes["href"].Value; 
            return novelContent;
        }

        public int ParseNovelPageCount(IHtmlDocument document)
        {
            return 1;
        }

        public NovelPageInfo ParseSearchUpdateInfo(IHtmlDocument document)
        {
            var novelPageInfo=new NovelPageInfo();

            novelPageInfo.Infos = new List<UpdateNovelInfo>();

            var nodes = document.QuerySelectorAll(".bookbox");
            foreach (var node in nodes)
            {
                var info=new UpdateNovelInfo();
                var aNode = node.QuerySelector(".bookname a");
                info.Name = aNode.TextContent;
                info.Url= aNode.Attributes["href"].Value;
                info.NovelType = node.QuerySelector(".cat")?.TextContent?.Replace("分类：", "");
                info.Author = node.QuerySelector(".author")?.TextContent?.Replace("作者：", "");
                aNode = node.QuerySelector(".update a");
                info.LastChapter = aNode.TextContent;
                info.LastChapterUrl = aNode.Attributes["href"].Value;
                novelPageInfo.Infos.Add(info);

            }
            return novelPageInfo;
        }

        public NovelPageInfo ParseUpdateInfo(IHtmlDocument document)
        {
            var novelPageInfo=new NovelPageInfo();
            var nodes = document.QuerySelectorAll(".lis li");
            novelPageInfo.Infos = new List<UpdateNovelInfo>();
            foreach (var node in nodes)
            {
                if (node.Children.Count() > 2)
                {
                    var info = new UpdateNovelInfo();
                    info.NovelType = node.Children[0].TextContent;
                    var aNode = node.Children[1].QuerySelector("a");
                    info.Name= aNode.TextContent;
                    info.Url = aNode.Attributes["href"].Value;
                    info.Author= node.Children[2].TextContent;
                    novelPageInfo.Infos.Add(info);
                }
             
            }
            novelPageInfo.PageCount = 1;
            var nextNode = document.QuerySelectorAll("a.a-btn").LastOrDefault();
            
            if (nextNode != null)
            {
                var href = nextNode.Attributes["href"].Value;
                var match = System.Text.RegularExpressions.Regex.Match(href, @".+\d+_(\d+).html");
                if (match.Success)
                {
                   var count= match.Groups[match.Groups.Count-1].Value;
                    if (int.TryParse(count, out int total))
                    {
                        novelPageInfo.PageCount = total;
                    }
                }
            }
            return novelPageInfo;
        }
    }
}
