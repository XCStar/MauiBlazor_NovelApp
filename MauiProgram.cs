using Microsoft.Extensions.Logging;
using MauiApp3.Data;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.Maui.LifecycleEvents;
using System.Text;

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
            builder.Services.AddSingleton<SoduService>();
            builder.Services.AddSingleton<IFileSystem>(FileSystem.Current);
            builder.Services.AddSingleton<NewsService>();
            // builder.Services.AddSingleton<INavigationService, MauiNavigationService>()
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
