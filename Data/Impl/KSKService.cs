using AngleSharp.Browser;
using MauiApp3.Common;
using MauiApp3.Data.Interfaces;
using MauiApp3.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MauiApp3.Data.Impl
{
    public class KSKService : INovelDataService
    {
        private static readonly string searchUrl = "http://m.kuaishuku.net/s.php";
        private readonly IHttpClientFactory httpClientFactory;
        private readonly IPageParser parser;
        private readonly static string baseUrl = "http://m.kuaishuku.net";
        public KSKService(IHttpClientFactory httpClientFactory, Func<string, IPageParser> parserFunc)
        {
            this.httpClientFactory = httpClientFactory;
            this.parser =parserFunc(nameof(KSKParser)) ;
        }
        public async Task<NovelContent> GetChapter(string url, string novelId, string novleName, string novelAddr)
        {
            var novelContent = new NovelContent();
            var html = string.Empty;

            try
            {
                var client = httpClientFactory.CreateClient(nameof(KSKService));
                AddDefaultHeader(client);
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
                    await DBHelper.Insert(novleName, novelId, novelAddr, novelContent.ChapterName, url, nameof(KSKService));
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
            pageNum = pageNum == 0 ? 1 : pageNum;
            var novelInfo = new NovelInfo();
            novelInfo.CurrentPage = pageNum;
            var html = string.Empty;
            try
            {
                var client =httpClientFactory.CreateClient(nameof(KSKService));
                AddDefaultHeader(client);
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
            if (pageNum == 1)
            {
                novelInfo = parser.ParseNovel(document);
            }

            novelInfo.Chapters = parser.ParseChapters(document);
            novelInfo.TotalPage = parser.ParseNovelPageCount(document);
            return novelInfo;
        }

        public async Task<NovelPageInfo> Search(string searchText)
        {
            var novelPageInfo=new NovelPageInfo();
            if (string.IsNullOrEmpty(searchText))
            {
                return novelPageInfo;
            }
            var client= httpClientFactory.CreateClient(nameof(KSKService));
            AddDefaultHeader(client);
            var formData = new FormUrlEncodedContent(new Dictionary<string, string> {
                { "submit",""},
                {"type","articlename" },{"s",searchText }
            });
            var html = string.Empty;
            try
            {
                var res = await client.PostAsync(searchUrl, formData); 
                html=await res.Content.ReadAsStringAsync();
            }
            catch (Exception ex)
            {

                throw ex;
            }
            var document = parser.LoadDocument(html);
            novelPageInfo = parser.ParseSearchUpdateInfo(document);

            return novelPageInfo;
          
        }

        public async Task<NovelPageInfo> VisitIndexPage(int pageNum = 1)
        {
            var pageInfo = new NovelPageInfo();
            var url = string.Format($"/sort/2-{pageNum}.html");
            var html = string.Empty;
            try
            {
                var client = httpClientFactory.CreateClient(nameof(KSKService));
                AddDefaultHeader(client);
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
        private void AddDefaultHeader(HttpClient client)
        {
            client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Linux; Android 6.0; Nexus 5 Build/MRA58N) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/114.0.0.0 Mobile Safari/537.36 Edg/114.0.1823.24");
            client.DefaultRequestHeaders.Add("Accept-Language", "zh-CN,zh;q=0.9,en;q=0.8");
            client.DefaultRequestHeaders.Add("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.7");
            client.DefaultRequestHeaders.Add("Referer", baseUrl);
            client.BaseAddress =new Uri( baseUrl);
        }
    }
}
