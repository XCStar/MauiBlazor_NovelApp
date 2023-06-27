using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MauiApp3.Data
{
    public class DYConfig
    {
        public static string userAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/114.0.0.0 Safari/537.36 Edg/114.0.1823.41";
        public static string referer = "https://www.douyin.com/";
        public static string requestParameterTemplete = "device_platform=webapp&aid=6383&channel=channel_pc_web&tag_id=&ug_source=&creative_id=&count=10&refresh_index={0}&video_type_select=0&aweme_pc_rec_raw_data=&live_insert_type=&pc_launch_live_filters=&globalwid=&version_code=170400&version_name=17.4.0&pull_type={1}&min_window=0&cookie_enabled=true&screen_width=1920&screen_height=1080&browser_language=zh-CN&browser_platform=Win32&browser_name=Edge&browser_version=114.0.1823.41&browser_online=true&engine_name=Blink&engine_version=114.0.0.0&os_name=Windows&os_version=10&cpu_core_num=16&device_memory=8&platform=PC&downlink=10&effective_type=4g&round_trip_time=50{2}&pc_client_type=1&msToken=";
        public static string danmakuParameter = "device_platform=webapp&aid=6383&channel=channel_pc_web&app_name=aweme&format=json&group_id={0}&item_id={1}&start_time=0&end_time={2}&pc_client_type=1&version_code=170400&version_name=17.4.0&cookie_enabled=true&screen_width=1920&screen_height=1080&browser_language=zh-CN&browser_platform=Win32&browser_name=Edge&browser_version=114.0.1823.41&browser_online=true&engine_name=Blink&engine_version=114.0.0.0&os_name=Windows&os_version=10&cpu_core_num=16&device_memory=8&platform=PC&downlink=10&effective_type=4g&round_trip_time=50&webid={3}&msToken=";
    }
}
