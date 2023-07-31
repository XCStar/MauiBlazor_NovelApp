using MauiApp3.Common;
using MauiApp3.Data.Interfaces;
using MauiApp3.Model;
using Microsoft.Maui.Animations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MauiApp3.Data.Impl
{
    public class QXSService : IDataService
    {
        public string BaseUrl => "http://m.81ht.com/";

        private static readonly string searchUrl = "/search.htm";
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IPageParser _pageParser;
       

        private List<KeyValuePair<string, string>> novelTypes = new List<KeyValuePair<string, string>>();
        public IEnumerable<KeyValuePair<string, string>> NovelTypes => novelTypes;
        public QXSService(IHttpClientFactory httpClientFactory, Func<string, IPageParser> func)
        {
            _httpClientFactory = httpClientFactory;
            _pageParser = func(nameof(QXSParser));
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
            novelTypes .Add(new KeyValuePair<string, string>("玄幻", "xuanhuanxiaoshuo"));
            novelTypes .Add(new KeyValuePair<string, string>("奇幻", "qihuanxiaoshuo"));
            novelTypes .Add(new KeyValuePair<string, string>("修真", "xiuzhenxiaoshuo"));
            novelTypes .Add(new KeyValuePair<string, string>("武侠", "wuxiaxiaoshuo"));
            novelTypes .Add(new KeyValuePair<string, string>("科幻", "kehuanxiaoshuo"));
        }
        public async Task<NovelContent> GetChapterContent(string url, string novelId, string novleName, string novelAddr)
        {
            var novelContent = new NovelContent();
            if (string.IsNullOrEmpty(url) || string.IsNullOrWhiteSpace(url) || url.Contains("end.htm"))
            {
                novelContent.Content = "没有了";
                novelContent.Next = "";
                return novelContent;
            }
            var html = string.Empty;
            var client = GetHttpClient();
            try
            {
                var res = await client.GetAsync(url);

                var bytes = await res.Content.ReadAsByteArrayAsync();
                html = Encoding.GetEncoding("gbk").GetString(bytes);
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
                    await DBHelper.Insert(novleName, novelId, novelAddr, novelContent.ChapterName, url, nameof(QXSService));
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
          
            pageNum = pageNum == 0 ? 1 : pageNum;
            var novelInfo = new NovelInfo();
            var html = string.Empty;
            html = FileCacheHelper.Get(StringExtensions.GetMd5(url));
            var client = GetHttpClient();
            if (string.IsNullOrEmpty(html))
            {
                var res= await client.GetAsync(url);
                var data = await res.Content.ReadAsByteArrayAsync();
                html = Encoding.GetEncoding("gbk").GetString(data);
            }
            if (string.IsNullOrEmpty(html))
            {
                return new NovelInfo();
            }
            var document = _pageParser.LoadDocument(html);
            novelInfo = _pageParser.ParseNovel(document);
            var readUrl = url;
            var match = Regex.Match(url, @"/book-(\d+)/");
            if (match.Success)
            {
                var id = match.Groups[match.Groups.Count - 1].Value;
                readUrl = $"/read-{id}_{pageNum}/";
            }
            var respose = await client.GetAsync(readUrl);
            var bytes = await respose.Content.ReadAsByteArrayAsync();
            html = Encoding.GetEncoding("gbk").GetString(bytes);
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
            var res = await client.GetAsync(string.Format("{0}?keyword={1}", searchUrl, UrlEncoder.Default.Encode(searchText)));
            var bytes= await res.Content.ReadAsByteArrayAsync();
            var html = Encoding.GetEncoding("gbk").GetString(bytes);
            var document = _pageParser.LoadDocument(html);
            return _pageParser.ParseSearchUpdateInfo(document);

        }

        public async Task<NovelPageInfo> GetNovelList(string type,int pageNum = 1)
        {
           
            var client = GetHttpClient();
            var html = string.Empty;
            try
            {
                 //html = await client.GetStringAsync(GetNovelTypeUrl(pageNum));
                var res= await client.GetAsync(GetNovelTypeUrl(type,pageNum));
                var bytes = await res.Content.ReadAsByteArrayAsync();
                html = Encoding.GetEncoding("gbk").GetString(bytes);
                ;
            }
            catch (Exception ex)
            {

                throw ex;
            }
           
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
            var client = _httpClientFactory.CreateClient(nameof(KSKService));
            client.BaseAddress = new Uri(BaseUrl);
            //articlevisited=1
            //client.DefaultRequestVersion=new Version(1,1);
            //client.DefaultVersionPolicy = HttpVersionPolicy.RequestVersionOrLower;
            //client.DefaultRequestHeaders.Add("Transfer-Encoding", "chunked");
            client.DefaultRequestHeaders.Add("Connection", "close");
            client.DefaultRequestHeaders.Add("User-Agent", HttpHeaderHelper.mobileUserAgent);
            client.DefaultRequestHeaders.Add("Referer", BaseUrl);
            client.DefaultRequestHeaders.Add("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.7");
            client.DefaultRequestHeaders.Add("Accept-Language", "zh-CN,zh;q=0.9,en;q=0.8");
            return client;
        }

        public string GetNovelTypeUrl(string type,int pageNum)
        {
            if (novelTypes.Any(x => x.Value == type))
            {
                return $"/{type}/{pageNum}.htm";
            }
            return string.Empty;
        }

       
    }
}
