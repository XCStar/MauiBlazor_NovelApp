using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MauiApp3.Model
{
    [Table("records")]
    public class Records
    {
        [PrimaryKey,AutoIncrement]
        public int id { get; set; }
        public string novelid { get; set; }
       public string novelname { get; set; }
        public string noveladdr { get; set; }
        
        public string chaptername { get; set; }
        public string chapteraddr { get; set; }
    }
}
