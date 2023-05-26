using Microsoft.Extensions.Logging;
using MauiApp3.Data;
using Microsoft.Maui.LifecycleEvents;
using System.Text;
using MauiApp3.Data.Impl;
using MauiApp3.Data.Interfaces;
#if ANDROID

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

                //  x.AddHandler(typeof(Microsoft.Maui.Controls.WebView),typeof(MauiApp3.Platforms.Android.MltWebViewHandler));

#endif
                });
#if ANDROID

#endif
            builder.Services.AddMauiBlazorWebView();

           
#if DEBUG
        builder.Services.AddBlazorWebViewDeveloperTools();
		builder.Logging.AddDebug();
#endif

            
            builder.Services
                .AddHttpClient(nameof(SoduService)).ConfigurePrimaryHttpMessageHandler(() =>
                {
                    return new HttpClientHandler
                    {
                        AutomaticDecompression = System.Net.DecompressionMethods.Deflate | System.Net.DecompressionMethods.GZip

                    };

                });
            builder.Services
               .AddHttpClient(nameof(LinDianService)).ConfigurePrimaryHttpMessageHandler(() =>
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
            builder.Services.AddHttpClient(nameof(BQGService)).ConfigurePrimaryHttpMessageHandler(() =>
            {
                return new HttpClientHandler
                {
                    AutomaticDecompression = System.Net.DecompressionMethods.Deflate | System.Net.DecompressionMethods.GZip | System.Net.DecompressionMethods.Brotli

                };

            });
            builder.Services.AddSingleton<AppShell>();

            builder.Services.AddSingleton<SoduParser>();
            builder.Services.AddSingleton<BQGParser>();
            builder.Services.AddSingleton<LinDianParser>();
            builder.Services.AddSingleton<BQG1Parser>();
            builder.Services.AddSingleton(provider => {
                Func<string, IPageParser> accesor = key => {
                    if (key == nameof(SoduParser))
                    {
                        return provider.GetService<SoduParser>();
                    }
                    else if (key == nameof(BQGParser))
                    {
                        return provider.GetService<BQGParser>();
                    }
                    else if (key == nameof(LinDianParser))
                    {
                        return provider.GetService<LinDianParser>();
                    }
                    else if (key == nameof(BQG1Parser))
                    {
                        return provider.GetService<BQG1Parser>();
                    }
                    throw new ArgumentException($"不支持的类型{key}");
                
                
                };

                return accesor;
            });
            builder.Services.AddSingleton(provider => {
                Func<string, INovelDataService> accesor = key => {
                    if (key == nameof(SoduService))
                    {
                        return provider.GetService<SoduService>();
                    }
                    else if (key == nameof(BQGService))
                    {
                        return provider.GetService<BQGService>();
                    }
                    else if (key == nameof(LinDianService))
                    {
                        return provider.GetService<LinDianService>();
                    }
                    else if (key == nameof(BQG1Service))
                    {
                        return provider.GetService<BQG1Service>();
                    }
                    throw new ArgumentException($"不支持的类型{key}");


                };

                return accesor;
            });
            builder.Services.AddSingleton<SoduService>();
            builder.Services.AddSingleton<BQGService>();
            builder.Services.AddSingleton<LinDianService>();
            builder.Services.AddSingleton<BQG1Service>();
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

