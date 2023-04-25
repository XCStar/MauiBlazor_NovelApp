using AngleSharp.Dom;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MauiApp3.Model
{
    public class NovelInfo
    {
        public string Url { get; set; }
        public string Name { get; set; }
        public string Author { get; set; }
        public string NovelType { get; set; }
        public string LastChapter { get; set; }
        public string LastChapterUrl { get; set; }
        public string UpdateTime { get; set; }
        public string Description { get; set; }
        public List<Chapter> Chapters { get; set; }
        public int TotalPage { get; set; }
        public DateTime GetTime { get; set; } = DateTime.Now;
        public int CurrentPage { get; set; }
        public string UpdateState { get; set; }
        
    }
    public class Chapter
    {
        public string Name { get; set; }
        public string Url { get; set; }
    }
    public class NovelContent
    {
        public string ChapterName { get; set; }
        public string Content { get; set; }
        public string Next { get; set; }
    }
}
