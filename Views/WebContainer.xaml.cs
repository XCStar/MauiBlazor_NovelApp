using Microsoft.Maui.Controls;

namespace MauiApp3.Views;

public partial class WebContainer : ContentPage
{
	public WebContainer(string _url)
	{
		InitializeComponent();
		this.view.Navigating += Navigating;
		try
		{
			this.view.Source = _url;

		}
		catch (Exception ex)
		{

			Application.Current.MainPage.DisplayAlert("ÌבÊ¾", ex.Message, "ok");
		}

	}
	public void Navigating(object sender, WebNavigatingEventArgs evnetArgs)
	{
		var currentUrl = this.view.Source;
		if (!evnetArgs.Url.StartsWith("http"))
		{
			evnetArgs.Cancel = true;
			return;
		}
	
	}
}