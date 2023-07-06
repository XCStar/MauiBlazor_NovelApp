using AngleSharp.Dom;
using Esprima;
using MauiApp3.Common;
using MauiApp3.Data.Impl;
using MauiApp3.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace MauiApp3.Data.Interfaces
{
    public class SHU20Service : IDataService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IPageParser _pageParser;
        public string BaseUrl = "https://www.shu20.com/";
        private  static string novelType ="-1";

        string IDataService.BaseUrl => throw new NotImplementedException();

        public SHU20Service(IHttpClientFactory httpClientFactory,Func<string,IPageParser> func)
        {
            _httpClientFactory = httpClientFactory;
            _pageParser = func.Invoke(nameof(SHU20Parser));
           
        }
        public async Task<NovelContent> GetChapterContent(string url, string novelId, string novleName, string novelAddr)
        {
            var novelContent = new NovelContent();

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
                    await DBHelper.Insert(novleName, novelId, novelAddr, novelContent.ChapterName, url, nameof(SHU20Service));
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
            url = $"{url}{pageNum}/";
            var client = GetHttpClient();
            var html= await client.GetStringAsync(url);
            if (string.IsNullOrEmpty(html))
            {
                return new NovelInfo();
            }
            var document = _pageParser.LoadDocument(html);
            var novelInfo= new NovelInfo(); 
            novelInfo = _pageParser.ParseNovel(document);
            novelInfo.Chapters = _pageParser.ParseChapters(document);
            novelInfo.TotalPage = _pageParser.ParseNovelPageCount(document);
            novelInfo.CurrentPage = pageNum;
            return novelInfo;
        }

        public async Task<NovelPageInfo> Search(string searchText)
        {
            var url = $"/search/?searchkey={UrlEncoder.Default.Encode(searchText)}";
            var client = GetHttpClient();
            if (client.DefaultRequestHeaders.Contains("Cookie"))
            {
                client.DefaultRequestHeaders.Remove("Cookie");
            }
            var res=await client.GetAsync(url);
            if (res.StatusCode ==System.Net.HttpStatusCode.OK)
            {
                var content=await res.Content.ReadAsStringAsync();
                var setcookies = res.Headers.GetValues("Set-Cookie");
                var cookies = StringExtensions.GetCookie(setcookies);
                var token = cookies.Where(x => x.Key == "token").FirstOrDefault();
                var secret = cookies.Where(x => x.Key == "secret").FirstOrDefault();
                var s = int.Parse(secret.Value);
                client = GetHttpClient();
                var cookieStr = $"t={token.Value}; r={s - 100}; token={token.Value}; secret={s};";
                client.DefaultRequestHeaders.Add("Cookie",cookieStr);
                var html = await client.GetStringAsync($"/search/?searchkey={System.Text.Encodings.Web.UrlEncoder.Default.Encode(searchText)}");
                if (string.IsNullOrEmpty(html))
                {
                    return new NovelPageInfo();
                }
                var document = _pageParser.LoadDocument(html);
                var info = _pageParser.ParseSearchUpdateInfo(document);
                return info;
            }
            return new NovelPageInfo();
        }

        public async Task<NovelPageInfo> GetNovelList(int pageNum = 1)
        {
            if (novelType == "-1")
            {
                novelType = RandomTypeGeneroator();
            }
            var client = GetHttpClient();
            var html = await client.GetStringAsync(GetNovelTypeUrl(pageNum));
            if (string.IsNullOrEmpty(html))
            {
                return new NovelPageInfo();
            }
            var document = _pageParser.LoadDocument(html);
            var info= _pageParser.ParseUpdateInfo(document);
            info.CurrentPage = pageNum;
            info.PageCount= 20;
            return info;
        }
        public HttpClient GetHttpClient()
        {
            var client = _httpClientFactory.CreateClient(nameof(SHU20Service));
            client.BaseAddress = new Uri(BaseUrl);
            client.DefaultRequestHeaders.Add("User-Agent",HttpHeaderHelper.pcUserAgent);
            client.DefaultRequestHeaders.Add("Referer", BaseUrl);
            client.DefaultRequestHeaders.Add("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.7");
            client.DefaultRequestHeaders.Add("Accept-Language", "zh-CN,zh;q=0.9,en;q=0.8");
            return client;
        }
       

        public string GetNovelTypeUrl(int pageNum)
        {
            var url = "";
            if (novelType == "0")
            {
                url = "wuxia";
            }
            else if (novelType == "1")
            {
                url = "xuanhuan";
            }
            else
            {
                url = "kehuanhuan";
            }
            return $"/{url}/{pageNum}/";
        }

        public string RandomTypeGeneroator()
        {
            var type = Random.Shared.Next(0,3);
            return type.ToString();
        }
    }
}
