using AngleSharp.Html.Parser;
using System.Net.Http.Json;

namespace MauiApp3.Data;

public class NewsService
{
	private static string newsUrl = @"/api/hot/item?id={0}";
	public static int[] ids = new int[] {1,3,69,18};
	private readonly IHttpClientFactory httpClientFactory;
    private readonly IFileSystem fileSystem;
    public NewsService(IHttpClientFactory httpClientFactory, IFileSystem fileSystem)
	{
		this.httpClientFactory= httpClientFactory;
        this.fileSystem = fileSystem;
	}
	public async Task<IList<News>> GetNewsByID(int id)
	{
		var client = httpClientFactory.CreateClient("my");
		client.BaseAddress = new Uri("https://momoyu.cc");
		client.DefaultRequestHeaders.Add("user-agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/108.0.0.0 Safari/537.36 Edg/108.0.1462.20");
		client.DefaultRequestHeaders.Add("cookie", "connect.sid=s:qRsLeEgbGAltNmW1AzN18GR2TdnaJkyM.2lH+DDicNQXjgaMu84ADIMnEOIE0uLDyz7/3+e3+ItY; Hm_lvt_b962bca679d1c121d780bffc64fa42a1=1668591616; Hm_lpvt_b962bca679d1c121d780bffc64fa42a1=1668591616");
		try
		{
            var data = await client.GetFromJsonAsync<ReceiveData>(string.Format(newsUrl, id));
            if (data.status == 100000)
            {
                return data.data.list;
            }
        }
		catch (Exception ex)
		{

			throw ex;
		}
		return new List<News>();
		

	}
    private string GetHtmlCache(string url)
    {
        var basePath = fileSystem.CacheDirectory;
        var distPath = Path.Combine(basePath, "cache_html");
        if (!Directory.Exists(distPath))
        {
            return string.Empty;
        }
        var fileName = MauiApp3.Common.StringExtensions.GetMd5(url);
        var filePath = Path.Combine(distPath, fileName);
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
    public async Task<IList<News>> GetNewByTopID(int id)
	{
        var list = new List<News>();
        var html = GetHtmlCache("top_html");
        if (string.IsNullOrEmpty(html))
        {
            try
            {
                var client = httpClientFactory.CreateClient("top");
                client.BaseAddress = new Uri("https://tophub.today");
                client.DefaultRequestHeaders.Add("user-agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/108.0.0.0 Safari/537.36 Edg/108.0.1462.20");
                html = await client.GetStringAsync("/");
                SetHtmlCache("top_html", html) ;
            }
            catch (Exception ex)
            {

                list.Add(new News { title = ex.Message, extra = "发生错误" });
            }
           
        }
        try
        {
          
            var parser = new HtmlParser();
            var document = parser.ParseDocument(html);
            var links = document.QuerySelectorAll($"#node-{id} div.cc-cd-cb-l a");
            foreach (var item in links)
            {
                var data = new News();
                data.link = item.Attributes["href"].Value;
                if (id == 132)
                {
                    var client = httpClientFactory.CreateClient("top");
                    //client.BaseAddress = new Uri("https://tophub.today");
                    client.DefaultRequestHeaders.Add("user-agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/108.0.0.0 Safari/537.36 Edg/108.0.1462.20");
                    var linkHtml = await client.GetStringAsync(data.link);
                    var  linkDocument = parser.ParseDocument(linkHtml);
                    data.link = "https://toutiao.io" + linkDocument.QuerySelector(".title>a").GetAttribute("href");
                }
                data.title = item.QuerySelector("span.t").TextContent;
                data.extra = item.QuerySelector("span.e").TextContent;
                list.Add(data);
            }

        }
        catch (Exception ex)
        {
            list.Add(new News { title=ex.Message,extra="发生错误"});
        }
        return list;
    }
	
}

