using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MauiApp3.Model
{
    public struct DYInfo
    {
        public DYInfo(string id, string title, string url, long duration, string createtime,string sec_uid,string uid,string nickname)
        {
            this.id = id;
            this.title = title;
            this.url = url; 
            this.duration = duration;
            this.createtime = createtime;
        }
        public DYInfo():this("","","",0,"","","","")
        {
                
        }
        public string id { get; set; }
        public string title { get; set; }
        public string url { get; set; }
        public long duration { get; set; }
        public string createtime { get; set; }
    }
}
