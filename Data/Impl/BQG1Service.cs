using AngleSharp.Dom;
using MauiApp3.Common;
using MauiApp3.Data.Interfaces;
using MauiApp3.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace MauiApp3.Data.Impl
{
    public class BQG1Service : INovelDataService
    {
        private readonly IHttpClientFactory httpClientFactory;
        private readonly IPageParser pageParser;
        private readonly string baseUrl = "http://www.biqugse.com";
        public BQG1Service(IHttpClientFactory httpClientFactory, Func<string, IPageParser> parserFunc)
        {
            this.httpClientFactory = httpClientFactory;
           
            this.pageParser = parserFunc(nameof(BQG1Parser));
        }

        public async Task<NovelContent> GetChapter(string url, string novelId, string novleName, string novelAddr)
        {
            var novelContent = new NovelContent();
            var html = string.Empty;

            try
            {
                var client = httpClientFactory.CreateClient(nameof(BQGService));
                client.BaseAddress = new Uri(baseUrl);
                AddRequestHeader(client);
                html = await client.GetStringAsync(url);

            }
            catch (Exception ex)
            {

                throw ex;
            }

            if (!string.IsNullOrEmpty(html))
            {
                try
                {
                    var document = pageParser.LoadDocument(html);
                    novelContent = pageParser.ParseNovelContent(document);
                    novelContent.ChapterName = novelContent.ChapterName;
                    //get chapater
                    var id = await DBHelper.GetRecordByNovelId(novelId);
                    if (id > 0)
                    {
                        await DBHelper.DelByIds(new List<int> { id });
                    }
                    if (string.IsNullOrEmpty(novelContent.ChapterName))
                    {
                        novelContent.ChapterName = "最后阅读章节";
                    }
                    await DBHelper.Insert(novleName, novelId, novelAddr, novelContent.ChapterName, url, nameof(BQG1Service));
                }
                catch (Exception ex)
                {
                    novelContent.Content = "已无下一页" + ex.Message;
                    novelContent.Next = "";
                    return novelContent;
                }

                //parse novel
            }

            return novelContent;
        }

        public async Task<NovelInfo> GetNovel(string url, int pageNum = 1)
        {
            pageNum = pageNum == 0 ? 1 : pageNum;
            var novelInfo = new NovelInfo();
            novelInfo.CurrentPage = pageNum;
            var html = string.Empty;
            try
            {
                var client = httpClientFactory.CreateClient(nameof(BQGService));
                client.BaseAddress = new Uri(baseUrl);
                AddRequestHeader(client);
                html = await client.GetStringAsync(url);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            var document = pageParser.LoadDocument(html);
            if (document == null)
            {
                return novelInfo;
            }
            if (pageNum == 1)
            {
                novelInfo = pageParser.ParseNovel(document);
            }

            novelInfo.Chapters = pageParser.ParseChapters(document);
            novelInfo.TotalPage = pageParser.ParseNovelPageCount(document);
            return novelInfo;
        }

        public async Task<NovelPageInfo> Search(string searchText)
        {
            //http://www.biqugse.com/case.php?m=search
            var novelInfo= new NovelPageInfo(); ;
            if (string.IsNullOrEmpty(searchText))
            {
                return new NovelPageInfo();
            }
            var html = string.Empty;
            try
            {
                var client = httpClientFactory.CreateClient(nameof(BQGService));
                client.BaseAddress = new Uri(baseUrl);
                AddRequestHeader(client);
                var form = new FormUrlEncodedContent(new Dictionary<string, string> { {"key",searchText } });
                var res = await client.PostAsync("/case.php?m=search",form);
                if (res.StatusCode == HttpStatusCode.OK)
                {
                    html=await res.Content.ReadAsStringAsync();

                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            if (string.IsNullOrEmpty(html))
            {
                return novelInfo;
            }
            var document = pageParser.LoadDocument(html);
            if (document == null)
            {
                return novelInfo;
            }
            novelInfo = pageParser.ParseSearchUpdateInfo(document);
            novelInfo.PageCount = 1;
            return novelInfo;
        }

        public async Task<NovelPageInfo> VisitIndexPage(int pageNum = 1)
        {
            var pageInfo = new NovelPageInfo();
            var url = string.Format("/");
            var html = string.Empty;
            try
            {
                var client = httpClientFactory.CreateClient(nameof(BQGService));
                client.BaseAddress = new Uri(baseUrl);
                AddRequestHeader(client);
                html = await client.GetStringAsync(url);
            }
            catch (Exception ex)
            {

                throw ex;
            }
            try
            {
                var document = pageParser.LoadDocument(html);
                pageInfo = pageParser.ParseUpdateInfo(document);
                pageInfo.CurrentPage = pageNum;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return pageInfo;
            //get next pages

        }

        private void AddRequestHeader(HttpClient client)
        {

            client.DefaultRequestHeaders.Add("accept-language", "zh-CN,zh;q=0.9,en;q=0.8");
            client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/114.0.0.0 Safari/537.36 Edg/114.0.1823.18");
            client.DefaultRequestHeaders.Add("referer", baseUrl);
        }
    }
}
