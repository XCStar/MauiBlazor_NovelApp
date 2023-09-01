using MauiApp3.Common;
using MauiApp3.Data.Interfaces;
using MauiApp3.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace MauiApp3.Data.Impl
{
    public class FSService : IDataService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IPageParser _pageParser;
        public string BaseUrl => "https://m.feisxs.com/";
        private static string novelType = "-1";

        private List<KeyValuePair<string, string>> novelTypes = new List<KeyValuePair<string, string>>();
        public IEnumerable<KeyValuePair<string, string>> NovelTypes => novelTypes;
        public FSService(IHttpClientFactory httpClientFactory, Func<string, IPageParser> func)
        {
            _httpClientFactory = httpClientFactory;
            _pageParser = func.Invoke(nameof(FSParser));

            novelTypes.Add(new KeyValuePair<string, string>("奇幻玄幻", "1"));
            novelTypes.Add(new KeyValuePair<string, string>("武侠仙侠", "2"));
            novelTypes.Add(new KeyValuePair<string, string>("科幻灵异", "6"));

        }
        public async Task<NovelContent> GetChapterContent(string url, string novelId, string novleName, string novelAddr)
        {
            var novelContent = new NovelContent();
            if (url == novelAddr)
            {
                novelContent.ChapterName = "没有了";
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
                    await DBHelper.Insert(novleName, novelId, novelAddr, novelContent.ChapterName, url, nameof(FSService));
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
            url = $"{url.TrimEnd('/')}-{pageNum}/";
            var client = GetHttpClient();
            var html = await client.GetStringAsync(url);
            if (string.IsNullOrEmpty(html))
            {
                return new NovelInfo();
            }
            var document = _pageParser.LoadDocument(html);
            var novelInfo = new NovelInfo();
            novelInfo = _pageParser.ParseNovel(document);
            novelInfo.Chapters = _pageParser.ParseChapters(document);
            novelInfo.TotalPage = _pageParser.ParseNovelPageCount(document);
            novelInfo.CurrentPage = pageNum;
            return novelInfo;
        }

        public async Task<NovelPageInfo> Search(string searchText)
        {
            var url = $"/search.aspx?s={UrlEncoder.Default.Encode(searchText)}&submit=";
            var client = GetHttpClient();
            var html = await client.GetStringAsync(url);
            if (string.IsNullOrEmpty(html))
            {
                return new NovelPageInfo();
            }
            var document = _pageParser.LoadDocument(html);
            var info = _pageParser.ParseSearchUpdateInfo(document);
            return info;


        }

        public async Task<NovelPageInfo> GetNovelList(string type, int pageNum = 1)
        {

            var client = GetHttpClient();
            var html = await client.GetStringAsync(GetNovelTypeUrl(type, pageNum));
            if (string.IsNullOrEmpty(html))
            {
                return new NovelPageInfo();
            }
            var document = _pageParser.LoadDocument(html);
            var info = _pageParser.ParseUpdateInfo(document);
            info.CurrentPage = pageNum;
            info.PageCount = 20;
            return info;
        }
        public HttpClient GetHttpClient()
        {
            var client = _httpClientFactory.CreateClient(nameof(SHU20Service));
            client.BaseAddress = new Uri(BaseUrl);
            client.DefaultRequestHeaders.Add("User-Agent", HttpHeaderHelper.mobileUserAgent);
            client.DefaultRequestHeaders.Add("Referer", BaseUrl);
            client.DefaultRequestHeaders.Add("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.7");
            client.DefaultRequestHeaders.Add("Accept-Language", "zh-CN,zh;q=0.9,en;q=0.8");
            return client;
        }


        public string GetNovelTypeUrl(string type, int pageNum)
        {


            return $"/sort-{type}-{pageNum}";
        }

    }
}
