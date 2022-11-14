using Microsoft.Extensions.Logging;
using MauiApp3.Data;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
namespace MauiApp3;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
			});

		builder.Services.AddMauiBlazorWebView();
		

#if DEBUG
		builder.Services.AddBlazorWebViewDeveloperTools();
		builder.Logging.AddDebug();
#endif

		//builder.Services.AddSingleton<WeatherForecastService>();
		builder.Services
			.AddHttpClient("sodu").ConfigurePrimaryHttpMessageHandler(() => {
				return new HttpClientHandler
				{
					AutomaticDecompression = System.Net.DecompressionMethods.Deflate | System.Net.DecompressionMethods.GZip

				};
			
			});
		builder.Services.AddSingleton<SoduService>();
		builder.Services.AddSingleton<IFileSystem>(FileSystem.Current);
		return builder.Build();
	}
    
}
