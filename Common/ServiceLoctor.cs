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
            {nameof(LinDianService),nameof(LinDianService)}
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
