using AngleSharp.Dom;
using MauiApp3.Common;
using MauiApp3.Data.Interfaces;
using MauiApp3.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MauiApp3.Data.Impl
{
    public class BookBenService : IDataService
    {
        public string BaseUrl => "https://m.ibookben.com/";

        private List<KeyValuePair<string,string>>novelTypes=new List<KeyValuePair<string,string>>();
        public IEnumerable<KeyValuePair<string, string>> NovelTypes => novelTypes;

        private static readonly string searchUrl = "/search/";
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IPageParser _pageParser;

    

        public BookBenService(IHttpClientFactory httpClientFactory,Func<string,IPageParser> func) 
        {
            _httpClientFactory = httpClientFactory;
            _pageParser = func(nameof(BookBenParser));
            
            novelTypes.Add(new KeyValuePair<string,string>("玄幻", "xuanhuan"));
            novelTypes.Add(new KeyValuePair<string, string>("修真", "xiuzhen"));
            novelTypes.Add(new KeyValuePair<string, string>("科幻", "kehuan"));
        }
        public async Task<NovelContent> GetChapterContent(string url, string novelId, string novleName, string novelAddr)
        {
            var novelContent = new NovelContent();
            if (string.IsNullOrEmpty(url)||string.IsNullOrWhiteSpace(url))
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
                    await DBHelper.Insert(novleName, novelId, novelAddr, novelContent.ChapterName, url, nameof(BookBenService));
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
         
            var client = GetHttpClient();
            var html = string.Empty;
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
            var novelInfo = new NovelInfo();
            novelInfo = _pageParser.ParseNovel(document);
            var match = Regex.Match(url,$@"/txt/(\d+)\.html");
            var bookId = string.Empty;
            if (match != null && match.Groups.Count > 1)
            {
                bookId= match.Groups[1].Value;
            }
            var readUrl = $"/read/{bookId}/{pageNum}/";
            html= await client.GetStringAsync(readUrl);
            document = _pageParser.LoadDocument(html);
            novelInfo.Chapters = _pageParser.ParseChapters(document);
            novelInfo.TotalPage = _pageParser.ParseNovelPageCount(document);
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
            var res = await client.PostAsync(searchUrl, new FormUrlEncodedContent(new Dictionary<string, string> {
                {"searchkey",searchText }

            }));
            if (res.IsSuccessStatusCode)
            {
                var html = await res.Content.ReadAsStringAsync();
                var document = _pageParser.LoadDocument(html);
                return _pageParser.ParseSearchUpdateInfo(document);
            }
            return novelPageInfo;
        }

        public async Task<NovelPageInfo> GetNovelList(string type, int pageNum = 1)
        {
            var client = GetHttpClient();
            var url = GetNovelTypeUrl(type, pageNum);
            if (string.IsNullOrEmpty(url))
            {
                return new NovelPageInfo();
            }
            var html = await client.GetStringAsync(url);
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
            client.DefaultRequestHeaders.Add("User-Agent",HttpHeaderHelper.pcUserAgent);
            client.DefaultRequestHeaders.Add("Referer", BaseUrl);
            client.DefaultRequestHeaders.Add("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.7");
            client.DefaultRequestHeaders.Add("Accept-Language", "zh-CN,zh;q=0.9,en;q=0.8");
            return client;
        }
       
        public string GetNovelTypeUrl(string type,int pageNum)
        {
            if (novelTypes.Any(x => x.Value == type))
            {
                return $"/category/{type}/{pageNum}.html";
            }
            return string.Empty;
        }

       
    }
}
