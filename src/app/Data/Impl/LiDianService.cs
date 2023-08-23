using AngleSharp.Dom;
using MauiApp3.Common;
using MauiApp3.Data.Interfaces;
using MauiApp3.Model;
using System.Text.Encodings.Web;
using System.Text.Json;
using static System.Net.Mime.MediaTypeNames;

namespace MauiApp3.Data.Impl
{

    public class LinDianService : IDataService
    {
       public string BaseUrl => "http://www.lidapoly.com";
        private IPageParser pageParser;
        private IHttpClientFactory _httpClientFactory;

        private List<KeyValuePair<string, string>> novelTypes = new List<KeyValuePair<string, string>>();
        public IEnumerable<KeyValuePair<string, string>> NovelTypes => novelTypes;
        public LinDianService(IHttpClientFactory httpClientFactory, Func<string, IPageParser> parserFunc)
        {
            _httpClientFactory = httpClientFactory;
            pageParser = parserFunc.Invoke(nameof(LinDianParser));
            
            novelTypes.Add(new KeyValuePair<string, string>("玄幻奇幻", "1"));
            novelTypes.Add(new KeyValuePair<string, string>("武侠仙侠", "2"));
            novelTypes.Add(new KeyValuePair<string, string>("科幻灵异", "5"));
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
            int pageSize = RazorHelper.pageSize;
            var totalCount = 0;
            pageNum = pageNum == 0 ? 1 : pageNum;
            var novelInfo = new NovelInfo();
            if (pageNum > 1)
            {
                var chaptersJson = FileCacheHelper.Get(url);
                if (!string.IsNullOrEmpty(chaptersJson))
                {
                    novelInfo = JsonSerializer.Deserialize<NovelInfo>(chaptersJson);
                    totalCount = novelInfo.Chapters.Count;
                    novelInfo.Chapters = novelInfo.Chapters.Skip(pageSize * (pageNum - 1)).Take(pageSize).ToList();
                    novelInfo.CurrentPage = pageNum;
                    novelInfo.TotalPage = (totalCount + pageSize - 1) / pageSize;
                    return novelInfo;
                }
            }
            FileCacheHelper.DelFile(url);
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
            novelInfo.CurrentPage = pageNum;
            totalCount = novelInfo.Chapters.Count;
            if (totalCount > pageSize)
            {
                var json = JsonSerializer.Serialize(novelInfo);
                FileCacheHelper.Save(url, json);
                novelInfo.Chapters = novelInfo.Chapters.Skip((pageNum - 1) * pageSize).Take(pageSize).ToList();
            }
            novelInfo.TotalPage = (totalCount + pageSize - 1) / pageSize;
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

        public async Task<NovelPageInfo> GetNovelList(string type,int pageNum = 1)
        {
            var pageInfo = new NovelPageInfo();
            var url = GetNovelTypeUrl(type,pageNum);
            if (string.IsNullOrEmpty(url))
            {
                return pageInfo; 
            }
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

        public string GetNovelTypeUrl(string type,int pageNum)
        {
            if (novelTypes.Any(x => x.Value == type))
            {
                return $"/sort/{type}/{pageNum}.html";
            }
            return string.Empty;
        }

       
    }
}
