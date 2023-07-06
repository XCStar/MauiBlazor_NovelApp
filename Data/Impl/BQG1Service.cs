using AngleSharp.Browser;
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
using System.Text.Json;
using System.Threading.Tasks;

namespace MauiApp3.Data.Impl
{
    public class BQG1Service : IDataService
    {
        private readonly IHttpClientFactory httpClientFactory;
        private readonly IPageParser pageParser;
        public string BaseUrl => "http://www.biqugse.com";
        public BQG1Service(IHttpClientFactory httpClientFactory, Func<string, IPageParser> parserFunc)
        {
            this.httpClientFactory = httpClientFactory;
           
            this.pageParser = parserFunc(nameof(BQG1Parser));
        }

        public async Task<NovelContent> GetChapterContent(string url, string novelId, string novleName, string novelAddr)
        {
            var novelContent = new NovelContent();
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

            if (!string.IsNullOrEmpty(html))
            {
                try
                {
                    var document = pageParser.LoadDocument(html);
                    novelContent = pageParser.ParseNovelContent(document);
                    novelContent.ChapterName = novelContent.ChapterName;
                    if (novelContent.Content.Length > 3)
                    {
                        novelContent.Content = StringExtensions.HtmlFormat(novelContent.Content);
                    }
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
          
            var novelInfo= new NovelPageInfo(); ;
            if (string.IsNullOrEmpty(searchText))
            {
                return new NovelPageInfo();
            }
            var html = string.Empty;
            try
            {

                var client = GetHttpClient();
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

        public async Task<NovelPageInfo> GetNovelList(int pageNum = 1)
        {
            var pageInfo = new NovelPageInfo();
            var url = string.Format("/");
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
            //get next pages

        }

        

        public HttpClient GetHttpClient()
        {
            var client = httpClientFactory.CreateClient(nameof(BQG1Service));
            client.DefaultRequestHeaders.Add("Accept-Language", "zh-CN,zh;q=0.9,en;q=0.8");
            client.DefaultRequestHeaders.Add("User-Agent", HttpHeaderHelper.pcUserAgent);
            client.DefaultRequestHeaders.Add("referer",BaseUrl );
            client.BaseAddress = new Uri(BaseUrl);
            return client;
        }

        public string GetNovelTypeUrl(int pageNum)
        {
            throw new NotImplementedException();
        }

        public string RandomTypeGeneroator()
        {
            throw new NotImplementedException();
        }
    }
}
