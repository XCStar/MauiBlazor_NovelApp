using MauiApp3.Data;
using System.Threading;

namespace MauiApp3;

public partial class App : Application
{
    private Timer _timer;
	public App(MainPage mainPage,SoduService soduService)
	{
        InitializeComponent();
        MainPage = mainPage;
        _timer = new Timer(s => {
            if (s is not null && s is SoduService service)
            {
                service.ClearCache();
            }
        },soduService,TimeSpan.FromMinutes(20),TimeSpan.FromMinutes(20));
    }

}
