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
    public class SHU20Parser : BaseParser, IPageParser
    {
        public List<Chapter> ParseChapters(IHtmlDocument document)
        {
            var chapters=new List<Chapter>();
            var ddNodes= document.QuerySelectorAll("dl.panel-chapterlist dd");
            if (ddNodes.Any())
            {
                foreach (var ddNode in ddNodes)
                {
                    var aNode=ddNode.QuerySelector("a");
                    if (aNode is not null)
                    {
                        var chapter = new Chapter();
                        chapter.Name = aNode.Text();
                        chapter.Url = aNode.Attributes["href"].Value;
                        chapters.Add(chapter);
                    }
                }
             
            }
            return chapters;
        }

        public NovelInfo ParseNovel(IHtmlDocument document)
        {
            var novelInfo = new NovelInfo();
           var nameNode=document.QuerySelector(".bookTitle a");
            if (nameNode is not null)
            {
                novelInfo.Name = nameNode.Text();
                novelInfo.Url= nameNode.Attributes["href"].Value;
            }
            var tagNode = document.QuerySelector(".booktag");
            if (tagNode.Children.Length > 3)
            {
                novelInfo.Author= tagNode.Children[0].Text();
                novelInfo.WordCount= tagNode.Children[1].Text();
                novelInfo.UpdateTime = tagNode.Children[2].Text();
                novelInfo.UpdateState = tagNode.Children[3].Text();
            }
            return novelInfo;
           
        }

        public NovelContent ParseNovelContent(IHtmlDocument document)
        {
            var novelContent = new NovelContent();
            var contentNode = document.QuerySelector("#chaptercontent");
            if (contentNode!=null)
            {
                novelContent.Content = contentNode.InnerHtml;
            }
            var titleNode = document.QuerySelector("h1.readTitle");
            if (titleNode is not null)
            {
                novelContent.ChapterName = titleNode.Text();
            }
            var nextNode = document.QuerySelector("a#next_url");
            if (nextNode is not null)
            {
                novelContent.Next = nextNode.Attributes["href"].Value;
            }
          
            return novelContent;
        }

        public int ParseNovelPageCount(IHtmlDocument document)
        {
            var node = document.QuerySelector("select#indexselect");
            return node.ChildElementCount;
        }

        public NovelPageInfo ParseSearchUpdateInfo(IHtmlDocument document)
        {
            var novelPageInfo=new NovelPageInfo();
            novelPageInfo.Infos = new List<UpdateNovelInfo>();
            var bookNodes = document.QuerySelectorAll("div.book-coverlist");
            foreach (var item in bookNodes)
            {
                var node = item.QuerySelector("div.caption");
                if (node.ChildElementCount > 0)
                {
                    var info=new UpdateNovelInfo();
                    var aNode=node.Children[0].QuerySelector("a");
                    if (aNode!=null)
                    {
                        info.Name= aNode.Text();
                        info.Url = aNode.Attributes["href"].Value;
                    }
                    if (node.Children.Count() > 1)
                    {
                        var authorNode = node.Children[1];
                        info.Author= authorNode.Text();
                    }
                    
                    if (node.Children.Count() > 2)
                    {
                        info.Description = node.Children[2].Text();
                    }
                    novelPageInfo.Infos.Add(info);
                }
            }
            return novelPageInfo;
        }

        public NovelPageInfo ParseUpdateInfo(IHtmlDocument document)
        {
            var novelPageInfo=new NovelPageInfo();
            novelPageInfo.Infos = new List<UpdateNovelInfo>();
            var trNodes = document.QuerySelectorAll("table.table tr");
            for (int i = 1; i < trNodes.Length; i++)
            {
                if (trNodes[i].Children.Length > 6)
                {
                    var info = new UpdateNovelInfo();
                    info.NovelType = trNodes[i].Children[0].Text();
                    var aNode = trNodes[i].Children[1].QuerySelector("a");
                    if (aNode != null)
                    {
                        info.Name=aNode.Text();
                        info.Url = aNode.Attributes["href"].Value;
                    }
                     aNode = trNodes[i].Children[2].QuerySelector("a");
                    if (aNode != null)
                    {
                        info.LastChapter = aNode.Text();
                        info.LastChapterUrl = aNode.Attributes["href"].Value;
                    }
                    info.Author = trNodes[i].Children[3].Text();
                    info.WordCount = trNodes[i].Children[4].Text();
                    info.UpdateTime = trNodes[i].Children[5].Text();
                    info.UpdateState = trNodes[i].Children[6].Text();
                    novelPageInfo.Infos.Add(info);
                    
                }
            }
            return novelPageInfo;
        }
    }
}
