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
        public static string HtmlFormat(string s)
        {
            var sb = new StringBuilder(s.Length*2);
            sb.Append("<p>");
            var index = 0;
            while (index < s.Length)
            {
                if (s[index] == '\n' || s[index] == '\r')
                {
                    index++;
                    continue;
                }
                if (s[index] == '【')
                {
                    sb.Append("<p>");
                    sb.Append(s[index]);
                    index++;
                }
                else if (s[index] == '】')
                {
                    sb.Append(s[index]);
                    sb.Append("</p>");
                    index++;
                }
                else if (s[index] == '。')
                {
                    if (index < s.Length - 1)
                    {
                        if (s[index + 1] == '”')
                        {
                            sb.Append("。");
                            sb.Append(s[index + 1]);
                            sb.Append("</p>");
                            sb.Append("<p>");
                            index += 2;
                            continue;
                        }
                        else if (s[index + 1] == '】')
                        {
                            index++;
                            continue;
                        }

                    }
                    sb.Append('。');
                    sb.Append("</p>");
                    sb.Append("<p>");
                    index++;
                }
                else if (s[index] == '！' || s[index] == '？')
                {
                    if (index < s.Length - 1)
                    {
                        if (s[index + 1] == '”')
                        {
                            sb.Append("。");
                            sb.Append(s[index + 1]);
                            sb.Append("</p>");
                            sb.Append("<p>");
                            index += 2;
                            continue;
                        }
                    }
                    sb.Append(s[index++]);
                }
                else
                {
                    sb.Append(s[index]);
                    index++;
                }
            }
            
            sb.Append("</p>");
            return sb.ToString();
        }
    }
}
