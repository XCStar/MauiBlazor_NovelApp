using Jint;
using MauiApp3.Common;
using MauiApp3.Model;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Text;
using System.Text.Json;

namespace MauiApp3.Data
{
    public class DYService
    {
        private List<DYInfo> videos=new List<DYInfo>();
        private readonly IHttpClientFactory _httpClientFactory;
        private int PageIndex = 1;
        private int CurrentIndex = 1;
        private static readonly object obj=new object();
        public DYService(IHttpClientFactory httpClientFactory)
        {
            this._httpClientFactory = httpClientFactory;

        }
        public async Task<DYInfo> GetNextUrlAsync(bool flag=true)
        {
            if (videos.Count == 0)
            {
                await Fetch(true);
            }
            if (CurrentIndex < videos.Count)
            {
                if (CurrentIndex == videos.Count - 2)
                {
                    Fetch();
                }
                return videos[CurrentIndex++];
            }
            if (flag)
            {
                return await GetNextUrlAsync(!flag);
            }
            return new DYInfo();
         
        }
        public DYInfo GetLast()
        {
            if (CurrentIndex > 1 && videos.Count >= CurrentIndex)
            {
                CurrentIndex = CurrentIndex - 2;
                return videos[CurrentIndex++];
            }
            return new DYInfo();
        }
        private void Add(Aweme_List[] aweme_Lists) 
        {
            foreach (var item in aweme_Lists)
            {
                if (item.is_ads)
                {
                    continue;
                }
                
                if (item.video != null)
                {
                    if (item.video.bit_rate != null && item.video.bit_rate.Length > 0)
                    {
                        if (item.video.bit_rate.Any(x => x.gear_name == "normal_1080_0"))
                        {
                            var bitRate = item.video.bit_rate.Where(x => x.gear_name == "normal_1080_0").FirstOrDefault();
                            if (bitRate != null)
                            {
                                var url = bitRate.play_addr.url_list.LastOrDefault().Replace("\\u0026", "&");
                                var uri = new Uri(url);
                                JavaScriptConfig.userAgentHosts.Add(uri.Host);
                                videos.Add(new DYInfo(item.aweme_id,item.desc, url,item.video.duration,DateTimeOffset.FromUnixTimeSeconds(item.create_time).ToString("yyyy-MM-dd"),item.author?.sec_uid, item.author?.uid,item.author?.nickname));
                                continue;
                            }
                        }
                    }
                   if (aweme_Lists[CurrentIndex].video.play_addr != null && aweme_Lists[CurrentIndex].video.play_addr.url_list != null && aweme_Lists[CurrentIndex].video.play_addr.url_list.Length > 0)
                    {
                        var url = item.video.play_addr.url_list.LastOrDefault().Replace("\\u0026", "&");
                        var uri=new Uri(url);
                        JavaScriptConfig.userAgentHosts.Add(uri.Host);
                        videos.Add(new DYInfo(item.aweme_id, item.desc,url, item.video.duration, DateTimeOffset.FromUnixTimeSeconds(item.create_time).ToString("yyyy-MM-dd"), item.author?.sec_uid, item.author?.uid, item.author?.nickname));
                        continue;
                    }
                    if (aweme_Lists[CurrentIndex].video.play_addr_265 != null && aweme_Lists[CurrentIndex].video.play_addr_265.url_list != null && aweme_Lists[CurrentIndex].video.play_addr_265.url_list.Length > 0)
                    {
                        var url = item.video.play_addr_265.url_list.LastOrDefault().Replace("\\u0026", "&");
                        var uri = new Uri(url);
                        JavaScriptConfig.userAgentHosts.Add(uri.Host);
                        videos.Add(new DYInfo(item.aweme_id, item.desc, url, item.video.duration, DateTimeOffset.FromUnixTimeSeconds(item.create_time).ToString("yyyy-MM-dd"), item.author?.sec_uid, item.author?.uid, item.author?.nickname));
                        continue;
                    }
                    if (aweme_Lists[CurrentIndex].video.play_addr_h264 != null && aweme_Lists[CurrentIndex].video.play_addr_h264.url_list != null && aweme_Lists[CurrentIndex].video.play_addr_h264.url_list.Length > 0)
                    {
                        var url = item.video.play_addr_h264.url_list.LastOrDefault().Replace("\\u0026", "&");
                        var uri = new Uri(url);
                        JavaScriptConfig.userAgentHosts.Add(uri.Host);
                        videos.Add(new DYInfo(item.aweme_id, item.desc, url, item.video.duration, DateTimeOffset.FromUnixTimeSeconds(item.create_time).ToString("yyyy-MM-dd"), item.author?.sec_uid, item.author?.uid, item.author?.nickname));
                        continue;
                    }
                }
            }
           
        }

