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
    public class BQGParser : BaseParser, IPageParser
    {
        public List<Chapter> ParseChapters(IHtmlDocument document)
        {
            var list=new List<Chapter>();
            var chaptersNodes = document.QuerySelectorAll("#chapterlist a");
            foreach (var chapterNode in chaptersNodes) 
            {
                var chapter = new Chapter();
                chapter.Url = chapterNode.Attributes["href"].Value;
                chapter.Name = chapterNode.Text();
                list.Add(chapter);
            }
            return list;
        }

        public NovelContent ParseNovelContent(IHtmlDocument document)
        { 
            var novelContent = new NovelContent();
            var chapterNode = document.QuerySelector("span.title");
            if (chapterNode != null)
            {
                novelContent.ChapterName = chapterNode.Text();
            }
             var conentNode=document.QuerySelector("#chaptercontent");
            if (conentNode != null)
            {
                novelContent.Content = conentNode.TextContent;
            }
            var nextNode = document.QuerySelector("a#pb_next");
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
            var novelInfo=new NovelInfo();
            var infoNode = document.QuerySelector("div.synopsisArea_detail");
            if (infoNode is not null)
            {
                var pNodes = infoNode.QuerySelectorAll("p") ;
                if (pNodes.Count() == 5)
                {
                    novelInfo.Author = pNodes.ElementAt(0).Text().Replace("作者：", "");
                    novelInfo.NovelType= pNodes.ElementAt(1).Text().Replace("类别：", "");
                    novelInfo.UpdateState = pNodes.ElementAt(2).Text().Replace("状态", "");
                    novelInfo.LastChapter = pNodes.ElementAt(3).Text();
                    novelInfo.UpdateTime = pNodes.ElementAt(4).Text();
                }
            }
            var descrptionNode = document.QuerySelector("p.review");
            if (descrptionNode != null)
            {
                novelInfo.Description = descrptionNode.Text();
            }
            return novelInfo;
        }
        public NovelPageInfo ParseUpdateInfo(IHtmlDocument document)
        {
            var novelPageInfo= new NovelPageInfo();
            var divNodes = document.QuerySelectorAll("div.bannerLink");
            if (divNodes!= null && divNodes.Count() > 0)
            {
                novelPageInfo.Infos = new List<UpdateNovelInfo>();
                foreach (var divNode in divNodes)
                {
                    var h2Node = divNode.QuerySelector("h2");
                    if (h2Node != null)
                    {
                        var novelTypeText = h2Node.TextContent;
                        if (novelTypeText.Contains("玄幻") || novelTypeText.Contains("仙侠") || novelTypeText.Contains("科幻"))
                        {
                            var pNodes = divNode.QuerySelectorAll("div.slide-item>p");
                            foreach (var item in pNodes)
                            {
                                var info = new UpdateNovelInfo();
                                var infoNodes = item.QuerySelectorAll("span");
                                if (infoNodes != null && infoNodes.Count() == 3)
                                {
                                    info.NovelType = infoNodes.ElementAt(0).Text();
                                    var nameNode=infoNodes.ElementAt(1).QuerySelector("a");
                                    info.Name = nameNode.Text();
                                    info.Url = nameNode.Attributes["href"].Value;
                                    info.Author = infoNodes.ElementAt(2).Text();
                                    novelPageInfo.Infos.Add(info);
                                }
                                
                            }
                        }
                    }

                }

            }
            novelPageInfo.CurrentPage = 1;
            novelPageInfo.PageCount = 1;
            
            return novelPageInfo;
        }
    }
}
