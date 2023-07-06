using Microsoft.Extensions.Logging;
using MauiApp3.Data;
using Microsoft.Maui.LifecycleEvents;
using System.Text;
using MauiApp3.Data.Impl;
using MauiApp3.Data.Interfaces;
#if WINDOWS
using MauiApp3.Platforms.Windows.Handlers;
#endif
using Microsoft.Maui.Handlers;
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

#if WINDOWS

#endif

                });

            builder.Services.AddMauiBlazorWebView();

           
#if DEBUG
        builder.Services.AddBlazorWebViewDeveloperTools();
		builder.Logging.AddDebug();
#endif

            builder.Services
                .AddHttpClient(nameof(DYService)).ConfigurePrimaryHttpMessageHandler(() =>
                {
                    return new HttpClientHandler
                    {
                        AutomaticDecompression = System.Net.DecompressionMethods.Deflate | System.Net.DecompressionMethods.GZip,
                        UseCookies=false

                    };

                });
            builder.Services
              .AddHttpClient(nameof(BookBenService)).ConfigurePrimaryHttpMessageHandler(() =>
              {
                  return new HttpClientHandler
                  {
                      AutomaticDecompression = System.Net.DecompressionMethods.Deflate | System.Net.DecompressionMethods.GZip,
                      UseCookies = false

                  };

              });
            builder.Services
                .AddHttpClient(nameof(SHU20Service)).ConfigurePrimaryHttpMessageHandler(() =>
                {
                    return new HttpClientHandler
                    {
                        AutomaticDecompression = System.Net.DecompressionMethods.Deflate | System.Net.DecompressionMethods.GZip,
                         UseCookies = false
                    };

                });
          
            builder.Services
                .AddHttpClient(nameof(KSKService)).ConfigurePrimaryHttpMessageHandler(() =>
                {
                    return new HttpClientHandler
                    {
                        AutomaticDecompression = System.Net.DecompressionMethods.Deflate | System.Net.DecompressionMethods.GZip

                    };

                });
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
            
            builder.Services.AddSingleton<AppShell>();

            builder.Services.AddSingleton<SoduParser>();
            builder.Services.AddSingleton<LinDianParser>();
            builder.Services.AddSingleton<BQG1Parser>();
            builder.Services.AddSingleton<KSKParser>();
            builder.Services.AddSingleton<SHU20Parser>();
            builder.Services.AddSingleton<BookBenParser>();
            builder.Services.AddSingleton(provider => {
                Func<string, IPageParser> accesor = key => {
                    if (key == nameof(SoduParser))
                    {
                        return provider.GetService<SoduParser>();
                    }
                   
                    else if (key == nameof(LinDianParser))
                    {
                        return provider.GetService<LinDianParser>();
                    }
                    else if (key == nameof(BQG1Parser))
                    {
                        return provider.GetService<BQG1Parser>();
                    }
                    else if (key == nameof(KSKParser))
                    {
                        return provider.GetService<KSKParser>();
                    }
                    else if (key == nameof(SHU20Parser))
                    {
                        return provider.GetService<SHU20Parser>();
                    }
                    else if (key == nameof(BookBenParser))
                    {
                        return provider.GetService<BookBenParser>();
                    }
                    else
                    {
                        throw new ArgumentException($"不支持的类型{key}");
                    }
                
                
                
                };

                return accesor;
            });
            builder.Services.AddSingleton(provider => {
                Func<string, IDataService> accesor = key => {
                    if (key == nameof(SoduService))
                    {
                        return provider.GetService<SoduService>();
                    }
                    else if (key == nameof(LinDianService))
                    {
                        return provider.GetService<LinDianService>();
                    }
                    else if (key == nameof(BQG1Service))
                    {
                        return provider.GetService<BQG1Service>();
                    }
                    else if (key == nameof(KSKService))
                    {
                        return provider.GetService<KSKService>();
                    }
                    else if (key == nameof(SHU20Service))
                    {
                        return provider.GetService<SHU20Service>();
                    }
                    else if (key == nameof(BookBenService))
                    {
                        return provider.GetService<BookBenService>();
                    }
                    else {
                        throw new ArgumentException($"不支持的类型{key}");
                    }
               


                };

                return accesor;
            });
            builder.Services.AddSingleton<BookBenService>();
            builder.Services.AddSingleton<SHU20Service>();
            builder.Services.AddSingleton<SoduService>();
        
            builder.Services.AddSingleton<LinDianService>();
            builder.Services.AddSingleton<BQG1Service>();
            builder.Services.AddSingleton<KSKService>();
            builder.Services.AddSingleton<IFileSystem>(FileSystem.Current);
            builder.Services.AddSingleton<NewsService>();
            builder.Services.AddTransient<DYService>();
            builder.Services.AddSingleton<INovelService, NovelService>();
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

