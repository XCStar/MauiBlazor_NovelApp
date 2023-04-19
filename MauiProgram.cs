using Microsoft.Extensions.Logging;
using MauiApp3.Data;
using Microsoft.Maui.LifecycleEvents;
using System.Text;
using Microsoft.Maui.Handlers;
using MauiApp3.Data.Impl;
using MauiApp3.Data.Interfaces;
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

                   // x.AddHandler(typeof(Microsoft.Maui.Controls.WebView),typeof(MltWebViewHandler));
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

            builder.Services.AddSingleton<SoduParser>();
            builder.Services.AddSingleton<BQGParser>();
            builder.Services.AddSingleton(provider => {
                Func<Type, IPageParser> accesor = key => {
                    if (key == typeof(SoduParser))
                    {
                        return provider.GetService<SoduParser>();
                    }
                    else if (key == typeof(BQGParser))
                    {
                        return provider.GetService<BQGParser>();
                    }
                    throw new ArgumentException($"不支持的类型{key}");
                
                
                };

                return accesor;
            });
            builder.Services.AddSingleton(provider => {
                Func<Type, INovelDataService> accesor = key => {
                    if (key == typeof(SoduService))
                    {
                        return provider.GetService<SoduService>();
                    }
                    else if (key == typeof(BQGService))
                    {
                        return provider.GetService<BQGService>();
                    }
                    throw new ArgumentException($"不支持的类型{key}");


                };

                return accesor;
            });
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
//没什么卵用，android OnPageFinished不会响应 最新版已解决，等待更新https://github.com/dotnet/maui/pull/14321/files/bab24281ac3638d91d28a11a402e043f3a8f2378
public class MltWebViewHandler : WebViewHandler
{
    
    protected override Android.Webkit.WebView CreatePlatformView()
    {
        var platformView= base.CreatePlatformView();
        platformView.SetWebViewClient(new CustomWebViewClient());
        global::Android.Webkit.WebView.SetWebContentsDebuggingEnabled(true);
        return platformView;
    }
   
}

public class CustomWebViewClient : global::Android.Webkit.WebViewClient
{
    public CustomWebViewClient()
    {
    }

    
    public override void OnPageFinished(global::Android.Webkit.WebView view,string url)
    {
          Console.WriteLine($"<------------------------------{url}---------------------------->");
          base.OnPageFinished(view,url);
         
          if(url.Contains("zhihu"))
          {
             view.LoadUrl("javascript:window.alert('===============onPageFinished==============================')");
          }
         
    }
}
#endif
