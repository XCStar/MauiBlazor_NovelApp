using Android.App;
using Android.Content.PM;
using Android.OS;

namespace MauiApp3;

[Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
public class MainActivity : MauiAppCompatActivity
{
    protected override void OnCreate(Bundle savedInstanceState)
    {
        Window.SetFlags(Android.Views.WindowManagerFlags.TranslucentNavigation,Android.Views.WindowManagerFlags.TranslucentNavigation);
        Window.SetStatusBarColor(Android.Graphics.Color.Transparent);
        Window.SetNavigationBarColor(Android.Graphics.Color.Transparent);
        //Window.SetFlags(Android.Views.WindowManagerFlags.HardwareAccelerated, Android.Views.WindowManagerFlags.HardwareAccelerated);
        base.OnCreate(savedInstanceState);
    }
    
}

