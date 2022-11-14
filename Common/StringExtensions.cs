using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security;
using System.Security.Cryptography;

namespace MauiApp3.Common
{
    public class StringExtensions
    {
        public static string GetMd5(string s)
        {
            if (string.IsNullOrEmpty(s))
            {
                return string.Empty;
            }
            using (var md5 = MD5.Create())
            {
                var bytes=md5.ComputeHash(Encoding.UTF8.GetBytes(s));
                var builder=new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();  
            }
           
        }
    }
}
