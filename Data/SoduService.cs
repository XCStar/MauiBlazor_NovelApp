using AngleSharp.Dom;
using AngleSharp.Html.Parser;
using AngleSharp.Text;
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

namespace MauiApp3.Data
{
    public class SoduService
    {
        private readonly IHttpClientFactory httpClientFactory;
        private static readonly string baseUrl = "http://m.soduzw.com";
        private static readonly string firstUrl = "/top/monthvisit/{0}.html";
        private static readonly string serachUrl = "/search.html?searchkey={0}&searchtype=novelname";
        private readonly IFileSystem fileSystem;
        private static string conncetString = "{0}";
        private void InitDb()
        {
            var dbPath = Path.Combine(fileSystem.AppDataDirectory,"db");
            if (!Directory.Exists(dbPath))
            {
                Directory.CreateDirectory(dbPath);
            }
            conncetString = string.Format(conncetString, Path.Combine(dbPath, "data.db"));
            try
            {
                SQLitePCL.Batteries_V2.Init();
                using (var sql = new SQLiteConnection(conncetString))
                {
                    
                    sql.CreateTable<Records>();
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
            
        }
        public SoduService(IHttpClientFactory httpClientFactory,IFileSystem fileSystem)
        {
            this.httpClientFactory= httpClientFactory;
            this.fileSystem= fileSystem;
            InitDb();
        }
        public string Test()
        {
            try
            {
                InitDb();

            }
            catch (Exception ex)
            {

                return ex.Message+ex.StackTrace.ToString()+ex.InnerException?.Message+ex.InnerException?.StackTrace;
            }
            return "success";
        }
        public async Task<NovelPageInfo> GetVisit(int pageNum=1)
        {
            var pageInfo = new NovelPageInfo();
            pageInfo.CurrentPage= pageNum;
            pageInfo.Infos= new List<UpdateNovelInfo>(); 
            var url = string.Format(firstUrl,pageNum);
            var html=GetHtmlCache(url);
            if (string.IsNullOrEmpty(html))
            {
                var client = httpClientFactory.CreateClient("sodu");
                client.BaseAddress = new Uri(baseUrl);
                client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (iPhone; CPU iPhone OS 13_2_3 like Mac OS X) AppleWebKit/605.1.15 (KHTML, like Gecko) Version/13.0.3 Mobile/15E148 Safari/604.1 Edg/107.0.0.0");
                try
                {
                    html = await client.GetStringAsync(url);
                    SetHtmlCache(url, html);
                }
                catch (Exception ex)
                {

                    throw ex;
                }
              
            }
            try
            {
                var parser = new HtmlParser();
                var document = parser.ParseDocument(html);
                var list = document.QuerySelectorAll("div.list>ul>li");
                if (list.Length > 0)
                {
                    //get update info
                    foreach (var novel in list)
                    {
                        var novelInfo = new UpdateNovelInfo();
                        var nameNode = novel.Children[0];
                        novelInfo.Name = nameNode.TextContent;
                        novelInfo.Url =nameNode.Attributes["href"].Value;
                        var chapterNode = novel.Children[1];
                        novelInfo.LastChapter = chapterNode.TextContent;
                        var timeNode = novel.Children[2];
                        novelInfo.UpdateTime = timeNode.TextContent;
                        pageInfo.Infos.Add(novelInfo);

                    }
                }
              var optionNodes=   document.QuerySelectorAll("select.select option");
                if (optionNodes.Length > 0)
                {
                    var text= optionNodes.Last().TextContent;
                    var val= Regex.Match(text, @"\d+").Value;
                    if (int.TryParse(val, out int v))
                    {
                        pageInfo.PageCount = v;
                    }
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
         
             
            return pageInfo;
            //get next pages

        }
        private string GetHtmlCache(string url)
        {
            var basePath = fileSystem.CacheDirectory;
            var distPath = Path.Combine(basePath,"cache_html");
            if (!Directory.Exists(distPath))
            {
                return string.Empty;
            }
            var fileName = MauiApp3.Common.StringExtensions.GetMd5(url);
            var filePath= Path.Combine(distPath, fileName);
            if (!File.Exists(filePath)) 
            {
                return string.Empty;
            }
            using (var fs = File.Open(filePath, FileMode.Open, FileAccess.Read))
            {
                using (StreamReader reader = new StreamReader(fs))
                {
                    return reader.ReadToEnd();
                }
            }
        }
        private bool SetHtmlCache(string url, string text)
        {
            var basePath = fileSystem.CacheDirectory;
            var distPath = Path.Combine(basePath, "cache_html");
            if (!Directory.Exists(distPath))
            {
               Directory.CreateDirectory(distPath);
            }
            var fileName = MauiApp3.Common.StringExtensions.GetMd5(url);
            var filePath = Path.Combine(distPath, fileName);
            if (File.Exists(filePath))
            {
                return true;
            }
            using (var fs = File.Open(filePath, FileMode.OpenOrCreate, FileAccess.ReadWrite))
            {
                using (StreamWriter writer = new StreamWriter(fs))
                {
                    writer.Write(text);
                }
            }
            return true;
        }
        public async Task<NovelInfo> GetNovel(string url,int pageNum=1)
        {
            var novelInfo=new NovelInfo();
            if (pageNum > 1)
            {
                url = url.Replace(".html",$"_{pageNum}.html");
            }
            novelInfo.CurrentPage=pageNum;
            var html = GetHtmlCache(url);
            if (string.IsNullOrEmpty(html))
            {
                var client = httpClientFactory.CreateClient("sodu");
                client.BaseAddress = new Uri(baseUrl);
                client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (iPhone; CPU iPhone OS 13_2_3 like Mac OS X) AppleWebKit/605.1.15 (KHTML, like Gecko) Version/13.0.3 Mobile/15E148 Safari/604.1 Edg/107.0.0.0");
                try
                {
                    html = await client.GetStringAsync(url);
                    SetHtmlCache(url, html);
                }
                catch (Exception ex)
                {

                    throw ex;
                }
            }
            if (!string.IsNullOrEmpty(html))
            {
                var parser = new HtmlParser();
                var document = parser.ParseDocument(html);
                //parse novel info
                var infoNode = document.QuerySelector("div.info");
                novelInfo.Name= infoNode.Children[0].TextContent;
                novelInfo.Author = infoNode.Children[1].QuerySelector("a").TextContent;
                novelInfo.NovelType = infoNode.Children[2].QuerySelector("a").TextContent;
                novelInfo.LastChapter = infoNode.Children[3].TextContent;
                novelInfo.UpdateTime = infoNode.Children[4].TextContent;
                novelInfo.Description = document.QuerySelector("div.con").TextContent;
                novelInfo.Chapters = new List<Chapter>();
                //parse chapters
                var chapterNodes = document.QuerySelectorAll("div.list.mt10 li");
                foreach (var chapterNode in chapterNodes)
                {
                    var chapter = new Chapter();
                    var aNode = chapterNode.Children[0];
                    chapter.Name =aNode.TextContent;
                    chapter.Url = aNode.Attributes["href"].Value;   
                    novelInfo.Chapters.Add(chapter);
                }
               var optionNodes= document.QuerySelectorAll("select.select option");
               var value= optionNodes[optionNodes.Length-1].TextContent;
               var total= Regex.Match(value, @"\d+").Value;
                if (int.TryParse(total, out int v))
                {
                    novelInfo.TotalPage= v;
                }

            }
            return novelInfo;
        }
        public async Task<NovelContent> GetChapter(string url,string novelId,string novleName,string novelAddr)
        {
            var novelContent=new NovelContent();
           
            var html = GetHtmlCache(url);
            if (string.IsNullOrEmpty(html))
            {
                var client = httpClientFactory.CreateClient("sodu");
                client.BaseAddress = new Uri(baseUrl);
                client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (iPhone; CPU iPhone OS 13_2_3 like Mac OS X) AppleWebKit/605.1.15 (KHTML, like Gecko) Version/13.0.3 Mobile/15E148 Safari/604.1 Edg/107.0.0.0");
                try
                {
                    html = await client.GetStringAsync(url);
                    SetHtmlCache(url, html);
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
                    var parser = new HtmlParser();
                    var document = parser.ParseDocument(html);
                    //get chapater
                    var titleNode = document.QuerySelector("h1.title");
                    if (titleNode != null)
                    {
                        novelContent.ChapterName = titleNode.TextContent.Replace(novleName,"").TrimStart();
                    }
                    var node = document.QuerySelector("#pb_next").Attributes["href"];
                    if (node != null)
                    {
                        novelContent.Next = node.Value;
                    }
                    novelContent.Content = document.QuerySelector("div.articlecon").InnerHtml;
                    var id = await GetRecordByNovelId(novelId);
                    if (id > 0)
                    {
                        await DelByIds(new List<int> { id });
                    }
                    if (string.IsNullOrEmpty(novelContent.ChapterName))
                    {
                        novelContent.ChapterName = "最后阅读章节";
                    }
                    await Insert(novleName, novelId, novelAddr, novelContent.ChapterName, url);
                }
                catch (Exception ex)
                {
                    novelContent.Content = "已无下一页"+ex.Message;
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
            pageInfo.CurrentPage = 0;
            pageInfo.Infos = new List<UpdateNovelInfo>();
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
                var parser = new HtmlParser();
                var document = parser.ParseDocument(html);
                var list = document.QuerySelectorAll("div.list>ul>li");
                if (list.Length > 0)
                {
                    //get update info
                    foreach (var novel in list)
                    {
                        var novelInfo = new UpdateNovelInfo();
                        var nameNode = novel.Children[0];
                        novelInfo.Name = nameNode.TextContent;
                        novelInfo.Url = nameNode.Attributes["href"].Value;
                        var chapterNode = novel.Children[1];
                        novelInfo.LastChapter = chapterNode.TextContent;
                        var timeNode = novel.Children[2];
                        novelInfo.UpdateTime = timeNode.TextContent;
                        pageInfo.Infos.Add(novelInfo);

                    }
                }

            }
            catch (Exception ex)
            {

                throw ex;
            }


            return pageInfo;
            //get next pages

        }
        public bool ClearCache()
        {
            var basePath = fileSystem.CacheDirectory;
            var distPath = Path.Combine(basePath, "cache_html");
            if (Directory.Exists(distPath))
            {
               Directory.Delete(distPath, true);
            }
            return true;
        }
        private async Task<int> GetRecordByNovelId(string Id)
        {
            var sql = new SQLiteAsyncConnection(conncetString);

            {
               var id=  await sql.ExecuteScalarAsync<int>("select id from records where novelid=?", Id);
                return id;
            }    
        }
        public async Task<bool> DelByIds(List<int>Ids)
        {
            var sql = new SQLiteAsyncConnection(conncetString);
            {
               var row= await sql.Table<Records>().DeleteAsync(x => Ids.Contains(x.id));
                //for (int i = 0; i < Ids.Count; i++)
                //{

                //    var row = await sql.DeleteAsync(Ids[Ids[i]]);
                //}
                return row>0;
            }
        }
        private async Task<bool> Insert(string novelName,string novelId,string novelAddr,string chapterName,string chapterAddr)
        {
            var sql = new SQLiteAsyncConnection(conncetString);

            {
                var record = new Records { novelname=novelName,novelid=novelId,noveladdr=novelAddr,chaptername=chapterName,chapteraddr=chapterAddr};
                var row = await sql.InsertAsync(record);
                return row > 0;
            }
        }
        public async Task<List<Records>> GetAllRecords()
        {
            var sql = new SQLiteAsyncConnection(conncetString);

            {
               var list= await sql.QueryAsync<Records>("select * from records order by id desc");
                return list;
            }
            

        }
        public async Task<bool> DelAllRecord()
        {
            var sql = new SQLiteAsyncConnection(conncetString);

            {
                var row = await sql.ExecuteAsync("delete  from records");
                return row > 0;
            }
        }
    }
}
