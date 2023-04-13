using Microsoft.AspNetCore.Components.WebView;
using System.Threading;
namespace MauiApp3;

public partial class MainPage : ContentPage
{
    
    public MainPage()
	{
		 InitializeComponent();

    }
    public void BlazorUrlLoading(object sender, UrlLoadingEventArgs e)
	{
		
		e.UrlLoadingStrategy = UrlLoadingStrategy.OpenInWebView;
	}
}
