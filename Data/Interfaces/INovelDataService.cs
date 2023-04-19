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
    public interface INovelDataService
    {
        Task<NovelPageInfo> VisitIndexPage(int pageNum = 1);
        Task<NovelInfo> GetNovel(string url, int pageNum = 1);
        Task<NovelContent> GetChapter(string url, string novelId, string novleName, string novelAddr);
        Task<NovelPageInfo> Search(string searchText);

    }
}
