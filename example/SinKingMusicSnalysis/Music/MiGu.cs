using SinKingMusicSnalysis.Common;
using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using System.Security.Cryptography;
using System.Linq;
using System.Text.RegularExpressions;
using System.Net;

namespace SinKingMusicSnalysis
{
    public class MiGu
    {
        /// <summary>
        /// 音乐搜索
        /// </summary>
        /// <param name="query">关键词</param>
        /// <param name="page">页码</param>
        /// <param name="pagesize">每页个数</param>
        /// <param name="lrc">是否返回歌词</param>
        /// <returns>数据</returns>
        public static List<MusicInfo> Search(string query, int page = 1, int pagesize = 10, bool lrc = false)
        {
            Http http = new Http();
            List<MusicInfo> list = new List<MusicInfo>();
            string url = "https://m.music.migu.cn/migu/remoting/scr_search_tag?rows=" + pagesize + "&type=2&keyword=" + query + "&pgc=" + page;
            string res = http.Send(url);
            JObject data = JObject.Parse(res);
            if (data["musics"].Count() <= 0)
            {
                return list;
            }
            foreach (var item in data["musics"])
            {
                if (string.IsNullOrEmpty(item["mp3"].ToString())) continue;
                MusicInfo music = new MusicInfo()
                {
                    Type = "MiGu",
                    Link = "https://music.migu.cn/v3/music/song/" + item["copyrightId"].ToString(),
                    SongID = item["copyrightId"].ToString(),
                    SongName = item["songName"].ToString(),
                    SingerName = item["singerName"].ToString(),
                    Lrc = lrc ? Lrc(item["copyrightId"].ToString()) : "",
                    Url = item["mp3"].ToString().Replace("http://","https://"),
                    Logo = item["cover"].ToString(),
                    AlbumName = item["albumName"].ToString()
                };
                list.Add(music);
            }
            return list;
        }
        /// <summary>
        /// 歌曲信息
        /// </summary>
        /// <param name="SongID">歌曲ID</param>
        /// <param name="lrc">是否返回歌词</param>
        /// <returns></returns>
        public static MusicInfo Song(string SongID, bool lrc = true)
        {
            MusicInfo music = new MusicInfo();
            Http http = new Http();
            string res = http.Send("https://m.music.migu.cn/migu/remoting/cms_detail_tag?cpid=" + SongID);
            JObject data = JObject.Parse(res);
            if (string.IsNullOrEmpty(data["data"].ToString()) || string.IsNullOrEmpty(data["data"]["songId"].ToString()))
            {
                return music;
            }
            string songID = data["data"]["songId"].ToString();
            string lrcText = data["data"]["lyricLrc"].ToString();
            string logo = data["data"]["picS"].ToString();
            string url = "http://app.c.nf.migu.cn/MIGUM2.0/v2.0/content/listen-url";
            string post = "netType=01&resourceType=E&songId=" + songID + "&toneFlag=PQ&dataType=2";
            string referer = "https://music.migu.cn/v3/music/player/audio";
            string[] header = new string[] {
                "Content-Type:application/x-www-form-urlencoded",
                "channel:0146951"
            };
            http.gzip = true;
            res = http.Send(url, post, referer, null, header);
            data = JObject.Parse(res);
            music.Type = "MiGu";
            music.Link = "https://music.migu.cn/v3/music/song/" + SongID;
            music.SongID = SongID;
            music.SongName = data["data"]["songItem"]["songName"].ToString();
            music.SingerName = data["data"]["songItem"]["singer"].ToString();
            music.Lrc = lrc ? lrcText : "";
            music.Url = data["data"]["url"].ToString().Replace("http://", "https://");
            music.Logo = logo;
            music.AlbumName = data["data"]["songItem"]["album"].ToString();
            return music;
        }
        /// <summary>
        /// 歌词获取
        /// </summary>
        /// <param name="id">歌曲ID</param>
        /// <returns></returns>
        public static string Lrc(string id)
        {
            try
            {
                string url = "https://music.migu.cn/v3/api/music/audioPlayer/getLyric?copyrightId=" + id;
                string referer = "https://music.migu.cn/v3/music/player/audio";
                Http http = new Http();
                string res = http.Send(url, null, referer);
                JObject data = JObject.Parse(res);
                return data["lyric"].ToString();
            }
            catch
            {
                return "";
            }
        }
    }
}
