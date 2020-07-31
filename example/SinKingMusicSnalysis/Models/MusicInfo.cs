using System;
using System.Collections.Generic;
using System.Text;

namespace SinKingMusicSnalysis
{
    public class MusicInfo
    {
        /// <summary>
        /// 来源
        /// </summary>
        public string Type { get; set; }
        /// <summary>
        /// 音乐主页链接
        /// </summary>
        public string Link { get; set; }
        /// <summary>
        /// 音乐ID
        /// </summary>
        public string SongID { get; set; }
        /// <summary>
        /// 音乐名称
        /// </summary>
        public string SongName { get; set; }
        /// <summary>
        /// 作者
        /// </summary>
        public string SingerName { get; set; }
        /// <summary>
        /// 歌词
        /// </summary>
        public string Lrc { get; set; }
        /// <summary>
        /// 歌曲地址
        /// </summary>
        public string Url { get; set; }
        /// <summary>
        /// 歌曲Logo
        /// </summary>
        public string Logo { get; set; }
        /// <summary>
        /// 专辑名称
        /// </summary>
        public string AlbumName { get; set; }
    }
}
