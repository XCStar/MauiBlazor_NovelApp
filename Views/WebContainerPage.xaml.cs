
using System.Text.Encodings.Web;
using System.Web;

namespace MauiApp3.Views;

[QueryProperty(nameof(Url),"url")]
public partial class WebContainerPage : ContentPage
{
  private static readonly string userAgentScript = @"
 Object.defineProperties(window.navigator, {
    'userAgent': {
      enumerable: true,
      value: 'Mozilla/5.0 (Windows Phone 10)'
    },
    'appVersion': {
      enumerable: true,
      value: '5.0 (Windows Phone 10)'
    },
    'platform': {
      enumerable: true,
      value: 'Win32'
    }
  });
";
	private static readonly string zhiHuJavaSrcitpt = @"
const style = document.querySelector('style');
      style.innerHTML += `
        .MobileAppHeader-downloadLink {
          display: none !important;
        }
function remove(sel) {document.querySelectorAll(sel).forEach( a => a.remove());};
remove(""DIV.AdvertImg.AdvertImg--isLoaded.MBannerAd-image"");
remove(""DIV.Banner-adTag"");
remove(""div.MBannerAd-third"");
remove(""a.MHotFeedAd"");
remove(""div.Pc-feedAd-container.Pc-feedAd-container--mobile"");
remove("".WeiboAd-wrap"");
remove(""div.AdBelowMoreAnswers"");
remove(""div.OpenInAppButton"");
remove("".KfeCollection-VipRecommendCard"");
remove(""div.MHotFeedAd-smallCard"");
remove('div[style = ""margin - bottom: 10px;""]');
remove('div.OpenInAppButton');
remove('div.Question-sideColumn');
document.querySelector('.MBannerAd').parentNode.remove();
document.querySelectorAll(""a.HotQuestionsItem"").forEach(a=>a.onclick=function(){});
window.alert(""clear"");
";
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
  public string UserAgent{
    get
    {
      return webView.UserAgent;
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

	private async void webView_Navigating(object sender, WebNavigatingEventArgs e)
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
            //await webView.EvaluateJavaScriptAsync("window.alert(window.navigator.userAgent+'--'+window.navigator.appVersion+'--'+window.navigator.platform);");
            await webView.EvaluateJavaScriptAsync(userAgentScript.Replace("\r\n", ""));
            await webView.EvaluateJavaScriptAsync(zhiHuJavaSrcitpt.Replace("\r\n",""));
		}
    }
}