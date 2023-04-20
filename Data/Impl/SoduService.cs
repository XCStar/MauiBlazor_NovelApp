using AngleSharp.Dom;
using AngleSharp.Html.Parser;
using AngleSharp.Text;
using MauiApp3.Common;
using MauiApp3.Data.Interfaces;
using MauiApp3.Model;
using SQLite;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MauiApp3.Data.Impl
{
    public class SoduService : INovelDataService
    {
        private readonly IHttpClientFactory httpClientFactory;
        private static readonly string baseUrl = "http://m.soduzw.com";
        private static readonly string firstUrl = "/top/monthvisit/{0}.html";
        private static readonly string serachUrl = "/search.html?searchkey={0}&searchtype=novelname";
        private IPageParser pageParser;

        public SoduService(IHttpClientFactory httpClientFactory, Func<Type,IPageParser> parserFunc)
        {
            this.httpClientFactory = httpClientFactory;
            this.pageParser = parserFunc(typeof(SoduParser));

        }

        public async Task<NovelPageInfo> VisitIndexPage(int pageNum = 1)
        {
            var pageInfo = new NovelPageInfo();
            var url = string.Format(firstUrl, pageNum);
            var html = FileCacheHelper.Get(url);
            if (string.IsNullOrEmpty(html))
            {
                var client = httpClientFactory.CreateClient("sodu");
                client.BaseAddress = new Uri(baseUrl);
                client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (iPhone; CPU iPhone OS 13_2_3 like Mac OS X) AppleWebKit/605.1.15 (KHTML, like Gecko) Version/13.0.3 Mobile/15E148 Safari/604.1 Edg/107.0.0.0");
                try
                {
                    html = await client.GetStringAsync(url);
                    FileCacheHelper.Save(url, html);
                }
                catch (Exception ex)
                {

                    throw ex;
                }

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
        public async Task<NovelInfo> GetNovel(string url, int pageNum = 1)
        {
            var novelInfo = new NovelInfo();
            if (pageNum > 1)
            {
                url = url.Replace(".html", $"_{pageNum}.html");
            }
            novelInfo.CurrentPage = pageNum;
            var html = FileCacheHelper.Get(url);
            if (string.IsNullOrEmpty(html))
            {
                var client = httpClientFactory.CreateClient("sodu");
                client.BaseAddress = new Uri(baseUrl);
                client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (iPhone; CPU iPhone OS 13_2_3 like Mac OS X) AppleWebKit/605.1.15 (KHTML, like Gecko) Version/13.0.3 Mobile/15E148 Safari/604.1 Edg/107.0.0.0");
                try
                {
                    html = await client.GetStringAsync(url);
                    FileCacheHelper.Save(url, html);
                }
                catch (Exception ex)
                {

                    throw ex;
                }
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
        public async Task<NovelContent> GetChapter(string url, string novelId, string novleName, string novelAddr)
        {
            var novelContent = new NovelContent();
            var html = FileCacheHelper.Get(url);
            if (string.IsNullOrEmpty(html))
            {
                var client = httpClientFactory.CreateClient("sodu");
                client.BaseAddress = new Uri(baseUrl);
                client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (iPhone; CPU iPhone OS 13_2_3 like Mac OS X) AppleWebKit/605.1.15 (KHTML, like Gecko) Version/13.0.3 Mobile/15E148 Safari/604.1 Edg/107.0.0.0");
                try
                {
                    html = await client.GetStringAsync(url);
                    FileCacheHelper.Save(url, html);
                }
                catch (Exception ex)
                {

                    throw ex;
                }
            }
            if (!string.IsNullOrEmpty(html))
            {
                try
                {
                    var document = pageParser.LoadDocument(html);
                    novelContent = pageParser.ParseNovelContent(document);
                    novelContent.ChapterName = novelContent.ChapterName?.Replace(novleName, "").TrimStart();
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
                    await DBHelper.Insert(novleName, novelId, novelAddr, novelContent.ChapterName, url,"sodu");
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
        public async Task<NovelPageInfo> Search(string searchText)
        {
            var pageInfo = new NovelPageInfo();
            var url = string.Format(serachUrl, searchText);
            string html = string.Empty;
            var client = httpClientFactory.CreateClient("sodu");
            client.BaseAddress = new Uri(baseUrl);
            client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (iPhone; CPU iPhone OS 13_2_3 like Mac OS X) AppleWebKit/605.1.15 (KHTML, like Gecko) Version/13.0.3 Mobile/15E148 Safari/604.1 Edg/107.0.0.0");
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
                var document= pageParser.LoadDocument(html);
                pageInfo=pageParser.ParseSearchUpdateInfo(document);
                pageInfo.CurrentPage = 1;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return pageInfo;
            //get next pages

        }


    }
}
