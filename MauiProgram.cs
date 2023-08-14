using Microsoft.Extensions.Logging;
using MauiApp3.Data;
using Microsoft.Maui.LifecycleEvents;
using System.Text;
using MauiApp3.Data.Impl;
using MauiApp3.Data.Interfaces;
using Microsoft.Extensions.DependencyInjection.Extensions;
using MauiApp3.Data.Extensions;
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
                        AutomaticDecompression = System.Net.DecompressionMethods.Deflate | System.Net.DecompressionMethods.GZip,
                   
                      
                        
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
            builder.Services.AddSingleton<XS166Parser>();
            builder.Services.AddSingleton<XPTParser>();
            builder.Services.AddSingleton<QXSParser>();
            builder.Services.AddSingleton<MK99Parser>();
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
                    else if (key == nameof(XS166Parser))
                    {
                        return provider.GetService<XS166Parser>();
                    }
                    else if (key == nameof(XPTParser))
                    {
                        return provider.GetService<XPTParser>();
                    }
                    else if (key == nameof(QXSParser))
                    {
                        return provider.GetService<QXSParser>();
                    }
                    else if (key == nameof(MK99Parser))
                    {
                        return provider.GetService<MK99Parser>();
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
                    else if (key == nameof(XS166Service))
                    {
                        return provider.GetService<XS166Service>();
                    }
                    else if (key == nameof(XPTService))
                    {
                        return provider.GetService<XPTService>();
                    }
                    else if (key == nameof(QXSService))
                    {
                        return provider.GetService<QXSService>();
                    }
                    else if (key == nameof(MK99Service))
                    {
                        return provider.GetService<MK99Service>();
                    }
                    else
                    {
                        throw new ArgumentException($"不支持的类型{key}");
                    }



                };

                return accesor;
            });
            builder.Services.AddSingleton<BookBenService>();
            builder.Services.AddSingleton<SHU20Service>();
            builder.Services.AddSingleton<SoduService>();
            builder.Services.AddSingleton<XS166Service>();
            builder.Services.AddSingleton<XPTService>();
            builder.Services.AddSingleton<QXSService>();
            builder.Services.AddSingleton<MK99Service>();

            builder.Services.AddSingleton<LinDianService>();
            builder.Services.AddSingleton<BQG1Service>();
            builder.Services.AddSingleton<KSKService>();
            builder.Services.AddSingleton<IFileSystem>(FileSystem.Current);
            builder.Services.AddSingleton<NewsService>();
            builder.Services.AddTransient<DYService>();
            builder.Services.AddSingleton<INovelService, NovelService>();
            builder.Services.TryAddSingleton<TetrominoGenerator>();
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

