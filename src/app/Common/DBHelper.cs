using MauiApp3.Model;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MauiApp3.Common
{
    public class DBHelper
    {

        private static string conncetString = "{0}";
        static DBHelper()
        {
            InitDb();
        }
        private static void InitDb()
        {
            var dbPath = Path.Combine(FileSystem.Current.AppDataDirectory, "db");
            if (!Directory.Exists(dbPath))
            {
                Directory.CreateDirectory(dbPath);
            }
            conncetString = string.Format(conncetString, Path.Combine(dbPath, "data.db"));
            try
            {
                SQLitePCL.Batteries_V2.Init();
                using (var sql = new SQLiteConnection(conncetString))
                {

                    sql.CreateTable<Records>();
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }
        public static async Task<int> GetRecordByNovelId(string Id)
        {
            var sql = new SQLiteAsyncConnection(conncetString);

            {
                var id = await sql.ExecuteScalarAsync<int>("select id from records where novelid=?", Id);
                return id;
            }
        }
        public static async Task<bool> DelByIds(List<int> Ids)
        {
            var sql = new SQLiteAsyncConnection(conncetString);
            {
                var row = await sql.Table<Records>().DeleteAsync(x => Ids.Contains(x.id));

                return row > 0;
            }
        }
        public static async Task<bool> Insert(string novelName, string novelId, string novelAddr, string chapterName, string chapterAddr,string srcType)
        {
            var sql = new SQLiteAsyncConnection(conncetString);

            {
                var record = new Records { novelname = novelName, novelid = novelId, noveladdr = novelAddr, chaptername = chapterName, chapteraddr = chapterAddr,srcType=srcType };
                var row = await sql.InsertAsync(record);
                return row > 0;
            }
        }
        public static async Task<List<Records>> GetAllRecords()
        {
            var sql = new SQLiteAsyncConnection(conncetString);

            {
                var list = await sql.QueryAsync<Records>("select * from records order by id desc");
                return list;
            }


        }
        public static async Task<bool> DelAllRecord()
        {
            var sql = new SQLiteAsyncConnection(conncetString);

            {
                var row = await sql.ExecuteAsync("delete  from records");
                return row > 0;
            }
        }
    }
}
