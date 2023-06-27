using AngleSharp.Html.LinkRels;
using Microsoft.Maui.Handlers;
using Microsoft.Maui.Platform;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MauiApp3.Platforms.Windows.Handlers
{
    public class CustomWebViewHandler :WebViewHandler
    {
        protected override void ConnectHandler(WebView2 platformView)
        {
            platformView.CoreWebView2.WebResourceRequested += CoreWebView2_WebResourceRequested;
            base.ConnectHandler(platformView);
        }

        private void CoreWebView2_WebResourceRequested(Microsoft.Web.WebView2.Core.CoreWebView2 sender, Microsoft.Web.WebView2.Core.CoreWebView2WebResourceRequestedEventArgs args)
        {
            if (args.Request.Uri.Contains("zhihu.com"))
            {
                args.Request.Headers.SetHeader("User-Agent", "Mozilla/5.0 (Linux; Android 11; Pixel 5) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/90.0.4430.91 Mobile Safari/537.36 Edg/114.0.0.0");
            }
            
        }
    }
}
