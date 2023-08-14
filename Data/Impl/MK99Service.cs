﻿
using MauiApp3.Common;
using MauiApp3.Data.Interfaces;
using MauiApp3.Model;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;

namespace MauiApp3.Data.Impl
{
    public class MK99Service : IDataService
    {
        private IPageParser parser;
        private IHttpClientFactory httpClientFactory;
        private string searchUrl = "/search.php";
        public MK99Service(Func<string,IPageParser> parserFunc,IHttpClientFactory clientFactory)
        {
            this.parser = parserFunc(nameof(MK99Parser));
            this.httpClientFactory = clientFactory;
        }
        public string BaseUrl => "http://wap.99mk.org/";

        public IEnumerable<KeyValuePair<string, string>> NovelTypes => novelTypes;
        private IEnumerable < KeyValuePair<string, string> > novelTypes = new List<KeyValuePair<string, string>>() 
        {
            new KeyValuePair<string, string>("玄幻","1"),
              new KeyValuePair<string, string>("武侠","2"),
                new KeyValuePair<string, string>("科幻","6"),
        };
    public async Task<NovelContent> GetChapterContent(string url, string novelId, string novleName, string novelAddr)
        {
            var novelContent = new NovelContent();
            var html = string.Empty;
            if (url == novelAddr)
            {
                novelContent.Content = "已无下一页";
            }
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
                    var document = parser.LoadDocument(html);
                    novelContent = parser.ParseNovelContent(document);
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
                    await DBHelper.Insert(novleName, novelId, novelAddr, novelContent.ChapterName, url, nameof(MK99Service));
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
            var document = parser.LoadDocument(html);
            if (document == null)
            {
                return novelInfo;
            }
            novelInfo = parser.ParseNovel(document);
            {
                var chapterUrl =url+"list.html";
                var client = GetHttpClient();
                html = await client.GetStringAsync(chapterUrl);
            }
            document = parser.LoadDocument(html);
            novelInfo.Chapters = parser.ParseChapters(document);
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
            var novelPageInfo = new NovelPageInfo();
            if (string.IsNullOrEmpty(searchText))
            {
                return novelPageInfo;
            }
            var client = GetHttpClient();
            var formData = new FormUrlEncodedContent(new Dictionary<string, string> {
              
               {"searchkey",searchText }
            });
            var html = string.Empty;
            try
            {
                var res = await client.PostAsync(searchUrl, formData);
                html = await res.Content.ReadAsStringAsync();
            }
            catch (Exception ex)
            {

                throw ex;
            }
            var document = parser.LoadDocument(html);
            novelPageInfo = parser.ParseSearchUpdateInfo(document);

            return novelPageInfo;

        }

        public async Task<NovelPageInfo> GetNovelList(string type, int pageNum = 1)
        {
            var pageInfo = new NovelPageInfo();

            var url = GetNovelTypeUrl(type, pageNum);
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
                var document = parser.LoadDocument(html);
                pageInfo = parser.ParseUpdateInfo(document);

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
            var client = httpClientFactory.CreateClient(nameof(KSKService));
            client.DefaultRequestHeaders.Add("User-Agent", HttpHeaderHelper.mobileUserAgent);
            client.DefaultRequestHeaders.Add("Accept-Language", "zh-CN,zh;q=0.9,en;q=0.8");
            client.DefaultRequestHeaders.Add("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.7");
            client.DefaultRequestHeaders.Add("Referer", BaseUrl);
            client.BaseAddress = new Uri(BaseUrl);
            return client;
        }

        public string GetNovelTypeUrl(string type, int pageNum)
        {
            if (NovelTypes.Any(x => x.Value == type))
            {
                return $"/type/{type}_{pageNum}.html";
            }
            return string.Empty;

        }

    }
}