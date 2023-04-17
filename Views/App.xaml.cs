using MauiApp3.Data;
using MauiApp3.Views;
using System.Threading;

namespace MauiApp3;

public partial class App : Application
{
    private Timer _timer;
	public App(AppShell mainPage,SoduService soduService)
	{
        InitializeComponent();
        //var navigationPage =new NavigationPage(mainPage);
        //MainPage = navigationPage;
        MainPage = mainPage;
        _timer = new Timer(s => {
            if (s is not null && s is SoduService service)
            {
                service.ClearCache();
            }
        },soduService,TimeSpan.FromMinutes(20),TimeSpan.FromMinutes(20));
    }

}
