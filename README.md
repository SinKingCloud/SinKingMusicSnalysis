# SinKingMusicSnalysis
因近期音乐项目需要用到音乐解析，c#音乐解析资源较少，便自己写了一个，支持酷狗音乐、网易云英语、QQ音乐、咪咕音乐歌曲信息解析及搜索等，使用.net core编写，支持macos linux win等平台
### 使用方法
##### 1.导入DLL
导入SinKingMusicSnalysis项目中的程序SinKingMusicSnalysis.dll即可
##### 2.导入命名空间
```csharp
using SinKingMusicSnalysis;
```
##### 3.引用方法
```csharp
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
            //搜索
            List<MusicInfo> list = KuGou.Search("salt", 1, 1,true);//KuGou,QQ,NetEase
            foreach (var item in list)
            {
                Console.WriteLine(item.SongName);
                Console.WriteLine(item.SingerName);
                Console.WriteLine(item.SongID);
                Console.WriteLine(item.Url);
                Console.WriteLine(item.Logo);
                Console.WriteLine(item.AlbumName);
                Console.WriteLine(item.SongID);
                Console.WriteLine(item.Lrc);
                Console.WriteLine("\n");
            }
            //单曲信息
            MusicInfo info =  QQ.Song("001Xtqd62Iqwvj",true);//KuGou,QQ,NetEase
            Console.WriteLine(info.SongName);
            Console.WriteLine(info.SingerName);
            Console.WriteLine(info.SongID);
            Console.WriteLine(info.Url);
            Console.WriteLine(info.Logo);
            Console.WriteLine(info.AlbumName);
            Console.WriteLine(info.Lrc);
            Console.WriteLine(info.Link);
            Console.WriteLine("\n");
            //获取歌词
            Console.WriteLine(QQ.Lrc("001Xtqd62Iqwvj"));//KuGou,QQ,NetEase
        }
        
    }
}

```
