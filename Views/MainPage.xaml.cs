using AngleSharp.Io;
using Microsoft.AspNetCore.Components.WebView;
namespace MauiApp3.Views;
public partial class MainPage : ContentPage
{


    public MainPage()
	{
		 InitializeComponent();

    }


        private  void BlazorUrlLoading(object sender, UrlLoadingEventArgs e)
		{


			e.UrlLoadingStrategy = UrlLoadingStrategy.OpenInWebView;

			if (e.Url.Scheme != "http" && e.Url.Scheme != "https")
			{
				e.UrlLoadingStrategy = UrlLoadingStrategy.CancelLoad;
			}

		}

		private  void blazorWebView_BlazorWebViewInitialized(object sender, BlazorWebViewInitializedEventArgs e)
		{

#if ANDROID
		    //替换之后还是无法截获一些连接，估计是blazor自身导致的
			
			e.WebView.Settings.JavaScriptEnabled = true;
			e.WebView.Settings.AllowFileAccess = true;
			e.WebView.Settings.MediaPlaybackRequiresUserGesture = false;
			e.WebView.Settings.SetGeolocationEnabled(true);
			e.WebView.Settings.SetGeolocationDatabasePath(e.WebView.Context?.FilesDir?.Path);
			e.WebView.SetWebViewClient(new MauiApp3.Platforms.Android.MyBlazorWebViewClient(e.WebView.WebViewClient));
			
			
#endif
#if WINDOWS
        e.WebView.CoreWebView2.WebResourceRequested += CoreWebView2_WebResourceRequested;
#endif
	}
#if WINDOWS
	private void CoreWebView2_WebResourceRequested(Microsoft.Web.WebView2.Core.CoreWebView2 sender, Microsoft.Web.WebView2.Core.CoreWebView2WebResourceRequestedEventArgs args)
    {
		if (args.Request.Uri.ToString().Contains("v95-p.douyinvod.com"))
		{
            var request = args.Request;
            request.Headers.SetHeader("User-Agent", "Mozilla/5.0 (Linux; Android 11; Pixel 5) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/90.0.4430.91 Mobile Safari/537.36 Edg/114.0.0.0");
            request.Headers.SetHeader("Accept", "text/html,application/xhtml + xml,application/xml; q = 0.9,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.7");
            request.Headers.SetHeader("Accept-Encoding", "gzip, deflate, br");
            request.Headers.SetHeader("Accept-Language", "zh-CN,zh;q=0.9,en;q=0.8,en-GB;q=0.7,en-US;q=0.6");
            request.Headers.SetHeader("Range", "bytes=0-1048575");
        }
		
    }
#endif


}
