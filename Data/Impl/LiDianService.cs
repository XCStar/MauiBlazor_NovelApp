using MauiApp3.Common;
using MauiApp3.Data.Interfaces;
using MauiApp3.Model;
using System.Text.Encodings.Web;

namespace MauiApp3.Data.Impl
{

    public class LinDianService : INovelDataService
    {
        private static readonly string baseUrl = "http://www.lidapoly.com";
        private IPageParser pageParser;
        private IHttpClientFactory _httpClientFactory;
        
        public LinDianService(IHttpClientFactory httpClientFactory, Func<string, IPageParser> parserFunc)
        {
            _httpClientFactory = httpClientFactory;
            pageParser = parserFunc.Invoke(nameof(LinDianParser));
        }
        private HttpClient GetDefaultHeaderHttpClient()
        {
            var client = _httpClientFactory.CreateClient(nameof(LinDianService));
            client.BaseAddress = new Uri(baseUrl);
            client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (iPhone; CPU iPhone OS 13_2_3 like Mac OS X) AppleWebKit/605.1.15 (KHTML, like Gecko) Version/13.0.3 Mobile/15E148 Safari/604.1 Edg/113.0.0.0");
            return client;
        }

        public async Task<NovelContent> GetChapter(string url, string novelId, string novleName, string novelAddr)
        {
            var novelContent = new NovelContent();
            var html = string.Empty;

            try
            {
                var client = GetDefaultHeaderHttpClient();
                if (!url.StartsWith("/"))
                {
                    url = "/" + url;
                }
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
                    await DBHelper.Insert(novleName, novelId, novelAddr, novelContent.ChapterName, url, nameof(LinDianService));
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
            if (pageNum == 2)
            {
                url = url + "index_all.html";
            }
            novelInfo.CurrentPage = pageNum;
            var html = string.Empty;
            try
            {
                var client = GetDefaultHeaderHttpClient();
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
            //http://www.lidapoly.com踏星
            var pageInfo = new NovelPageInfo();
            var url = string.Format("/ar.php?keyWord={0}", UrlEncoder.Default.Encode(searchText));
            string html = string.Empty;
            var client = GetDefaultHeaderHttpClient();
            try
            {
                html = await client.GetStringAsync(url);

            }
            catch (Exception ex)
            {
                throw ex;
            }

            try
            {
                var document = pageParser.LoadDocument(html);
                pageInfo = pageParser.ParseSearchUpdateInfo(document);
                pageInfo.CurrentPage = 1;
                pageInfo.PageCount = pageParser.ParseNovelPageCount(document);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return pageInfo;
        }

        public async Task<NovelPageInfo> VisitIndexPage(int pageNum = 1)
        {
            var pageInfo = new NovelPageInfo();
            var url = string.Format("/", pageNum);
            var html = string.Empty;
            try
            {
                var client = GetDefaultHeaderHttpClient();
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
                pageInfo.PageCount = pageParser.ParseNovelPageCount(document);
                pageInfo.CurrentPage = pageNum;
              
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return pageInfo;
        }
    }
}
