using Android.Graphics;
using Android.Webkit;
using MauiApp3.Common;
using Microsoft.AspNetCore.Components.WebView.Maui;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Versioning;
using System.Text;
using System.Threading.Tasks;

namespace MauiApp3.Platforms.Android
{
    public class MyBlazorWebViewClient : WebViewClient
    {
        private readonly WebViewClient _blazorWebViewClient;
        public MyBlazorWebViewClient(WebViewClient blazorWebViewClient)
        {

            _blazorWebViewClient = blazorWebViewClient;

        }
        public override bool ShouldOverrideUrlLoading(global::Android.Webkit.WebView view, string url)
        {
            if (!url.Contains("0.0.0.0"))
            {
                return false;
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

            return _blazorWebViewClient.ShouldInterceptRequest(view, request);
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

