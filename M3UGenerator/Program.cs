﻿namespace M3UGenerator
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            int count = 100;

            if (args.Length == 1)
            {
                int.TryParse(args[0], out count);
            }

            List<string> m3u = BuildM3ULineForVideoStream(count);
            File.WriteAllLines($"test_{count}.m3u", m3u);
        }

        private static List<string> BuildM3ULineForVideoStream(int m3uCount = 100)
        {
            List<string> lines = [];
            lines.Add("#EXTM3U");
            for (int i = 0; i < m3uCount; i++)
            {
                string id = Guid.NewGuid().ToString();
                lines.Add($"#EXTINF:-1 tvg-id=\"Channel_{i}\" tvg-name=\"Channel {i}\" tvg-logo=\"https://logo{i}.png\" group-title =\"TEST CHANNEL GROUP\", Channel {i}");
                lines.Add($"http://channelfake.test/live/{id}.ts");
            }
            return lines;// string.Join("\r\n", lines.ToArray());
        }


    }

}
