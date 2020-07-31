using System;
using SinKingMusicSnalysis;
using System.Collections.Generic;
using System.Text;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {

            /*MusicInfo music = MiGu.Song("60054701923",false);
            Console.WriteLine(music.SongName);
            Console.WriteLine(music.SingerName);
            Console.WriteLine(music.AlbumName);
            Console.WriteLine(music.Link);
            Console.WriteLine(music.Logo);
            Console.WriteLine(music.Lrc);
            Console.WriteLine(music.Type);
            Console.WriteLine(music.Url);*/
            List<MusicInfo> list = MiGu.Search("周杰伦");
            foreach (var music in list)
            {
                Console.WriteLine(music.SongName);
                Console.WriteLine(music.Url);
                Console.WriteLine("");
            }
        }
    }
}
