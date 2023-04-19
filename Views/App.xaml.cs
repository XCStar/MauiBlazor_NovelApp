using MauiApp3.Common;
namespace MauiApp3;
public partial class App : Application
{
    private Timer _timer;
	public App(AppShell mainPage)
	{
        InitializeComponent();
        MainPage = mainPage;
        _timer = new Timer(s => {
            FileCacheHelper.ClearCache();
            
        },null,TimeSpan.FromMinutes(20),TimeSpan.FromMinutes(20));
    }

}
