using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MauiApp3.Model
{
   
    public class DYVideo
    {
        public Aweme_List[] aweme_list { get; set; }
        public int has_more { get; set; }
        public Log_Pb log_pb { get; set; }
        public int status_code { get; set; }
    }

    public class Log_Pb
    {
        public string impr_id { get; set; }
    }

    public class Aweme_List
    {
        public object ad_candidates { get; set; }
        public Author author { get; set; }
        public long author_user_id { get; set; }
        public Aweme_Control aweme_control { get; set; }
        public string aweme_id { get; set; }
        public int aweme_type { get; set; }
        public object chapter_list { get; set; }
        public int collect_stat { get; set; }
        public object commerce_top_labels { get; set; }
        public string common_bar_info { get; set; }
        public int create_time { get; set; }
        public Danmaku_Control danmaku_control { get; set; }
        public string desc { get; set; }
        public Digg_Lottie digg_lottie { get; set; }
        public object dislike_dimension_list { get; set; }
        public object dislike_dimension_list_v2 { get; set; }
        public string group_id { get; set; }
        public int horizontal_type { get; set; }
        public object images { get; set; }
        public object img_bitrate { get; set; }
        public Impression_Data impression_data { get; set; }
        public bool is_ads { get; set; }
        public bool is_horizontal { get; set; }
        public bool is_image_beat { get; set; }
        public bool is_life_item { get; set; }
        public int is_story { get; set; }
        public int is_top { get; set; }
        public object jump_tab_info_list { get; set; }
        public Mix_Info mix_info { get; set; }
        public object music_guidance_tag_list { get; set; }
        public object original_images { get; set; }
        public Photo_Search_Entrance photo_search_entrance { get; set; }
        public bool prevent_download { get; set; }
        public object raw_ad_data { get; set; }
        public object ref_tts_id_list { get; set; }
        public object ref_voice_modify_id_list { get; set; }
        public string region { get; set; }
        public Share_Info2 share_info { get; set; }
        public string share_url { get; set; }
        public Statistics statistics { get; set; }
        public Status1 status { get; set; }
        public Text_Extra[] text_extra { get; set; }
        public object tts_id_list { get; set; }
        public int user_digged { get; set; }
        public int user_recommend_status { get; set; }
        public Video video { get; set; }
        public object video_tag { get; set; }
        public object voice_modify_id_list { get; set; }
        public string web_raw_data { get; set; }
        public object yumme_recreason { get; set; }
        public Cell_Room cell_room { get; set; }
        public int preview_video_status { get; set; }
        public Descendants descendants { get; set; }
        public Music music { get; set; }
        public Suggest_Words suggest_words { get; set; }
    }

    public class Author
    {
        public Avatar_Thumb avatar_thumb { get; set; }
        public object common_interest { get; set; }
        public Cover_Url[] cover_url { get; set; }
        public string custom_verify { get; set; }
        public string enterprise_verify_reason { get; set; }
        public int follow_status { get; set; }
        public int follower_status { get; set; }
        public bool is_ad_fake { get; set; }
        public string nickname { get; set; }
        public object not_seen_item_id_list { get; set; }
        public bool prevent_download { get; set; }
        public long room_id { get; set; }
        public string sec_uid { get; set; }
        public Share_Info share_info { get; set; }
        public string uid { get; set; }
    }

    public class Avatar_Thumb
    {
        public int height { get; set; }
        public string uri { get; set; }
        public string[] url_list { get; set; }
        public int width { get; set; }
    }

    public class Share_Info
    {
        public string share_desc { get; set; }
        public Share_Qrcode_Url share_qrcode_url { get; set; }
        public string share_title { get; set; }
        public string share_title_myself { get; set; }
        public string share_title_other { get; set; }
        public string share_url { get; set; }
        public string share_weibo_desc { get; set; }
    }

    public class Share_Qrcode_Url
    {
        public int height { get; set; }
        public string uri { get; set; }
        public string[] url_list { get; set; }
        public int width { get; set; }
    }

    public class Cover_Url
    {
        public int height { get; set; }
        public string uri { get; set; }
        public string[] url_list { get; set; }
        public int width { get; set; }
    }

    public class Aweme_Control
    {
        public bool can_comment { get; set; }
        public bool can_forward { get; set; }
        public bool can_share { get; set; }
        public bool can_show_comment { get; set; }
    }

    public class Danmaku_Control
    {
        public int danmaku_cnt { get; set; }
        public bool enable_danmaku { get; set; }
        public bool is_post_denied { get; set; }
        public string post_denied_reason { get; set; }
        public int post_privilege_level { get; set; }
        public bool skip_danmaku { get; set; }
    }

    public class Digg_Lottie
    {
        public int can_bomb { get; set; }
        public string lottie_id { get; set; }
    }

    public class Impression_Data
    {
        public long?[] group_id_list_a { get; set; }
        public long?[] group_id_list_b { get; set; }
        public long?[] group_id_list_c { get; set; }
        public object similar_id_list_a { get; set; }
        public long[] similar_id_list_b { get; set; }
    }

    public class Mix_Info
    {
        public Cover_Url1 cover_url { get; set; }
        public string desc { get; set; }
        public string extra { get; set; }
        public string mix_id { get; set; }
        public string mix_name { get; set; }
        public Share_Info1 share_info { get; set; }
        public Statis statis { get; set; }
        public Status status { get; set; }
    }

    public class Cover_Url1
    {
        public int height { get; set; }
        public string uri { get; set; }
        public string[] url_list { get; set; }
        public int width { get; set; }
    }

    public class Share_Info1
    {
        public string share_desc { get; set; }
        public string share_title { get; set; }
        public string share_title_myself { get; set; }
        public string share_title_other { get; set; }
        public string share_url { get; set; }
        public string share_weibo_desc { get; set; }
    }

    public class Statis
    {
        public int collect_vv { get; set; }
        public int current_episode { get; set; }
        public int play_vv { get; set; }
        public int updated_to_episode { get; set; }
    }

    public class Status
    {
        public int is_collected { get; set; }
        public int status { get; set; }
    }

    public class Photo_Search_Entrance
    {
        public int ecom_type { get; set; }
    }

    public class Share_Info2
    {
        public string share_link_desc { get; set; }
        public string share_url { get; set; }
    }

    public class Statistics
    {
        public int collect_count { get; set; }
        public int comment_count { get; set; }
        public int digg_count { get; set; }
        public int download_count { get; set; }
        public int forward_count { get; set; }
        public int play_count { get; set; }
        public int share_count { get; set; }
    }

    public class Status1
    {
        public bool allow_share { get; set; }
        public bool in_reviewing { get; set; }
        public bool is_delete { get; set; }
        public bool is_prohibited { get; set; }
        public int part_see { get; set; }
        public int private_status { get; set; }
    }

    public class Video
    {
        public Big_Thumbs[] big_thumbs { get; set; }
        public Bit_Rate[] bit_rate { get; set; }
        public object bit_rate_audio { get; set; }
        public Cover cover { get; set; }
        public int duration { get; set; }
        public Dynamic_Cover dynamic_cover { get; set; }
        public int height { get; set; }
        public int is_long_video { get; set; }
        public string meta { get; set; }
        public Origin_Cover origin_cover { get; set; }
        public Play_Addr play_addr { get; set; }
        public Play_Addr_265 play_addr_265 { get; set; }
        public Play_Addr_H264 play_addr_h264 { get; set; }
        public string ratio { get; set; }
        public string video_model { get; set; }
        public int width { get; set; }
    }

    public class Cover
    {
        public int height { get; set; }
        public string uri { get; set; }
        public string[] url_list { get; set; }
        public int width { get; set; }
    }

    public class Dynamic_Cover
    {
        public int height { get; set; }
        public string uri { get; set; }
        public object[] url_list { get; set; }
        public int width { get; set; }
    }

    public class Origin_Cover
    {
        public int height { get; set; }
        public string uri { get; set; }
        public string[] url_list { get; set; }
        public int width { get; set; }
    }

    public class Play_Addr
    {
        public int data_size { get; set; }
        public string file_cs { get; set; }
        public string file_hash { get; set; }
        public int height { get; set; }
        public string uri { get; set; }
        public string url_key { get; set; }
        public string[] url_list { get; set; }
        public int width { get; set; }
    }

    public class Play_Addr_265
    {
        public int data_size { get; set; }
        public string file_cs { get; set; }
        public string file_hash { get; set; }
        public int height { get; set; }
        public string uri { get; set; }
        public string url_key { get; set; }
        public string[] url_list { get; set; }
        public int width { get; set; }
    }

    public class Play_Addr_H264
    {
        public int data_size { get; set; }
        public string file_cs { get; set; }
        public string file_hash { get; set; }
        public int height { get; set; }
        public string uri { get; set; }
        public string url_key { get; set; }
        public string[] url_list { get; set; }
        public int width { get; set; }
    }

    public class Big_Thumbs
    {
        public float duration { get; set; }
        public string fext { get; set; }
        public int img_num { get; set; }
        public string img_url { get; set; }
        public int img_x_len { get; set; }
        public int img_x_size { get; set; }
        public int img_y_len { get; set; }
        public int img_y_size { get; set; }
        public float interval { get; set; }
        public string uri { get; set; }
    }

    public class Bit_Rate
    {
        public int FPS { get; set; }
        public string HDR_bit { get; set; }
        public string HDR_type { get; set; }
        public int bit_rate { get; set; }
        public string gear_name { get; set; }
        public int is_h265 { get; set; }
        public Play_Addr1 play_addr { get; set; }
        public int quality_type { get; set; }
        public string video_extra { get; set; }
    }

    public class Play_Addr1
    {
        public int data_size { get; set; }
        public string file_cs { get; set; }
        public string file_hash { get; set; }
        public int height { get; set; }
        public string uri { get; set; }
        public string url_key { get; set; }
        public string[] url_list { get; set; }
        public int width { get; set; }
    }

    public class Cell_Room
    {
        public string rawdata { get; set; }
    }

    public class Descendants
    {
        public string notify_msg { get; set; }
        public string[] platforms { get; set; }
    }

    public class Music
    {
        public string album { get; set; }
        public object artist_user_infos { get; set; }
        public int audition_duration { get; set; }
        public string author { get; set; }
        public bool author_deleted { get; set; }
        public object author_position { get; set; }
        public Avatar_Large avatar_large { get; set; }
        public Avatar_Medium avatar_medium { get; set; }
        public Avatar_Thumb1 avatar_thumb { get; set; }
        public int binded_challenge_id { get; set; }
        public int collect_stat { get; set; }
        public Cover_Hd cover_hd { get; set; }
        public Cover_Large cover_large { get; set; }
        public Cover_Medium cover_medium { get; set; }
        public Cover_Thumb cover_thumb { get; set; }
        public int duration { get; set; }
        public int end_time { get; set; }
        public object[] external_song_info { get; set; }
        public string extra { get; set; }
        public long id { get; set; }
        public string id_str { get; set; }
        public bool is_author_artist { get; set; }
        public bool is_del_video { get; set; }
        public bool is_original { get; set; }
        public bool is_pgc { get; set; }
        public bool is_restricted { get; set; }
        public bool is_video_self_see { get; set; }
        public string mid { get; set; }
        public bool mute_share { get; set; }
        public string offline_desc { get; set; }
        public string owner_handle { get; set; }
        public string owner_id { get; set; }
        public string owner_nickname { get; set; }
        public Play_Url play_url { get; set; }
        public object position { get; set; }
        public bool prevent_download { get; set; }
        public int prevent_item_download_status { get; set; }
        public float preview_end_time { get; set; }
        public float preview_start_time { get; set; }
        public bool redirect { get; set; }
        public string schema_url { get; set; }
        public string sec_uid { get; set; }
        public float shoot_duration { get; set; }
        public int source_platform { get; set; }
        public float start_time { get; set; }
        public int status { get; set; }
        public string title { get; set; }
        public object unshelve_countries { get; set; }
        public int user_count { get; set; }
        public int video_duration { get; set; }
        public Strong_Beat_Url strong_beat_url { get; set; }
    }

    public class Avatar_Large
    {
        public int height { get; set; }
        public string uri { get; set; }
        public string[] url_list { get; set; }
        public int width { get; set; }
    }

    public class Avatar_Medium
    {
        public int height { get; set; }
        public string uri { get; set; }
        public string[] url_list { get; set; }
        public int width { get; set; }
    }

    public class Avatar_Thumb1
    {
        public int height { get; set; }
        public string uri { get; set; }
        public string[] url_list { get; set; }
        public int width { get; set; }
    }

    public class Cover_Hd
    {
        public int height { get; set; }
        public string uri { get; set; }
        public object[] url_list { get; set; }
        public int width { get; set; }
    }

    public class Cover_Large
    {
        public int height { get; set; }
        public string uri { get; set; }
        public object[] url_list { get; set; }
        public int width { get; set; }
    }

    public class Cover_Medium
    {
        public int height { get; set; }
        public string uri { get; set; }
        public object[] url_list { get; set; }
        public int width { get; set; }
    }

    public class Cover_Thumb
    {
        public int height { get; set; }
        public string uri { get; set; }
        public string[] url_list { get; set; }
        public int width { get; set; }
    }

    public class Play_Url
    {
        public int height { get; set; }
        public string uri { get; set; }
        public string url_key { get; set; }
        public string[] url_list { get; set; }
        public int width { get; set; }
    }

    public class Strong_Beat_Url
    {
        public int height { get; set; }
        public string uri { get; set; }
        public string[] url_list { get; set; }
        public int width { get; set; }
    }

    public class Suggest_Words
    {
        public Suggest_Words1[] suggest_words { get; set; }
    }

    public class Suggest_Words1
    {
        public string hint_text { get; set; }
        public string icon_url { get; set; }
        public string scene { get; set; }
        public Word[] words { get; set; }
    }

    public class Word
    {
        public string info { get; set; }
        public string word { get; set; }
        public string word_id { get; set; }
    }

    public class Text_Extra
    {
        public int end { get; set; }
        public string hashtag_id { get; set; }
        public string hashtag_name { get; set; }
        public bool is_commerce { get; set; }
        public int start { get; set; }
        public int type { get; set; }
    }
    public class WebId
    {
        public int e { get; set; }
        public string web_id { get; set; }
    }
}
