using AngleSharp.Html.Parser;
using MauiApp3.Common;
using MauiApp3.Data.Interfaces;
using MauiApp3.Model;
using System.Net;

namespace MauiApp3.Data.Impl
{
    /// <summary>
    ///有cloudflare保护需要页面js验证 等maui更新劫持webview的webclient验证，手动分析很麻烦暂不分析
    ///学习tls协议浏览器指纹验证，浏览器可访问，curl或httpclient无法访问需要tls1.3 http2.0及user-agent,accept-langue
    /// </summary>
    public class BQGService : INovelDataService
    {
        private readonly IHttpClientFactory httpClientFactory;
        private readonly IPageParser pageParser;
        private readonly string baseUrl = "https://m.beqege.cc";
        public BQGService(IHttpClientFactory httpClientFactory, Func<Type, IPageParser> parserFunc)
        {
            this.httpClientFactory = httpClientFactory;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12|SecurityProtocolType.Tls13;
            this.pageParser = parserFunc(typeof(BQGParser));
        }

        public async Task<NovelContent> GetChapter(string url, string novelId, string novleName, string novelAddr)
        {
            var novelContent = new NovelContent();
            var html = string.Empty;
            
            try
            {
                var client = httpClientFactory.CreateClient("bqg");
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
                    await DBHelper.Insert(novleName, novelId, novelAddr, novelContent.ChapterName, url, "bqg");
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
            pageNum=pageNum==0?1 : pageNum;
            var novelInfo = new NovelInfo();
            if (pageNum == 2)
            {
                url = url + "index_all.html";
            }
            novelInfo.CurrentPage = pageNum;
            var html = string.Empty;
            try
            {
                var client = httpClientFactory.CreateClient("bqg");
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

        public Task<NovelPageInfo> Search(string searchText)
        {
            return null;
        }

        public async Task<NovelPageInfo> VisitIndexPage(int pageNum = 1)
        {
            var pageInfo = new NovelPageInfo();
            var url = string.Format("/", pageNum);
            var html = string.Empty;
            try
            {
                var client = httpClientFactory.CreateClient("bqg");
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
            
            
            client.DefaultRequestVersion = HttpVersion.Version20;
            //client.DefaultRequestHeaders.Add("Cookie", "__cf_bm=aVRR5wYLYqRiiPalmWy6RWfVIwAtCsaqRmcnJHz5fSY-1681973322-0-AS33zB2n3+8gGvUR0zbUtZsCNuPe9VO/KZOY52jYauzOVRtHe/A9Qy+jyGtb1zpTirPW/HGDZc7z7zfr9JUwnVU=;");
            client.DefaultRequestHeaders.Add("accept-language", "zh-CN,zh;q=0.9,en;q=0.8");
            client.DefaultRequestHeaders.Add("User-Agent", "AppleWebKit/601.1.46 (KHTML, like Gecko) Version/9.0 Mobile/13B143 Safari/601.1 (compatible; Baiduspider-render/2.0; +http://www.baidu.com/search/spider.html)");
            client.DefaultRequestHeaders.Add("referer", baseUrl);
        }
    }
}
