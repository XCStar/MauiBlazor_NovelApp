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
        public override void OnPageStarted(global::Android.Webkit.WebView view, string url, Bitmap favicon)
        {
            base.OnPageStarted(view, url, favicon);
            if (url.Contains("zhihu"))
            {
                view.LoadUrl($"javascript:{JavaScriptConfig.userAgentScript}");
            }
        }
    }
        //没什么卵用，android OnPageFinished不会响应 最新版已解决，等待更新https://github.com/dotnet/maui/pull/14321/files/bab24281ac3638d91d28a11a402e043f3a8f2378
    //public class MltWebViewHandler : WebViewHandler
    //{


    
    //    protected override global::Android.Webkit.WebView CreatePlatformView()
    //    {
            
    //        var platformView = base.CreatePlatformView();
    //        platformView.SetWebViewClient(new MyWebViewClient(this));

    //        return platformView;
    //    }
        


    //}
}
