using AngleSharp.Dom;
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
    public class KSKParser : BaseParser, IPageParser
    {
        public List<Chapter> ParseChapters(IHtmlDocument document)
        {
            var  chapters=new List<Chapter>();

            var liNodes=document.QuerySelectorAll("div.chapters ul li");
            foreach (var liNode in liNodes) 
            {
                var linkNode = liNode.QuerySelector("a");
                if (linkNode != null) 
                {
                    var chapter = new Chapter();
                    chapter.Url = linkNode.Attributes["href"].Value;
                    chapter.Name = linkNode.Text();
                    chapters.Add(chapter);
                }
            }
            return chapters;
        }

        public NovelInfo ParseNovel(IHtmlDocument document)
        {
            var info=new NovelInfo();
            var ulNode= document.QuerySelector("ul.article");
            if (ulNode != null)
            {
                var liNodes = ulNode.QuerySelectorAll("li");
                if (liNodes.Count() > 2)
                {
                    info.Name = liNodes.ElementAt(0).Text();
                    var authorAndType = liNodes.ElementAt(1).Text();
                    var arr = authorAndType.Split("/", StringSplitOptions.RemoveEmptyEntries);
                    if (arr.Length == 2)
                    {
                        info.Author = arr[1];
                        info.NovelType = arr[0];
                    }
                    else
                    {
                        info.Author = authorAndType;
                    }
                    var linkeNode = liNodes.ElementAt(2).QuerySelector("a");
                    if (linkeNode != null)
                    {
                        info.LastChapter = linkeNode.Text();
                        info.LastChapterUrl = linkeNode.Attributes["href"].Value;
                        var span = linkeNode.QuerySelector("span");
                        if (span != null)
                        {
                            info.UpdateTime = span.Text().Replace("最近更新", "");
                        }
                    }

                }
            }
            var descriptionNode = document.QuerySelector("p.intro");
            if (descriptionNode != null)
            {

                info.Description = descriptionNode.Text();
            }
            return info;
        }

        public NovelContent ParseNovelContent(IHtmlDocument document)
        {
            var novelContent=new NovelContent();
            var contentNode = document.QuerySelector("div.content");
            if (contentNode != null)
            {
                novelContent.Content = contentNode.InnerHtml;
                
            }
            var nameNode=document.QuerySelector("div.articlename>h1");
            if (nameNode != null) 
            {
                novelContent.ChapterName = nameNode.Text();
            }
            var pageNode = document.QuerySelector("ul.bottom_page");
            if (pageNode != null)
            {
               var lastNode= pageNode.Children.Last();
                if (lastNode != null)
                {
                   var linkNode= lastNode.QuerySelector("a");
                    if (linkNode != null)
                    {

                        novelContent.Next = linkNode.Attributes["href"].Value;
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
           var novelPageInfo=new NovelPageInfo();
            novelPageInfo.Infos = new List<UpdateNovelInfo>();
            var liNodes = document.QuerySelectorAll("ul.result li");
            foreach ( var liNode in liNodes ) 
            {
                if (liNode.Children.Count() > 2)
                {
                    var info=new UpdateNovelInfo();
                    info.NovelType=  liNode.Children.ElementAt(0).Text();
                    var linkNode = liNode.Children.ElementAt(1);
                    if (linkNode!=null)
                    {
                        info.Name = linkNode.Text();
                        info.Url = linkNode.Attributes["href"].Value;
                    }
                    info.Author=liNode.Children.ElementAt(2).Text();
                    novelPageInfo.Infos.Add(info);
                }
            }
            return novelPageInfo;
        }

        public NovelPageInfo ParseUpdateInfo(IHtmlDocument document)
        {
            var novelPageInfo = new NovelPageInfo();
            novelPageInfo.Infos = new List<UpdateNovelInfo>();
            var ulNodels = document.QuerySelectorAll("ul.topul");
            if (ulNodels != null)
            {
                foreach (var ul in ulNodels)
                {
                    var liNode = ul.QuerySelector("li");
                    if (liNode.Children.Count() == 3)
                    {
                        var info = new UpdateNovelInfo();
                        var titleNode = liNode.Children.ElementAt(1).QuerySelector("a");
                        if (titleNode != null)
                        {
                            info.Name = titleNode.Text();
                            info.Url = titleNode.Attributes["href"].Value;
                        }
                        var infoNode = liNode.Children.ElementAt(2);
                        if (infoNode.Children.Count() == 3)
                        {
                            info.Description = infoNode.Children[0].Text();
                            info.Author = infoNode.Children[1].Text();
                            info.UpdateTime = infoNode.Children[2].Text();
                        }
                        novelPageInfo.Infos.Add(info);
                    }
                }
            }
            var node = document.QuerySelector("div.pagetips");
            if (node != null)
            {
                var text = node.Text();
                var match = Regex.Match(text, @"( 第 \d+ / (\d+) 页 )");
                if (match.Success)
                {
                    var page = match.Groups[2].Value;
                    if (int.TryParse(page, out int p))
                    {
                        novelPageInfo.PageCount = p;
                    }
                }
            }
            return novelPageInfo;
        }
    }
}
