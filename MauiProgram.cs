using Microsoft.Extensions.Logging;
using MauiApp3.Data;
using Microsoft.Maui.LifecycleEvents;
using System.Text;
using Microsoft.Maui.Handlers;
using MauiApp3.Views;
#if ANDROID
using Android.Webkit;
using Android.Content;
using Microsoft.Maui.Controls.Compatibility.Platform.Android;
#endif
namespace MauiApp3;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
        try
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                })
                .ConfigureLifecycleEvents(events => {

#if ANDROID
 
#endif
#if IOS

#endif


                }).
                ConfigureMauiHandlers(x =>
                {
#if ANDROID
                    x.AddHandler(typeof(Android.Webkit.WebView),typeof(MltWebViewRenderer));
#endif
                })

                ;

            builder.Services.AddMauiBlazorWebView();


#if DEBUG
		builder.Services.AddBlazorWebViewDeveloperTools();
		builder.Logging.AddDebug();
#endif

            //builder.Services.AddSingleton<WeatherForecastService>();
            builder.Services
                .AddHttpClient("sodu").ConfigurePrimaryHttpMessageHandler(() =>
                {
                    return new HttpClientHandler
                    {
                        AutomaticDecompression = System.Net.DecompressionMethods.Deflate | System.Net.DecompressionMethods.GZip

                    };

                });
            builder.Services.AddHttpClient("my").ConfigurePrimaryHttpMessageHandler(() =>
            {
                return new HttpClientHandler
                {
                    AutomaticDecompression = System.Net.DecompressionMethods.Deflate | System.Net.DecompressionMethods.GZip | System.Net.DecompressionMethods.Brotli

                };

            });
           
            builder.Services.AddHttpClient("top").ConfigurePrimaryHttpMessageHandler(() =>
            {
                return new HttpClientHandler
                {
                    AutomaticDecompression = System.Net.DecompressionMethods.Deflate | System.Net.DecompressionMethods.GZip | System.Net.DecompressionMethods.Brotli

                };

            });
            builder.Services.AddHttpClient("bqg").ConfigurePrimaryHttpMessageHandler(() =>
            {
                return new HttpClientHandler
                {
                    AutomaticDecompression = System.Net.DecompressionMethods.Deflate | System.Net.DecompressionMethods.GZip | System.Net.DecompressionMethods.Brotli

                };

            });
            builder.Services.AddSingleton<AppShell>();
            builder.Services.AddSingleton<SoduService>();
            builder.Services.AddSingleton<BQGService>();
            builder.Services.AddSingleton<IFileSystem>(FileSystem.Current);
            builder.Services.AddSingleton<NewsService>();
            return builder.Build();
        }
        catch (Exception ex)
        {

           var path= FileSystem.Current.AppDataDirectory;
            var filePath = Path.Combine(path,"log.txt");
            using (var fs = File.Open(filePath, FileMode.OpenOrCreate, FileAccess.Write))
            {
                var exception = ex;
                while (exception != null)
                {
                    var bytes= Encoding.UTF8.GetBytes(exception.Message);
                    fs.Write(bytes, 0, bytes.Length);
                    exception = exception.InnerException;
                }
            }
            return null;
        }
		
	}
    
}
#if ANDROID
public class MltWebViewRenderer : WebViewHandler
{
    protected override Android.Webkit.WebView CreatePlatformView()
    {

        PlatformView.SetWebViewClient(new MltWebViewClient());
        global::Android.Webkit.WebView.SetWebContentsDebuggingEnabled(true);
        return base.CreatePlatformView();
    }
}

public class MltWebViewClient : global::Android.Webkit.WebViewClient
{
    public MltWebViewClient()
    {
    }

    public override bool ShouldOverrideUrlLoading(global::Android.Webkit.WebView view, IWebResourceRequest request)
    {
        view.Settings.SetSupportMultipleWindows(false);
        view.Settings.JavaScriptCanOpenWindowsAutomatically = true;

        view.Settings.JavaScriptEnabled = true;

        view.SetWebChromeClient(new WebChromeClient());

        return base.ShouldOverrideUrlLoading(view, request);
    }
}
#endif
