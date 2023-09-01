using MauiApp3.Data.Impl;
using MauiApp3.Data.Interfaces;
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
            {nameof(LinDianService),nameof(LinDianService)},
              {nameof(MK99Service),nameof(MK99Service)},
            {nameof(BQG1Service),nameof(BQG1Service)},
             {nameof(KSKService),nameof(KSKService)},
            {nameof(SHU20Service),nameof(SHU20Service)},
              {nameof(BookBenService),nameof(BookBenService)},
               {nameof(XS166Service),nameof(XS166Service)},
                  {nameof(XPTService),nameof(XPTService)},
            { nameof(QXSService),nameof(QXSService)},
             { nameof(FSService),nameof(FSService)}

        };
        public static readonly Dictionary<string, string> novelKeyPairs = new Dictionary<string, string>
        {
             {nameof(KSKService),"看书库"},
             {nameof(SHU20Service),"SHU20"},
             {nameof(BookBenService),"iBookBen"},
             {nameof(MK99Service),"MK99"},
             {nameof(FSService),"飞速(快)"},
             {nameof(XS166Service),"166XS"},
             {nameof(QXSService),"求小说" },
             {nameof(BQG1Service),"笔趣阁"},
             {nameof(SoduService),"搜读" },
             {nameof(XPTService),"飘天(无法搜索)" },
             {nameof(LinDianService),"零点" }

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
