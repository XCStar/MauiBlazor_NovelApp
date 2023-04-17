using Microsoft.AspNetCore.Components.WebView;
#if WINDOWS
using Microsoft.Web.WebView2;
#endif
namespace MauiApp3.Views;
public partial class MainPage : ContentPage
{


    public MainPage()
	{
		 InitializeComponent();

    }
    public void BlazorUrlLoading(object sender, UrlLoadingEventArgs e)
	{
        
		
		e.UrlLoadingStrategy = UrlLoadingStrategy.OpenInWebView;

        if (e.Url.Scheme!="http"&&e.Url.Scheme!="https")
        {
            e.UrlLoadingStrategy = UrlLoadingStrategy.CancelLoad;
        }

	}

   

    private void blazorWebView_BlazorWebViewInitialized(object sender, BlazorWebViewInitializedEventArgs e)
    {


#if ANDROID

#elif IOS

#elif WINDOWS
        e.WebView.CoreWebView2.NewWindowRequested+=CoreWebView2_NewWindowRequested;
        void CoreWebView2_NewWindowRequested(object sender, Microsoft.Web.WebView2.Core.CoreWebView2NewWindowRequestedEventArgs @event)
        {
            @event.NewWindow = e.WebView.CoreWebView2;
        }

#endif


    }

}
