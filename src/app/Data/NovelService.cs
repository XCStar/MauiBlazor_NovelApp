using MauiApp3.Data.Interfaces;
using MauiApp3.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MauiApp3.Data
{
    public class NovelService : INovelService
    {
        private readonly Func<string, IDataService> services;
        public NovelService(Func<string, IDataService> services)
        {
                this.services= services;
        }

       
        public IEnumerable<System.Collections.Generic.KeyValuePair<string, string>> GetNoveTypes(string servieName)
        {
            return services(servieName).NovelTypes;
        }
        public async Task<NovelContent> GetChapterContent(string serviceName, string chapterUrl, Novel novel)
        {
            if (string.IsNullOrEmpty(chapterUrl))
            {
                return new NovelContent {ChapterName="提示页",Content="无下一页" };
            }
            var service = services(serviceName);
            return await service.GetChapterContent(chapterUrl,novel.Id,novel.Name,novel.Url);
        }

        public async Task<NovelInfo> GetNovelChapterList(string serviceName, string url, int pageNum)
        {
            var service = services(serviceName);
            return await service.GetChapterList(url, pageNum);
        }

        public async Task<NovelPageInfo> GetNovelList(string serviceName, string type, int pageNum)
        {
            var service = services(serviceName);
            return await service.GetNovelList(type, pageNum);
        }

        public async Task<NovelPageInfo> Search(string serviceName, string searchText)
        {
            var service = services(serviceName);
            return await service.Search(searchText);
        }
    }
}
