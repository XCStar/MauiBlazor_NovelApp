using AngleSharp.Dom;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace MauiApp3.Common
{
    public class FileCacheHelper
    {
        public static void Save(string key, string value)
        {
            var basePath = FileSystem.Current.CacheDirectory;
            var distPath = Path.Combine(basePath, "cache_html");
            if (!Directory.Exists(distPath))
            {
                Directory.CreateDirectory(distPath);
            }
            var fileName = MauiApp3.Common.StringExtensions.GetMd5(key);
            var filePath = Path.Combine(distPath, fileName);
            if (File.Exists(filePath))
            {
               File.Delete(filePath);
            }
            using (var fs = File.Open(filePath, FileMode.OpenOrCreate, FileAccess.ReadWrite))
            {
                using (StreamWriter writer = new StreamWriter(fs))
                {
                    writer.Write(value);
                }
            }
        }
        public static string Get(string key)
        {
            var basePath = FileSystem.Current.CacheDirectory;
            var distPath = Path.Combine(basePath, "cache_html");
            if (!Directory.Exists(distPath))
            {
                return string.Empty;
            }
            var fileName = MauiApp3.Common.StringExtensions.GetMd5(key);
            var filePath = Path.Combine(distPath, fileName);
            if (!File.Exists(filePath))
            {
                return string.Empty;
            }
            using (var fs = File.Open(filePath, FileMode.Open, FileAccess.Read))
            {
                using (StreamReader reader = new StreamReader(fs))
                {
                    return reader.ReadToEnd();
                }
            }
        }
        public static bool ClearCache()
        {
            var basePath = FileSystem.Current.CacheDirectory;
            var distPath = Path.Combine(basePath, "cache_html");
            if (Directory.Exists(distPath))
            {
                Directory.Delete(distPath, true);
            }
            return true;
        }
        public static bool DelFile(string key) 
        {
            var basePath = FileSystem.Current.CacheDirectory;
            var distPath = Path.Combine(basePath, "cache_html");
            if (!Directory.Exists(distPath))
            {
                return true;
            }
            var fileName = MauiApp3.Common.StringExtensions.GetMd5(key);
            var filePath = Path.Combine(distPath, fileName);
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
                return true;
            }
            return true;
        }
    }
}
