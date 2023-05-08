
using MauiApp3.Common;
using System.Text.Encodings.Web;
using System.Web;

namespace MauiApp3.Views;

[QueryProperty(nameof(Url),"url")]
public partial class WebContainerPage : ContentPage
{
	


	public WebContainerPage()
	{
		InitializeComponent();
		BindingContext = this;
		
	}
	private string url= "https://cn.bing.com/";
	public string Url
	{
		get=>url;
		set { 
		    url= HttpUtility.UrlDecode(value);
			OnPropertyChanged();
		}
	}
	private string currentUrl;
	public string CurrentUrl 
	{
        get
        {
            if (currentUrl is null)
            {
                return url;
            }
            return currentUrl;
        }
        set
        {
            currentUrl = value;
            OnPropertyChanged();
        }
    }

	private void webView_Navigating(object sender, WebNavigatingEventArgs e)
    {
        
		if (e.Url == "about:blank")
		{
			this.CurrentUrl = "www.bing.com";
		}
		if (!e.Url.StartsWith("http"))
		{
			e.Cancel = true;
			return;
		}
        this.CurrentUrl = e.Url;
    }
	private void Go()
	{
		if (string.IsNullOrEmpty(this.CurrentUrl))
		{
			this.CurrentUrl = "https://cn.bing.com/";
		}
		webView.Source = this.CurrentUrl;
		webView.Reload();
	}

    private void Button_Clicked(object sender, EventArgs e)
    {
		Go();
    }

    private  async void webView_Navigated(object sender, WebNavigatedEventArgs e)
    {
		
		if (e.Url.Contains("zhihu.com"))
        {
            
            await webView.EvaluateJavaScriptAsync(JavaScriptConfig.zhiHuJavaSrcitpt);
		}
    }
}