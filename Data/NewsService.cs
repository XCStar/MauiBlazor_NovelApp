using System.Net.Http.Json;

namespace MauiApp3.Data;

public class NewsService
{
	private static string newsUrl = @"/api/hot/item?id={0}";
	public static int[] ids = new int[] {1,3,69,18};
	private readonly IHttpClientFactory httpClientFactory;
	public NewsService(IHttpClientFactory httpClientFactory)
	{
		this.httpClientFactory= httpClientFactory;
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
	
}

