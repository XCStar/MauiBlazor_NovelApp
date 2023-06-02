using MauiApp3.Data.Impl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MauiApp3.Common
{
    public class ServiceLoctor
    {
        private static readonly Dictionary<string, string> loctors = new Dictionary<string, string>()
        {
            {nameof(SoduService),nameof(SoduService) },
            { nameof(BQGService),nameof(BQGService)},
            {nameof(LinDianService),nameof(LinDianService)},
            {nameof(BQG1Service),nameof(BQG1Service)},
             {nameof(KSKService),nameof(KSKService)}

        };
        public static readonly Dictionary<string, string> novelKeyPairs = new Dictionary<string, string>
        {
               {nameof(SoduService),"搜读" },
               {nameof(KSKService),"看书库"},
               {nameof(BQG1Service),"笔趣阁(1)"},
               {nameof(LinDianService),"零点" },
               {nameof(BQGService),"笔趣阁(cloudflare暂时无法抓取)"}

        };
        public static string GetServiceName(string key)
        {

            if (loctors.ContainsKey(key))
            {
                return loctors[key];
            }
            return loctors["sodu"];
        }
    }
}
