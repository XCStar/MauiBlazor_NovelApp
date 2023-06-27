using MauiApp3.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MauiApp3.Data.Interfaces
{
    public interface INovelService
    {
        Task<NovelPageInfo> GetNovelList(string serviceName,string type,int pageNum );
        Task<NovelPageInfo> Search(string serviceName, string searchText);
        Task<NovelContent> GetChapterContent(string serviceName,string chapterUrl, Novel novel);
        Task<NovelInfo> GetNovelChapterList(string serviceName, string url, int pageNum);
    }
}
