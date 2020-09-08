using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SinKingMusicSnalysis.Common;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;

namespace SinKingMusicSnalysis
{
    public class QQ
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
            string url = "http://c.y.qq.com/soso/fcgi-bin/search_for_qq_cp";
            string refer = "http://m.y.qq.com";
            string ua = "Mozilla/5.0 (iPhone; CPU iPhone OS 9_1 like Mac OS X) AppleWebKit/601.1.46 (KHTML, like Gecko) Version/9.0 Mobile/13B143 Safari/601.1";
            string get = string.Format("?w={0}&p={1}&n={2}&format={3}", query, page, pagesize, "json");
            string res = http.Send(url + get, null, refer, null, null, ua);
            JObject data = JObject.Parse(res);
            List<MusicInfo> list = new List<MusicInfo>();
            try
            {
                if (!data["code"].ToString().Equals("0"))
                {
                    return list;
                }
                if (string.IsNullOrEmpty(data["data"]["song"]["list"].ToString()))
                {
                    return list;
                }
                foreach (var item in data["data"]["song"]["list"])
                {
                    if (item["pay"]["payplay"].ToString().Equals("1"))
                    {
                        continue;
                    }
                    string author = "";
                    foreach (var authors in item["singer"])
                    {
                        author += authors["name"].ToString() + ",";
                    }
                    author = author.Remove(author.Length - 1, 1);
                    MusicInfo music = new MusicInfo()
                    {
                        Type = "QQ",
                        Link = "http://y.qq.com/n/yqq/song/" + item["songmid"].ToString() + ".html",
                        SongID = item["songmid"].ToString(),
                        SongName = item["songname"].ToString(),
                        SingerName = author,
                        Lrc = lrc ? Lrc(item["songmid"].ToString()) : "",
                        Url = Url(item["songmid"].ToString()),
                        Logo = "http://y.gtimg.cn/music/photo_new/T002R150x150M000" + item["albummid"].ToString() + ".jpg",
                        AlbumName = item["albumname"].ToString()
                    };
                    list.Add(music);
                }
                return list;
            }
            catch
            {

                return list;
            }
        }
        /// <summary>
        /// 歌曲信息
        /// </summary>
        /// <param name="SongID">歌曲ID</param>
        /// <param name="lrc">是否返回歌词</param>
        /// <returns></returns>
        public static MusicInfo Song(string SongID, bool lrc = false)
        {
            Http http = new Http();
            string url = "http://c.y.qq.com/v8/fcg-bin/fcg_play_single_song.fcg";
            string refer = "http://m.y.qq.com";
            string get = "?songmid=" + SongID + "&format=json";
            string ua = "Mozilla/5.0 (iPhone; CPU iPhone OS 9_1 like Mac OS X) AppleWebKit/601.1.46 (KHTML, like Gecko) Version/9.0 Mobile/13B143 Safari/601.1";
            string res = http.Send(url + get, null, refer, null, null, ua);
            MusicInfo music = new MusicInfo();
            try
            {
                JObject data = JObject.Parse(res);
                JToken info = data["data"][0];
                if (string.IsNullOrEmpty(info.ToString()))
                {
                    return music;
                }
                if (info["pay"]["pay_play"].ToString().Equals("1"))
                {
                    return music;
                }
                string author = "";
                foreach (var authors in info["singer"])
                {
                    author += authors["title"].ToString() + ",";
                }
                author = author.Remove(author.Length - 1, 1);
                music.Type = "QQ";
                music.Link = "http://y.qq.com/n/yqq/song/" + info["mid"].ToString() + ".html";
                music.SongID = info["mid"].ToString();
                music.SongName = info["title"].ToString();
                music.SingerName = author;
                music.Lrc = lrc ? Lrc(info["mid"].ToString()) : "";
                music.Url = Url(info["mid"].ToString());
                music.Logo = "http://y.gtimg.cn/music/photo_new/T002R150x150M000" + info["album"]["mid"].ToString() + ".jpg";
                music.AlbumName = info["album"]["name"].ToString();
                return music;
            }
            catch
            {
                return music;
            }
        }
        private static string Url(string SongID)
        {
            Http http = new Http();
            string url = "http://u.y.qq.com/cgi-bin/musicu.fcg";
            string refer = "http://y.qq.com/portal/player.html";
            string ua = "Mozilla/5.0 (iPhone; CPU iPhone OS 9_1 like Mac OS X) AppleWebKit/601.1.46 (KHTML, like Gecko) Version/9.0 Mobile/13B143 Safari/601.1";
            Dictionary<object, object> datas = new Dictionary<object, object>();
            string guid = new Random().Next(111111111, 999999999).ToString();
            datas.Add("req", new Dictionary<object, object>() {
                {"module","CDN.SrfCdnDispatchServer" },
                { "method","GetCdnDispatch"},
                {"param",
                    new Dictionary<object, object>() {
                        { "guid",guid},
                        { "calltype","0"},
                        { "userip",""}
                    }
                },
            });
            datas.Add("req_0", new Dictionary<object, object>() {
                {"module","vkey.GetVkeyServer" },
                { "method","CgiGetVkey"},
                {"param",
                    new Dictionary<object, object>() {
                        { "guid",guid},
                        { "songmid",new string[]{ SongID} },
                        { "songtype",new int[]{ 0 } },
                        { "uin","0"},
                        { "loginflag",1},
                        { "platform","20"}
                    }
                },
            });
            datas.Add("comm", new Dictionary<object, object>() {
                {"uin",0 },
                { "format","json"},
                {"ct",24 },
                {"cv",0 }
            });
            string get = "?data=" + JsonConvert.SerializeObject(datas);
            string res = http.Send(url + get, null, refer, null, null, ua);
            try
            {
                JObject data = JObject.Parse(res);
                if (data["code"].ToString().Equals("0"))
                {
                    return ("http://ws.stream.qqmusic.qq.com/" + data["req_0"]["data"]["midurlinfo"][0]["purl"].ToString()).Replace("http://", "https://");
                }
                else
                {
                    return "";
                }
            }
            catch
            {
                return "";
            }
        }
        /// <summary>
        /// 获取歌词
        /// </summary>
        /// <param name="SongID">歌曲ID</param>
        /// <returns></returns>
        public static string Lrc(string SongID)
        {
            Http http = new Http();
            string url = "http://c.y.qq.com/lyric/fcgi-bin/fcg_query_lyric.fcg";
            string refer = "http://m.y.qq.com";
            string ua = "Mozilla/5.0 (iPhone; CPU iPhone OS 9_1 like Mac OS X) AppleWebKit/601.1.46 (KHTML, like Gecko) Version/9.0 Mobile/13B143 Safari/601.1";
            string get = "?songmid=" + SongID + "&format=json&nobase64=1&songtype=0&callback=c";
            string res = Regex.Replace(http.Send(url + get, null, refer, null, null, ua), @"(.*\()(.*)(\).*)", "$2");
            JObject data = JObject.Parse(res);
            try
            {
                if (data["retcode"].ToString().Equals("0"))
                {
                    return WebUtility.HtmlDecode(data["lyric"].ToString());
                }
            }
            catch
            {

                return "";
            }
            return "";
        }
    }
}
