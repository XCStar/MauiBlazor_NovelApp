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
        public static int fontSize = 21;
        private static readonly StringBuilder urlStringBuilder=new StringBuilder();
        public static string GetIndexUrl(string type)
        {
            return $"index/{type}/1";
        }
        public static string BuilderUrl(string url, params string[] args)
        {
            urlStringBuilder.Clear();
            urlStringBuilder.Append(url);
            if (args.Length > 0)
            {
                urlStringBuilder.Append("/");
                urlStringBuilder.Append(string.Join("/", args.Select(x => UrlEncoder.Default.Encode(x))));
            }
            return urlStringBuilder.ToString();
        }
        public static void GoTo(this NavigationManager navigation,string url, params string[] args)
        {
            
            navigation.NavigateTo(BuilderUrl(url,args));

        }
        public static async Task OpenInWebView(this NavigationManager manager, string url)
        {
            await Shell.Current.GoToAsync($"{nameof(MauiApp3.Views.WebContainerPage)}?url={UrlEncoder.Default.Encode(url)}");
          
          
        }
    }
}
