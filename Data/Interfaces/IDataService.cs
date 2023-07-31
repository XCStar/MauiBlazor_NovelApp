using AngleSharp.Html.Parser;
using MauiApp3.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace MauiApp3.Data.Interfaces
{
    public interface IDataService
    {
        string BaseUrl { get; }
        IEnumerable<System.Collections.Generic.KeyValuePair<string, string>> NovelTypes { get; }
        Task<NovelPageInfo> GetNovelList(string type, int pageNum = 1);
        Task<NovelInfo> GetChapterList(string url, int pageNum = 1);
        Task<NovelContent> GetChapterContent(string url, string novelId, string novleName, string novelAddr);
        Task<NovelPageInfo> Search(string searchText);
        HttpClient GetHttpClient();
        string GetNovelTypeUrl(string type,int pageNum);
      

    }
}