        /// <summary>
        /// 没有ttwid全是重复的视频
        /// ttcid 和tt_scid 可以不需要
        /// </summary>
        /// <param name="isFirst"></param>
        /// <returns></returns>
        private async Task Fetch(bool isFirst=false)
        {
            var client = _httpClientFactory.CreateClient(nameof(DYService));
            client.DefaultRequestHeaders.Add("Accept", "*/*");
            client.DefaultRequestHeaders.Add("Accept-Language", "zh-CN,zh;q=0.9,en;q=0.8");
            client.DefaultRequestHeaders.Add("User-Agent", DYConfig.userAgent);
            if (isFirst)
            {
                /* 获取ttwid的方法
                var res = await client.GetAsync(DYConfig.referer);
                if (res.StatusCode == HttpStatusCode.OK)
                {
                    if (res.Headers.TryGetValues("Set-Cookie", out var values))
                    {
                        foreach (var item in values)
                        {
                            if (item.Contains("ttwid"))
                            {
                                var cookie = item.Split(",", StringSplitOptions.RemoveEmptyEntries).FirstOrDefault();
                                client.DefaultRequestHeaders.Add("Cookie", $"passport_csrf_token=e44e855c80dbb07c36edee686554f091; {cookie} ttcid=ea1e971aedde4670981ded581d0f63cc26; tt_scid=KNIhqF3unIHanT.A37K9yPEV7qVt.54369ZcNyPEBDWMqedOG.zEUlGtIYbwbeL641ae; s_v_web_id=verify_litmi9z2_USt9mqjY_ecy9_4iCw_Ar3D_KpBrfDYwV8Ue;");
                                continue;
                            }
                        }
                    }
                }*/
                //暂时用固定的
                client.DefaultRequestHeaders.Add("Cookie", "passport_csrf_token=e44e855c80dbb07c36edee686554f091; ttwid=1%7CYDsk3s2agHeKNwL1zzAZBehGOPxAOXPum_H2EtV75wI%7C1686620837%7Cad4b66b9f398491ee076462b14b3a2f11281ae80dd0aa2b76f933a0d44ce3a8e; ttcid=ea1e971aedde4670981ded581d0f63cc26; tt_scid=KNIhqF3unIHanT.A37K9yPEV7qVt.54369ZcNyPEBDWMqedOG.zEUlGtIYbwbeL641ae; s_v_web_id=verify_litmi9z2_USt9mqjY_ecy9_4iCw_Ar3D_KpBrfDYwV8Ue;");
            }
            if (!client.DefaultRequestHeaders.Contains("Cookie"))
            {
                client.DefaultRequestHeaders.Add("Cookie", "passport_csrf_token=e44e855c80dbb07c36edee686554f091; ttwid=1%7CYDsk3s2agHeKNwL1zzAZBehGOPxAOXPum_H2EtV75wI%7C1686620837%7Cad4b66b9f398491ee076462b14b3a2f11281ae80dd0aa2b76f933a0d44ce3a8e; ttcid=ea1e971aedde4670981ded581d0f63cc26; tt_scid=KNIhqF3unIHanT.A37K9yPEV7qVt.54369ZcNyPEBDWMqedOG.zEUlGtIYbwbeL641ae; s_v_web_id=verify_litmi9z2_USt9mqjY_ecy9_4iCw_Ar3D_KpBrfDYwV8Ue;");

            }

            var requestParameter = string.Empty;
            if (PageIndex > 1)
            {
                requestParameter = string.Format(DYConfig.requestParameterTemplete, PageIndex,2,"");
            }
            else
            {
                requestParameter = string.Format(DYConfig.requestParameterTemplete, PageIndex, 0, $"&webid=7243981249080264204");
            }
            try
            {
                var bougs = await GetBougs(requestParameter);
                var res = string.Empty;
                if (!string.IsNullOrEmpty(bougs))
                {
                    res = await client.GetStringAsync($"https://www.douyin.com/aweme/v1/web/tab/feed/?{requestParameter}&X-Bogus={bougs}");
                    if (!string.IsNullOrEmpty(res))
                    {
                        var video= JsonSerializer.Deserialize<DYVideo>(res);
                        PageIndex++;
                        lock (obj)
                        {
                            for (int i = CurrentIndex - 1; i > -1; i--)
                            {
                                if (videos.Count>i)
                                {
                                    videos.RemoveAt(i);
                                }
                               
                            }
                            CurrentIndex = 0;
                            Add(video.aweme_list);
                        }
                    }
                }
                
              
            }
            catch (Exception ex)
            {
                ;
               
            }
                       
            
        }
        private async Task<string> GetWebId() 
        {
            var url = "https://mcs.zijieapi.com/webid";
            var client = _httpClientFactory.CreateClient(nameof(DYService));
            client.DefaultRequestHeaders.Add("Accept", "*/*");
            client.DefaultRequestHeaders.Add("Referer", DYConfig.referer);
            client.DefaultRequestHeaders.Add("User-Agent", DYConfig.userAgent);
            var res = await client.PostAsync(url, new StringContent($"{{\"app_id\":6383,\"url\":\"https://www.douyin.com/\",\"user_agent\":\"{DYConfig.userAgent}\",\"referer\":\"\",\"user_unique_id\":\"\"}}", Encoding.UTF8, "application/json"));
            if (res.IsSuccessStatusCode)
            {
                var json = await res.Content.ReadAsStringAsync();
                var obj = JsonSerializer.Deserialize<WebId>(json);
                return obj.web_id;
            }
            return string.Empty;

        }
        /// <summary>
        /// endtime推测是视频长度，但是用视频长度会报错，只能取个整数减少报错
        /// </summary>
        /// <param name="aweme_id"></param>
        /// <param name="webid"></param>
        /// <param name="endtime"></param>
        /// <returns></returns>
        public async Task<DyDM> GetDanmaKu(string aweme_id, string webid= "7243981249080264204", long endtime=10)
        {
            if (endtime > 10)
            {
                endtime = endtime / 10 * 10;
            }
            var danmakuRequesetParamater = string.Format(DYConfig.danmakuParameter, aweme_id, aweme_id, endtime, webid);
            var client = _httpClientFactory.CreateClient(nameof(DYService));
            client.DefaultRequestHeaders.Add("Accept", "application/json, text/plain, */*");
            client.DefaultRequestHeaders.Add("Accept-Language", "zh-CN,zh;q=0.9,en;q=0.8");
            client.DefaultRequestHeaders.Add("Referer", DYConfig.referer);
            client.DefaultRequestHeaders.Add("User-Agent", DYConfig.userAgent);
            var bougs = await GetBougs(danmakuRequesetParamater);
            var danmakuUrl = $"https://www.douyin.com/aweme/v1/web/danmaku/get_v2/?{danmakuRequesetParamater}&X-Bogus={bougs}";
            var danmaRes = await client.GetStringAsync(danmakuUrl);
            var dm= JsonSerializer.Deserialize<DyDM>(danmaRes);
            return dm;
        }
        private async Task<string> GetBougs(string parameter)
        {
            using (var engine = new Engine())
            {
                using var stream = await FileSystem.OpenAppPackageFileAsync("douyin.min.js");
                using var reader = new StreamReader(stream);
                var js = reader.ReadToEnd();
                engine.Execute(js);
                var bougs = engine.Invoke("sign", parameter);
                return bougs.ToString();
            }
        }
    }
}
