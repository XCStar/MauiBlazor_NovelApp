using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Webkit;
using MauiApp3.Platforms.Android;
using Microsoft.AspNetCore.Components.WebView.Maui;
using Microsoft.Maui.Handlers;

namespace MauiApp3;

[Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
public class MainActivity : MauiAppCompatActivity
{
    protected override void OnCreate(Bundle savedInstanceState)
    {
        Window.SetFlags(Android.Views.WindowManagerFlags.TranslucentNavigation,Android.Views.WindowManagerFlags.TranslucentNavigation);
        Window.SetStatusBarColor(Android.Graphics.Color.Transparent);
        Window.SetNavigationBarColor(Android.Graphics.Color.Transparent);
        Microsoft.Maui.Handlers.WebViewHandler.Mapper.ModifyMapping(nameof(WebViewClient), (handler, view,method) =>
        {
            
            WebViewHandler.MapWebViewClient(handler,view);
           
            handler.PlatformView.SetWebViewClient(new CustomAndroidMauiWebClient((WebViewHandler)handler));
        });
        base.OnCreate(savedInstanceState);
    }
    
}

