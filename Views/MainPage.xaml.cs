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
    }

}
