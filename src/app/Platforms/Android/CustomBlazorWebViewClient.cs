using Android.Graphics;
using Android.Text;
using Android.Webkit;
using MauiApp3.Common;
using MauiApp3.Data;
using Microsoft.AspNetCore.Components.WebView.Maui;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Versioning;
using System.Text;
using System.Threading.Tasks;
using static Android.Telephony.CarrierConfigManager;
using static System.Net.Mime.MediaTypeNames;

namespace MauiApp3.Platforms.Android
{
    public class CustomBlazorWebViewClient : WebViewClient
    {
        private readonly WebViewClient _blazorWebViewClient;
        public CustomBlazorWebViewClient(WebViewClient blazorWebViewClient)
        {

            _blazorWebViewClient = blazorWebViewClient;

        }
        public override bool ShouldOverrideUrlLoading(global::Android.Webkit.WebView view, string url)
        {
            if (!url.Contains("0.0.0.0"))
            {
                return false;
            }
            if (url.StartsWith("https://m.weibo.cn/login"))
            {
                view.StopLoading();
                return true;
            }
            return _blazorWebViewClient.ShouldOverrideUrlLoading(view, url);



        }
        public override bool ShouldOverrideUrlLoading(global::Android.Webkit.WebView view, IWebResourceRequest request)
        {

            if (request.Url.Host != "0.0.0.0")
            {
                return false;
            }
            
            return _blazorWebViewClient.ShouldOverrideUrlLoading(view, request);

        }
        public override WebResourceResponse ShouldInterceptRequest(global::Android.Webkit.WebView view, IWebResourceRequest request)
        {
            if (JavaScriptConfig.userAgentHosts.Contains(request.Url.Host))
            {
                if (request.RequestHeaders.ContainsKey("User-Agent"))
                {
                    request.RequestHeaders["User-Agent"] = DYConfig.userAgent;
                }
                else
                {
                    request.RequestHeaders.Add("User-Agent", DYConfig.userAgent);
                }
                request.RequestHeaders["Accept"] = "text/html,application/xhtml + xml,application/xml; q = 0.9,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.7";

                request.RequestHeaders["Accept-Encoding"] = "gzip,deflate";
            }
            var response= _blazorWebViewClient.ShouldInterceptRequest(view, request);
            return response;
        }
        public override void OnPageStarted(global::Android.Webkit.WebView view, string url, Bitmap favicon)
        {
            base.OnPageStarted(view, url, favicon);
            if (url.Contains("zhihu"))
            {
                view.LoadUrl($"javascript:{JavaScriptConfig.userAgentScript}");
            }

        }
        public override void OnPageFinished(global::Android.Webkit.WebView view, string url)
        {

            _blazorWebViewClient.OnPageFinished(view, url);
            if (url.Contains("zhihu"))
            {
                view.LoadUrl("javascript:" + JavaScriptConfig.zhiHuJavaSrcitpt);
            }


        }
    }
}

