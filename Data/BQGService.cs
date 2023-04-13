using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MauiApp3.Data
{
   
    public class BQGService
    { 
        private readonly IHttpClientFactory httpClientFactory;
        private readonly HttpClient client;
        public BQGService(IHttpClientFactory httpClientFactory)
        {
            this.httpClientFactory = httpClientFactory;
            client = httpClientFactory.CreateClient("bqg");
            client.BaseAddress = new Uri(@"https://m.beqege.cc/");
        }
        
    }
}
