using System;
using SinKingMusicSnalysis.Common;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Threading;

namespace SinKingMusicSnalysis
{
    /// <summary>
    /// 酷狗音乐解析
    /// </summary>
    public class KuGou
    {
        /// <summary>
        /// 是否使用CDN
        /// </summary>
        public static bool CDN = false;
        /// <summary>
        /// 音乐搜索
        /// </summary>
        /// <param name="query">关键词</param>
        /// <param name="page">页码</param>
        /// <param name="pagesize">每页个数</param>
        /// <param name="lrc">是否返回歌词</param>
        /// <param name="format">返回数据格式</param>
        /// <returns>数据</returns>
        public static List<MusicInfo> Search(string query, int page = 1, int pagesize = 10, bool lrc = false)
        {
            Http http = new Http();
            string url = CDN ? "http://mobilecdn.kugou.com/api/v3/search/song" : "http://songsearch.kugou.com/song_search_v2";
            string refer = CDN ? "http://m.kugou.com" : "http://www.kugou.com";
            string get = string.Format("?keyword={0}&platform=WebFilter&format={1}&page={2}&pagesize={3}", query, "json", page, pagesize);
            string res = http.Send(url + get, null, refer);
            JObject data = JObject.Parse(res);
            List<MusicInfo> list = new List<MusicInfo>();
            string key = CDN ? "info" : "lists";
            try
            {
                if (data["status"].ToString().Equals("1"))
                {
                    //拼接列表
                    foreach (var item in data["data"][key])
                    {
                        string hash = null;
                        if (!CDN)
                        {
                            hash = item["HQFileHash"].ToString();
                            hash = string.IsNullOrEmpty(hash) ? item["FileHash"].ToString() : hash;
                        }
                        else
                        {
                            hash = item["320hash"].ToString().Equals("") ? item["hash"].ToString() : item["320hash"].ToString();
                        }
                        if (string.IsNullOrEmpty(hash)) continue;
                        MusicInfo music = new MusicInfo();
                        music = Song(hash, lrc);
                        music.AlbumName = item["AlbumName"].ToString();
                        if (!string.IsNullOrEmpty(music.Url))
                        {
                            list.Add(music);
                        }
                    }
                }
            }
            catch
            {

                return list;
            }
            return list;
        }
        /// <summary>
        /// 歌曲信息
        /// </summary>
        /// <param name="SongID">歌曲ID</param>
        /// <param name="lrc">是否返回歌词</param>
        /// <returns></returns>
        public static MusicInfo Song(string SongID, bool lrc = false)
        {
            MusicInfo music = new MusicInfo();
            Http http = new Http();
            string url = "http://m.kugou.com/app/i/getSongInfo.php";
            string refer = "http://m.kugou.com/play/info/" + SongID;
            string ua = "Mozilla/5.0 (iPhone; CPU iPhone OS 9_1 like Mac OS X) AppleWebKit/601.1.46 (KHTML, like Gecko) Version/9.0 Mobile/13B143 Safari/601.1";
            string get = "?cmd=playInfo&hash=" + SongID;
            string res = http.Send(url + get, null, refer, null, null, ua);
            try
            {
                JObject data = JObject.Parse(res);
                if (string.IsNullOrEmpty(data["url"].ToString()))
                {
                    return music;
                }
                else
                {
                    string pic1 = data["album_img"].ToString().Replace("{size}", "150");
                    string pic2 = data["imgUrl"].ToString().Replace("{size}", "150");
                    music.Type = "KuGou";
                    music.Link = "http://www.kugou.com/song/#hash=" + SongID;
                    music.SongID = SongID;
                    music.SongName = data["songName"].ToString();
                    music.SingerName = data["singerName"].ToString();
                    music.Url = data["url"].ToString().Replace("http://", "https://");
                    music.Lrc = lrc ? Lrc(SongID) : "";
                    music.Logo = string.IsNullOrEmpty(pic1) ? pic2 : pic1;
                }
            }
            catch
            {

                return music;
            }
            return music;
        }
        /// <summary>
        /// 获取歌词
        /// </summary>
        /// <param name="SongID">歌曲ID</param>
        /// <returns></returns>
        public static string Lrc(string SongID)
        {
            Http http = new Http();
            string url = "http://m.kugou.com/app/i/krc.php";
            string refer = "http://m.kugou.com/play/info/" + SongID;
            string ua = "Mozilla/5.0 (iPhone; CPU iPhone OS 9_1 like Mac OS X) AppleWebKit/601.1.46 (KHTML, like Gecko) Version/9.0 Mobile/13B143 Safari/601.1";
            string get = "?cmd=100&timelength=999999&hash=" + SongID;
            try
            {
                return http.Send(url + get, null, refer, null, null, ua);
            }
            catch
            {

                return "";
            }
        }
    }
}
