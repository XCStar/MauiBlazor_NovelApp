using MauiApp3.Views;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace MauiApp3.Common
{
    public static class RazorHelper
    {
        public static int fontSize = 18;
        private static readonly StringBuilder urlStringBuilder=new StringBuilder();
        public static void GoTo(this NavigationManager navigation,string url, params string[] args)
        {
            urlStringBuilder.Clear();
            urlStringBuilder.Append(url);
            if (args.Length > 0)
            {
                urlStringBuilder.Append("/");
                urlStringBuilder.Append(string.Join("/",args.Select(x=>UrlEncoder.Default.Encode(x))));
            }
            navigation.NavigateTo(urlStringBuilder.ToString());

        }
        public static void GoToReplace(this NavigationManager navigation, string url, params string[] args)
        {
            urlStringBuilder.Clear();
            urlStringBuilder.Append(url);
            if (args.Length > 0)
            {
                urlStringBuilder.Append("/");
                urlStringBuilder.Append(string.Join("/", args.Select(x => UrlEncoder.Default.Encode(x))));
            }
            navigation.NavigateTo(urlStringBuilder.ToString(),false,true);

        }
        public static async Task GoToWebView(this Application application, string url)
        {


            //await application.MainPage.Navigation.PushAsync(new WebContainer(url));
            var current = application.MainPage;
            application.MainPage = new NavigationPage(current);
            await application.MainPage.Navigation.PushAsync(new WebContainer(url)
            {
                Title = "网页"
            });

        }
    }
}
