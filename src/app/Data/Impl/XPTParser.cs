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
    public class XPTParser : BaseParser, IPageParser
    {
        public List<Chapter> ParseChapters(IHtmlDocument document)
        {
            var list=new List<Chapter>();
            var chapters= document.QuerySelectorAll(".chapter");
            var nodes=chapters.Last()?.QuerySelectorAll(".chapter li");
            for (int i = 0; i < nodes.Length - 1; i++)
            {
                var node = nodes[i];
                var chapter = new Chapter();
                var aNode = node.QuerySelector("a");
                chapter.Name = aNode.TextContent;
                chapter.Url = aNode.Attributes["href"].Value;
                list.Add(chapter);
            }

            return list;
        }

        public NovelInfo ParseNovel(IHtmlDocument document)
        {
            var novelInfo=new NovelInfo();
            var node = document.QuerySelector(".block_txt2");
            if (node != null&&node.Children.Length>5)
            {
                novelInfo.Name = node.Children[0].TextContent;
                novelInfo.Author = node.Children[1].QuerySelector("a")?.TextContent;
                novelInfo.NovelType = node.Children[2].QuerySelector("a")?.TextContent;
                novelInfo.UpdateState = node.Children[3].TextContent?.Replace("状态：", "");
                novelInfo.UpdateTime = node.Children[4].TextContent?.Replace("更新：", "");
                novelInfo.LastChapter = node.Children[5].QuerySelector("a").TextContent;
                novelInfo.LastChapterUrl = node.Children[5].QuerySelector("a")?.Attributes["href"]?.Value;

            }
            novelInfo.Description = document.QuerySelector(".intro_info")?.TextContent;
            return novelInfo;
        }

        public NovelContent ParseNovelContent(IHtmlDocument document)
        {
            var novelContent=new NovelContent();
            novelContent.ChapterName = document.QuerySelector("#nr_title").TextContent;
            novelContent.Next = "";
            if (document.QuerySelector("#lastchapter") == null)
            {

                novelContent.Next = document.QuerySelector("#pt_next").Attributes["href"].Value;
            }
           
            
            novelContent.Content = document.QuerySelector("#nr1").InnerHtml;
            return novelContent;
        }

        public int ParseNovelPageCount(IHtmlDocument document)
        {
            return 1;
        }

        public NovelPageInfo ParseSearchUpdateInfo(IHtmlDocument document)
        {
            var novelPageInfo = new NovelPageInfo();
            novelPageInfo.Infos = new List<UpdateNovelInfo>();
            var nodes = document.QuerySelectorAll("div.article");
            foreach (var node in nodes)
            {
                var info = new UpdateNovelInfo();
                info.Name = node.Children[0].TextContent;
                info.Url = node.Children[0].Attributes["href"].Value;
                novelPageInfo.Infos.Add(info);
            }
            novelPageInfo.PageCount = 1;
            return novelPageInfo;
        }

        public NovelPageInfo ParseUpdateInfo(IHtmlDocument document)
        {
            var novelPageInfo = new NovelPageInfo();
            novelPageInfo.Infos = new List<UpdateNovelInfo>();
            var nodes = document.QuerySelectorAll("div.article");
            foreach (var node in nodes)
            {
                var info = new UpdateNovelInfo();
                info.Name= node.Children[0].TextContent;
                info.Url = node.Children[0].Attributes["href"].Value;
                novelPageInfo.Infos.Add(info);
            }
            novelPageInfo.PageCount = 1;
            return novelPageInfo;
        }
    }
}
