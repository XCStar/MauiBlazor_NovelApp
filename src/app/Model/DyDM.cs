using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MauiApp3.Model
{
   
    public class DyDM
    {
        public Danmaku_List[] danmaku_list { get; set; }
        public long end_time { get; set; }
        public Extra extra { get; set; }
        public Log_Pb log_pb { get; set; }
        public int start_time { get; set; }
        public int status_code { get; set; }
        public int total { get; set; }
        public string status_msg{get;set;}
}

    public class Extra
    {
        public object[] fatal_item_ids { get; set; }
        public string logid { get; set; }
        public long now { get; set; }
    }


    public class Danmaku_List
    {
        public string danmaku_id { get; set; }
        public int danmaku_type { get; set; }
        public int digg_count { get; set; }
        public int digg_type { get; set; }
        public Extra1 extra { get; set; }
        public bool from_copy { get; set; }
        public string item_id { get; set; }
        public int offset_time { get; set; }
        public float score { get; set; }
        public bool show_copy { get; set; }
        public bool show_digg { get; set; }
        public int status { get; set; }
        public string text { get; set; }
        public string user_id { get; set; }
    }

    public class Extra1
    {
    }

}
