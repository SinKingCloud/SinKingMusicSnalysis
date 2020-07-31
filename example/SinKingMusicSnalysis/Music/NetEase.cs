using Newtonsoft.Json.Linq;
using SinKingMusicSnalysis.Common;
using System;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography;
using Newtonsoft.Json;
using ICSharpCode.SharpZipLib.Zip.Compression.Streams;
using System.IO;

namespace SinKingMusicSnalysis
{
    /// <summary>
    /// 网易云音乐解析
    /// </summary>
    public class NetEase
    {

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
            string url = "http://music.163.com/api/linux/forward";
            string refer = "http://music.163.com/";
            string offset = (page * pagesize - pagesize).ToString();
            string sign = "{\"method\":\"POST\",\"url\":\"http://music.163.com/api/cloudsearch/pc\",\"params\":{\"s\":\"" + query + "\",\"type\":1,\"offset\":" + offset + ",\"limit\":" + pagesize + "}}";
            string[] headers = { "Content-Type:application/x-www-form-urlencoded" };
            string post = "eparams=" + DataEnCode(sign);
            string ua = "Mozilla/5.0 (Macintosh; Intel Mac OS X 10_12_5) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/60.0.3112.50 Safari/537.36";
            http.gzip = true;
            string res = http.Send(url, post, refer, null, headers, ua);
            List<MusicInfo> list = new List<MusicInfo>();
            try
            {
                JObject data = JObject.Parse(res);
                if (data["code"].ToString() != "200")
                {
                    return list;
                }
                foreach (var item in data["result"]["songs"])
                {
                    string author = "";
                    foreach (var authors in item["ar"])
                    {
                        author += authors["name"].ToString() + ",";
                    }
                    http.Send("http://music.163.com/song/media/outer/url?id=" + item["id"].ToString() + ".mp3");
                    MusicInfo music = new MusicInfo()
                    {
                        Type = "NetEase",
                        Link = "http://music.163.com/#/song?id=" + item["id"].ToString(),
                        SongID = item["id"].ToString(),
                        SongName = item["name"].ToString(),
                        SingerName = author,
                        Lrc = lrc ? Lrc(item["id"].ToString()) : "",
                        Url = http.Headers["location"].Replace("http://", "https://"),
                        Logo = item["al"]["picUrl"].ToString() + "?param=150x150",
                        AlbumName = item["al"]["name"].ToString()
                    };
                    list.Add(music);
                }
            }
            catch
            {

                return list;
            }
            return list;
        }
        /// <summary>
        /// 获取歌词
        /// </summary>
        /// <param name="SongID">歌曲ID</param>
        /// <returns></returns>
        public static string Lrc(string SongID)
        {
            Http http = new Http();
            string url = "http://music.163.com/api/linux/forward";
            string refer = "http://music.163.com/";
            string sign = "{\"method\":\"GET\",\"url\":\"http://music.163.com/api/song/lyric\",\"params\":{\"id\":\"" + SongID + "\",\"lv\":1}}";
            string[] headers = { "Content-Type:application/x-www-form-urlencoded" };
            string post = "eparams=" + DataEnCode(sign);
            string ua = "Mozilla/5.0 (Macintosh; Intel Mac OS X 10_12_5) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/60.0.3112.50 Safari/537.36";
            http.gzip = true;
            string res = http.Send(url, post, refer, null, headers, ua);
            JObject data = JObject.Parse(res);
            try
            {
                if (string.IsNullOrEmpty(data["lrc"]["lyric"].ToString()))
                {
                    return "";
                }
                else
                {
                    return data["lrc"]["lyric"].ToString();
                }
            }
            catch
            {
                return "";
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
            string url = "http://music.163.com/api/linux/forward";
            string refer = "http://music.163.com/";
            string sign = "{\"method\":\"GET\",\"url\":\"http://music.163.com/api/song/detail\",\"params\":{\"id\":\"" + SongID + "\",\"ids\":\"[" + SongID + "]\"}}";
            string[] headers = { "Content-Type:application/x-www-form-urlencoded" };
            string post = "eparams=" + DataEnCode(sign);
            string ua = "Mozilla/5.0 (Macintosh; Intel Mac OS X 10_12_5) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/60.0.3112.50 Safari/537.36";
            http.gzip = true;
            string res = http.Send(url, post, refer, null, headers, ua);
            MusicInfo music = new MusicInfo();
            JObject data = JObject.Parse(res);
            try
            {
                JToken info = data["songs"][0];
                if (string.IsNullOrEmpty(info.ToString()))
                {
                    return music;
                }
                string author = "";
                foreach (var authors in info["artists"])
                {
                    author += authors["name"].ToString() + ",";
                }
                author = author.Remove(author.Length - 1, 1);
                music.Type = "NetEase";
                music.Link = "http://music.163.com/#/song?id=" + info["id"].ToString();
                music.SongID = info["id"].ToString();
                music.SongName = info["name"].ToString();
                music.SingerName = author;
                music.Lrc = lrc ? Lrc(info["id"].ToString()) : "";
                http.AllowAutoRedirect = false;
                http.Send("http://music.163.com/song/media/outer/url?id=" + info["id"].ToString() + ".mp3");
                music.Url = http.Headers["location"].Replace("http://", "https://");
                music.Logo = info["album"]["picUrl"].ToString() + "?param=100x100";
                music.AlbumName = info["album"]["name"].ToString();
                return music;
            }
            catch
            {
                return music;
            }
        }
        /// <summary>
        /// 歌单音乐列表获取
        /// </summary>
        /// <param name="SheetID">歌单ID</param>
        /// <returns></returns>
        public static List<MusicInfo> Sheet(string SheetID)
        {
            Http http = new Http();
            string url = "http://music.163.com/api/v3/playlist/detail?id="+ SheetID;
            string refer = "http://music.163.com/";
            string ua = "Mozilla/5.0 (Macintosh; Intel Mac OS X 10_12_5) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/60.0.3112.50 Safari/537.36";
            string res = http.Send(url, null, refer, null, null, ua);
            JObject data = JObject.Parse(res);
            List<MusicInfo> list = new List<MusicInfo>();
            if (data["code"].ToString().Equals("200"))
            {
                string ids = "";
                List<string> idss = new List<string>();
                int i = 1;
                foreach (var item in data["playlist"]["trackIds"])
                {
                    ids += item["id"].ToString() + ",";
                    if (i==200)
                    {
                        ids = ids.Remove(ids.Length - 1, 1);
                        idss.Add(ids);
                        ids = "";
                        i = 0;
                    }
                    i++;
                }
                if (ids!="")
                {
                    ids = ids.Remove(ids.Length - 1, 1);
                    idss.Add(ids);
                }
                foreach (var ids2 in idss)
                {
                    url = "http://music.163.com/api/linux/forward";
                    refer = "http://music.163.com/";
                    string sign = "{\"method\":\"GET\",\"url\":\"http://music.163.com/api/song/detail\",\"params\":{\"id\":\"" + ids2 + "\",\"ids\":\"[" + ids2 + "]\"}}";
                    string[] headers = { "Content-Type:application/x-www-form-urlencoded" };
                    string post = "eparams=" + DataEnCode(sign);
                    ua = "Mozilla/5.0 (Macintosh; Intel Mac OS X 10_12_5) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/60.0.3112.50 Safari/537.36";
                    http.gzip = true;
                    res = http.Send(url, post, refer, null, headers, ua);
                    data = JObject.Parse(res);
                    foreach (var item in data["songs"])
                    {
                        MusicInfo music = new MusicInfo();
                        JToken info = item;
                        string author = "";
                        foreach (var authors in info["artists"])
                        {
                            author += authors["name"].ToString() + ",";
                        }
                        author = author.Remove(author.Length - 1, 1);
                        music.Type = "NetEase";
                        music.Link = "http://music.163.com/#/song?id=" + info["id"].ToString();
                        music.SongID = info["id"].ToString();
                        music.SongName = info["name"].ToString();
                        music.SingerName = author;
                        music.Lrc = "";
                        music.Url = "http://music.163.com/song/media/outer/url?id=" + info["id"].ToString() + ".mp3";
                        music.Logo = info["album"]["picUrl"].ToString() + "?param=100x100";
                        music.AlbumName = info["album"]["name"].ToString();
                        list.Add(music);
                    }
                }
            }
            return list;
        }
        /// <summary>
        /// 网易云参数加密
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        private static string DataEnCode(string str)
        {
            string hash = "rFgB&h#%2?^eDg:Q";
            string data = AES.Encrypt128(str, hash);
            byte[] bytes = Convert.FromBase64String(data);
            return BytesTranfer.ByteArrayToHexString(bytes);
        }

    }
}
