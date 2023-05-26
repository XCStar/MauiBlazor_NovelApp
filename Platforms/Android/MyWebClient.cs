using Microsoft.Maui.Handlers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.Webkit;
using Android.Content;
using Microsoft.Maui.Controls.Compatibility.Platform.Android;
using Microsoft.Maui.Platform;
using Android.Graphics;
using MauiApp3.Common;

namespace MauiApp3.Platforms.Android
{
    
    public class MyWebViewClient :MauiWebViewClient
    {
        public MyWebViewClient(WebViewHandler handler) :base(handler)
        {
                
        }
        public override bool ShouldOverrideUrlLoading(global::Android.Webkit.WebView view, string url)
        {
            if (url.StartsWith("https://m.weibo.cn/login"))
            {
                view.StopLoading();
                return true;
            }
            return base.ShouldOverrideUrlLoading(view, url);
        }
        public override void OnPageStarted(global::Android.Webkit.WebView view, string url, Bitmap favicon)
        {
            base.OnPageStarted(view, url, favicon);
            if (url.Contains("zhihu"))
            {
                view.LoadUrl($"javascript:{JavaScriptConfig.userAgentScript}");
            }
        }
    }
       
}
