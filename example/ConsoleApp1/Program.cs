using System;
using SinKingMusicSnalysis;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SinKingMusicSnalysis.Common;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            MusicInfo music = NetEase.Song("1344897943", false);
            string test = JsonConvert.SerializeObject(new { Code = 1, Msg = "获取成功！", Data = music });
            Console.WriteLine(test);
            MusicInfo music1 = QQ.Song("002w57E00BGzXn", false);
             test = JsonConvert.SerializeObject(new { Code = 1, Msg = "获取成功！", Data = music1 });
            Console.WriteLine(test);
            MusicInfo music2 = KuGou.Song("E032025080D9ACB5F5ACD5918D8A1758", false);
             test = JsonConvert.SerializeObject(new { Code = 1, Msg = "获取成功！", Data = music2 });
            Console.WriteLine(test);
            MusicInfo music3 = MiGu.Song("60054704037",false);
             test = JsonConvert.SerializeObject(new { Code = 1, Msg = "获取成功！", Data = music3 });
            Console.WriteLine(test);
        }
    }
}
