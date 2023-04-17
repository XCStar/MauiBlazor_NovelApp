using MauiApp3.Views;

namespace MauiApp3;

public partial class AppShell : Shell
{
	public AppShell()
	{
		Routing.RegisterRoute(nameof(MainPage),typeof(MainPage));
		Routing.RegisterRoute(nameof(WebContainerPage),typeof(WebContainerPage));
		InitializeComponent();
	}
}