using MauiApp3.Common;
using MauiApp3.Data.Interfaces;
using MauiApp3.Model;
using System.Text.Encodings.Web;
using static System.Net.Mime.MediaTypeNames;

namespace MauiApp3.Data.Impl
{

    public class LinDianService : IDataService
    {
       public string BaseUrl => "http://www.lidapoly.com";
        private IPageParser pageParser;
        private IHttpClientFactory _httpClientFactory;
        private static readonly int novelType;

        public LinDianService(IHttpClientFactory httpClientFactory, Func<string, IPageParser> parserFunc)
        {
            _httpClientFactory = httpClientFactory;
            pageParser = parserFunc.Invoke(nameof(LinDianParser));
        }
        

        public async Task<NovelContent> GetChapterContent(string url, string novelId, string novleName, string novelAddr)
        {
            var novelContent = new NovelContent();
            var html = string.Empty;

            try
            {
                var client = GetHttpClient();
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

        public async Task<NovelInfo> GetChapterList(string url, int pageNum = 1)
        {
            pageNum = pageNum == 0 ? 1 : pageNum;
            var novelInfo = new NovelInfo();
            novelInfo.CurrentPage = pageNum;
            var html = string.Empty;
            try
            {
                var client = GetHttpClient();
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
            novelInfo = pageParser.ParseNovel(document);
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
            var client = GetHttpClient();
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

        public async Task<NovelPageInfo> GetNovelList(int pageNum = 1)
        {
            var pageInfo = new NovelPageInfo();
            var url = GetNovelTypeUrl(pageNum);
            var html = string.Empty;
            try
            {
                var client = GetHttpClient();
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
        }
      

        public HttpClient GetHttpClient()
        {
            var client = _httpClientFactory.CreateClient(nameof(LinDianService));
            client.BaseAddress = new Uri(BaseUrl);
            client.DefaultRequestHeaders.Add("Referer", BaseUrl);
            client.DefaultRequestHeaders.Add("User-Agent",HttpHeaderHelper.mobileUserAgent);
            client.DefaultRequestHeaders.Add("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.7");
            client.DefaultRequestHeaders.Add("Accept-Language", "zh-CN,zh;q=0.9,en;q=0.8");
            return client;
        }

        public string GetNovelTypeUrl(int pageNum)
        {
            var url = "";
            if (novelType == 0)
            {
                url = "1";
            }
            else if (novelType == 1)
            {
                url = "2";
            }
            else
            {
                url = "5";
            }
            return $"/sort/{url}/{pageNum}.html";
        }

        public string RandomTypeGeneroator()
        {
            throw new NotImplementedException();
        }
    }
}
