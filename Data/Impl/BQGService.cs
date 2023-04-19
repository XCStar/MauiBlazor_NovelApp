using AngleSharp.Html.Parser;
using MauiApp3.Common;
using MauiApp3.Data.Interfaces;
using MauiApp3.Model;
namespace MauiApp3.Data.Impl
{

    public class BQGService : INovelDataService
    {
        private readonly IHttpClientFactory httpClientFactory;
        private readonly IPageParser pageParser;
        private readonly string baseUrl = "https://m.beqege.cc";
        public BQGService(IHttpClientFactory httpClientFactory, Func<Type, IPageParser> parserFunc)
        {
            this.httpClientFactory = httpClientFactory;
            
            this.pageParser = parserFunc(typeof(BQGParser));
        }

        public async Task<NovelContent> GetChapter(string url, string novelId, string novleName, string novelAddr)
        {
            var novelContent = new NovelContent();
            var html = string.Empty;
            
            try
            {
                var client = httpClientFactory.CreateClient("bqg");
                client.BaseAddress = new Uri(baseUrl);
                AddRequestHeader(client);
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
                    await DBHelper.Insert(novleName, novelId, novelAddr, novelContent.ChapterName, url, "bqg");
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

        public async Task<NovelInfo> GetNovel(string url, int pageNum = 1)
        {
            pageNum=pageNum==0?1 : pageNum;
            var novelInfo = new NovelInfo();
            if (pageNum == 2)
            {
                url = url + "index_all.html";
            }
            novelInfo.CurrentPage = pageNum;
            var html = string.Empty;
            try
            {
                var client = httpClientFactory.CreateClient("bqg");
                client.BaseAddress = new Uri(baseUrl);
                AddRequestHeader(client);
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
            if (pageNum == 1)
            {
                novelInfo = pageParser.ParseNovel(document);
            }

            novelInfo.Chapters = pageParser.ParseChapters(document);
            novelInfo.TotalPage = pageParser.ParseNovelPageCount(document);
            return novelInfo;
        }

        public Task<NovelPageInfo> Search(string searchText)
        {
            return null;
        }

        public async Task<NovelPageInfo> VisitIndexPage(int pageNum = 1)
        {
            var pageInfo = new NovelPageInfo();
            var url = string.Format("/", pageNum);
            var html = string.Empty;
            try
            {
                var client = httpClientFactory.CreateClient("bqg");
                client.BaseAddress = new Uri(baseUrl);
                AddRequestHeader(client);
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

        private void AddRequestHeader(HttpClient client)
        {
            client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (iPhone; CPU iPhone OS 13_2_3 like Mac OS X) AppleWebKit/605.1.15 (KHTML, like Gecko) Version/13.0.3 Mobile/15E148 Safari/604.1 Edg/113.0.0.0");
            client.DefaultRequestHeaders.Add("Cookie", "cf_clearance=bQo9beEWy_SSxhSW.pYnvuWrYi1La0xzpF4XGAHcPs8-1681896870-0-160; __cf_bm=zSuMYIfg3abeXF3S3JN6pXrUCLaYZr8EE4EFRSySPP4-1681901648-0-AQTzNf/YuXey57mkcyl7mQ0t7iBEkl2nj+Glwl/09Dzr6xxutEmzXXNRdLvEKzVe5jgyk2okKEGpin6Et7ZO56hl8LRTgoyUn7vAxw4FKdSc");
            client.DefaultRequestHeaders.Add("Accept-Agent", "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.7");
            client.DefaultRequestHeaders.Add("rerfer", "https://m.beqege.cc/");
        }
    }
}
