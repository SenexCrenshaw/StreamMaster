using System.Text;

namespace StreamMaster.Streams.Services
{
    public class M3U8Generator : IM3U8Generator
    {
        private readonly IIntroPlayListBuilder _introPlayListBuilder;

        public M3U8Generator(IIntroPlayListBuilder introPlayListBuilder)
        {
            _introPlayListBuilder = introPlayListBuilder;
        }

        public string CreateM3U8Content(List<string> tsFiles, bool insertIntros = false)
        {
            StringBuilder m3u8Content = new();

            // Add the header for the M3U8 file
            m3u8Content.AppendLine("#EXTM3U");
            m3u8Content.AppendLine("#EXT-X-VERSION:3");
            m3u8Content.AppendLine("#EXT-X-ALLOW-CACHE:NO");
            m3u8Content.AppendLine("#EXT-X-TARGETDURATION:2");
            m3u8Content.AppendLine("#EXT-X-MEDIA-SEQUENCE:0");
            //m3u8Content.AppendLine("#EXT-X-DISCONTINUITY-SEQUENCE:0");
            m3u8Content.AppendLine();

            // Add each TS file to the playlist
            for (int i = 0; i < tsFiles.Count; i++)
            {
                string tsFile = tsFiles[i];

                if (insertIntros)
                {
                    string introTs = _introPlayListBuilder.GetRandomSMStreamIntro();
                    if (!string.IsNullOrEmpty(introTs))
                    {
                        //if (introTs.StartsWith("http://127.0.0.1:7095"))
                        //{
                        //    introTs = introTs[21..];
                        //}
                        m3u8Content.AppendLine("#EXT-X-DISCONTINUITY;");
                        m3u8Content.AppendLine("#EXTINF:1,");
                        m3u8Content.Append(introTs);
                        m3u8Content.AppendLine();
                    }
                }

                m3u8Content.AppendLine("#EXT-X-DISCONTINUITY;");
                m3u8Content.AppendLine("#EXTINF:1,");
                m3u8Content.Append(tsFile);
                m3u8Content.AppendLine();

                // Only append a new line if it's not the last TS file
                if (i < tsFiles.Count - 1)
                {
                    m3u8Content.AppendLine();
                }
            }

            // Return the content as a string
            return m3u8Content.ToString();
        }
    }
}
