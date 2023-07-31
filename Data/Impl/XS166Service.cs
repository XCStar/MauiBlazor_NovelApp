using MauiApp3.Common;
using MauiApp3.Data.Interfaces;
using MauiApp3.Model;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace MauiApp3.Data.Impl
{
    public class XS166Service : IDataService
    {
        public string BaseUrl => "https://m.166xs.org/";

        private static readonly string searchUrl = "/search.php";
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IPageParser _pageParser;
        


        private List<KeyValuePair<string, string>> novelTypes = new List<KeyValuePair<string, string>>();
        public IEnumerable<KeyValuePair<string, string>> NovelTypes => novelTypes;
        public XS166Service(IHttpClientFactory httpClientFactory, Func<string, IPageParser> func)
        {
            _httpClientFactory = httpClientFactory;
            _pageParser = func(nameof(XS166Parser));
            novelTypes.Add(new KeyValuePair<string, string>("玄幻", "1"));
            novelTypes.Add(new KeyValuePair<string, string>("武侠", "2"));
            novelTypes.Add(new KeyValuePair<string, string>("科幻", "6"));
        }
        public async Task<NovelContent> GetChapterContent(string url, string novelId, string novleName, string novelAddr)
        {
            var novelContent = new NovelContent();
            if (string.IsNullOrEmpty(url) || string.IsNullOrWhiteSpace(url)|| novelAddr == url)
            {
                novelContent.Content = "没有了";
                novelContent.Next = "";
                return novelContent;
            }
            var html = string.Empty;
            var client = GetHttpClient();
            try
            {
                html = await client.GetStringAsync(url);
            }
            catch (Exception ex)
            {
                novelContent.Content = "发生错误" + ex.Message;
                novelContent.Next = "";
            }

            if (!string.IsNullOrEmpty(html))
            {
                try
                {
                    var document = _pageParser.LoadDocument(html);
                    novelContent = _pageParser.ParseNovelContent(document);
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
                    await DBHelper.Insert(novleName, novelId, novelAddr, novelContent.ChapterName, url, nameof(XS166Service));
                }
                catch (Exception ex)
                {
                    novelContent.Content = "已无下一页" + ex.Message;
                    novelContent.Next = "";
                    return novelContent;
                }
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
            var client = GetHttpClient();
            html = FileCacheHelper.Get(StringExtensions.GetMd5(url));
            if (string.IsNullOrEmpty(html))
            {
                html = await client.GetStringAsync(url);
            }
            if (string.IsNullOrEmpty(html))
            {
                return new NovelInfo();
            }
            var document = _pageParser.LoadDocument(html);
            novelInfo = _pageParser.ParseNovel(document);
            var readUrl = url;
            var match= Regex.Match(url,@"/(\d+)/");
            if (match.Success) 
            {
                var id = match.Groups[match.Groups.Count-1].Value;
                readUrl = $"/xiaoshuo/{id.AsSpan().Slice(0,3)}/{id}/all.html";
            }
            html = await client.GetStringAsync(readUrl);
            document = _pageParser.LoadDocument(html);
            novelInfo.Chapters = _pageParser.ParseChapters(document);
            totalCount = novelInfo.Chapters.Count;
            if (totalCount > pageSize)
            {
                var json = JsonSerializer.Serialize(novelInfo);
                FileCacheHelper.Save(url, json);
                novelInfo.Chapters = novelInfo.Chapters.Skip((pageNum - 1) * pageSize).Take(pageSize).ToList();
            }
            novelInfo.TotalPage = (totalCount + pageSize - 1) / pageSize;
            novelInfo.CurrentPage = pageNum;
            return novelInfo;
        }

        public async Task<NovelPageInfo> Search(string searchText)
        {
            var novelPageInfo = new NovelPageInfo();
            if (string.IsNullOrEmpty(searchText))
            {
                return novelPageInfo;
            }
            var client = GetHttpClient();
            var res = await client.GetStringAsync(string.Format("{0}?keyword={1}", searchUrl, UrlEncoder.Default.Encode(searchText)));
            var document = _pageParser.LoadDocument(res);
            return _pageParser.ParseSearchUpdateInfo(document);

        }

        public async Task<NovelPageInfo> GetNovelList(string type, int pageNum = 1)
        {
            
            var client = GetHttpClient();
            var html = await client.GetStringAsync(GetNovelTypeUrl(type,pageNum));
            if (string.IsNullOrEmpty(html))
            {
                return new NovelPageInfo();
            }
            var document = _pageParser.LoadDocument(html);
            var info = _pageParser.ParseUpdateInfo(document);
            info.CurrentPage = pageNum;
            return info;
        }
        public HttpClient GetHttpClient()
        {
            var client = _httpClientFactory.CreateClient(nameof(SHU20Service));
            client.BaseAddress = new Uri(BaseUrl);
            //articlevisited=1
            client.DefaultRequestHeaders.Add("User-Agent", HttpHeaderHelper.mobileUserAgent);
            client.DefaultRequestHeaders.Add("Referer", BaseUrl);
            client.DefaultRequestHeaders.Add("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.7");
            client.DefaultRequestHeaders.Add("Accept-Language", "zh-CN,zh;q=0.9,en;q=0.8");
            return client;
        }

        public string GetNovelTypeUrl(string type,int pageNum)
        {
            
            return $"/xclass/{type}/{pageNum}.html";
        }

       
    }
}
