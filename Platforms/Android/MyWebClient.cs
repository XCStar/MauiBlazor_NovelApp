using Microsoft.Maui.Handlers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MauiApp3.Platforms.Android
{
    
    public class MyWebClient : Microsoft.Maui.Platform.MauiWebViewClient
    {
        public MyWebClient(WebViewHandler handler) : base(handler)
        {
           
        }
        public override void OnPageFinished(global::Android.Webkit.WebView view, string url)
        {
            base.OnPageFinished(view, url);
            if (url.Contains("zhihu"))
            {
                view.LoadUrl("javascript:window.alert('===============onPageFinished==============================')");
            }
        }
    }
}
