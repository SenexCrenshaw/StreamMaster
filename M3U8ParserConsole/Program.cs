﻿namespace M3U8ParserConsole
{
    internal class Program
    {
        private static async Task Main(string[] args)
        {
            string m3u8Url = "http://qthttp.apple.com.edgesuite.net/1010qwoeiuryfg/sl.m3u8";
            m3u8Url = "https://devstreaming-cdn.apple.com/videos/streaming/examples/img_bipbop_adv_example_fmp4/master.m3u8";

            M3u8Parser parser = new("Mozilla/5.0 (compatible; streammaster/1.0)");
            M3u8Playlist? m3u8 = await parser.ParsePlaylistAsync(m3u8Url);

            Console.WriteLine(m3u8);
        }
    }
}
