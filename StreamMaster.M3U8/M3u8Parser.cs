using System.Net.Http.Headers;
using System.Text.RegularExpressions;

public class M3u8Parser(string clientUserAgent)
{

    public async Task<M3u8Playlist?> ParsePlaylistAsync(string playlistUri)
    {
        M3u8Playlist playlist = new()
        {
            Uri = playlistUri,
            Streams = new List<M3u8Stream>()
        };

        HttpClient httpClient = new(new HttpClientHandler()
        {
            AllowAutoRedirect = true,
        });
        httpClient.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("gzip"));
        httpClient.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("deflate"));
        httpClient.DefaultRequestHeaders.UserAgent.ParseAdd(clientUserAgent);

        HttpResponseMessage response = await httpClient.GetAsync(playlistUri, HttpCompletionOption.ResponseHeadersRead);
        if (!response.IsSuccessStatusCode)
        {
            httpClient.Dispose();
            return null;
        }

        if (response.RequestMessage.RequestUri != null)
        {
            playlistUri = response.RequestMessage.RequestUri.ToString();
        }

        string? contentType = response.Content.Headers.ContentType?.MediaType;
        if (contentType.ToLower() is not "application/vnd.apple.mpegurl" and "application/x-mpegURL")
        {
            httpClient.Dispose();
            return null;
        }

        string[] lines = (await httpClient.GetStringAsync(playlistUri).ConfigureAwait(false)).Split("\n");

        for (int i = 0; i < lines.Length; i++)
        {
            string line = lines[i].Trim();

            if (line.StartsWith("#EXT-X-STREAM-INF:"))
            {
                string attributeString = line["#EXT-X-STREAM-INF:".Length..].Trim();

                MatchCollection attributeMatches = Regex.Matches(attributeString, @"([A-Z\-]+)=(""([^""]+)""|([^"",]+))");

                Dictionary<string, string> attributes = new();

                foreach (Match match in attributeMatches.Cast<Match>())
                {
                    string attributeName = match.Groups[1].Value;
                    string attributeValue = match.Groups[3].Success ? match.Groups[3].Value : match.Groups[4].Value;

                    attributes[attributeName] = attributeValue;
                }

                Uri streamUri = new(new Uri(playlistUri), lines[++i].Trim());

                _ = int.TryParse(GetValueByKey("AVERAGE-BANDWIDTH"), out int averageBandwidth);
                _ = int.TryParse(GetValueByKey("BANDWIDTH"), out int bandwidth);
                _ = int.TryParse(GetValueByKey("FRAME-RATE"), out int frameRate);

                M3u8Stream stream = new()
                {
                    AverageBandwidth = averageBandwidth,
                    Bandwidth = bandwidth,
                    Codecs = GetValueByKey("CODECS"),
                    Resolution = GetValueByKey("RESOLUTION"),
                    FrameRate = frameRate,
                    ClosedCaptions = GetValueByKey("CLOSED-CAPTIONS"),
                    Audio = GetValueByKey("AUDIO"),
                    Subtitles = GetValueByKey("SUBTITLES"),
                    Uri = streamUri.AbsoluteUri//lines[++i].Trim()
                };

                playlist.Streams.Add(stream);

                string? GetValueByKey(string key)
                {
                    return attributes.ContainsKey(key) ? attributes[key] : null;
                }
            }
            else if (line.StartsWith("#EXT-X-TARGETDURATION:"))
            {
                playlist.TargetDuration = int.Parse(line["#EXT-X-TARGETDURATION:".Length..].Trim());
            }
            else if (line.StartsWith("#EXT-X-VERSION:"))
            {
                playlist.Version = int.Parse(line["#EXT-X-VERSION:".Length..].Trim());
            }
            else if (line.StartsWith("#EXT-X-MEDIA-SEQUENCE:"))
            {
                playlist.MediaSequence = int.Parse(line["#EXT-X-MEDIA-SEQUENCE:".Length..].Trim());
            }
            else if (line.StartsWith("#EXT-X-PLAYLIST-TYPE:"))
            {
                playlist.PlaylistType = line["#EXT-X-PLAYLIST-TYPE:".Length..].Trim();
            }
            else if (line.StartsWith("#EXT-X-INDEPENDENT-SEGMENTS"))
            {
                playlist.IndependentSegments = true;
            }
        }

        Uri baseUri = new(playlistUri);

        List<M3u8Segment> test = GetM3U8Segment(lines, playlist.MediaSequence, baseUri);

        if (test.Any())
        {
            playlist.Streams = new List<M3u8Stream>
            {
                new M3u8Stream
                {
                    Segments = test
                }
            };
            return playlist;
        }

        // Now we parse child playlists
        foreach (M3u8Stream stream in playlist.Streams)
        {
            stream.Segments = new List<M3u8Segment>();

            Uri streamUri = new(baseUri, stream.Uri);
            string[] streamLines = (await httpClient.GetStringAsync(streamUri).ConfigureAwait(false)).Split("\n");

            stream.Segments = GetM3U8Segment(streamLines, playlist.MediaSequence, streamUri);

            // Sort segments based on BYTERANGE start position
            //stream.Segments.Sort((seg1, seg2) => seg1.ByterangeStart.CompareTo(seg2.ByterangeStart));
        }

        return playlist;
    }

    private List<M3u8Segment> GetM3U8Segment(string[] streamLines, int mediaSequence, Uri streamUri)
    {
        List<M3u8Segment> segments = new();

        M3u8Segment? segment = null;

        bool isPlayListFile = streamLines.Any(a => a.Contains("#EXT-X-STREAM-INF"));

        for (int i = 0; i < streamLines.Length; i++)
        {
            string line = streamLines[i].Trim();
            if (string.IsNullOrEmpty(line))
            {
                continue;
            }
            if (line.StartsWith("#EXTINF:"))
            {
                segment = new M3u8Segment
                {
                    Duration = double.Parse(line["#EXTINF:".Length..line.IndexOf(",")]),
                    MediaSequence = mediaSequence++,
                };
            }
            else if (line.StartsWith("#EXT-X-BYTERANGE:"))
            {
                long[] byterangeParts = line["#EXT-X-BYTERANGE:".Length..]
                                        .Split('@')
                                        .Select(long.Parse)
                                        .ToArray();
                segment.ByterangeLength = byterangeParts[0];
                segment.ByterangeStart = byterangeParts.Length > 1 ? byterangeParts[1] : 0;
            }
            else if (!line.StartsWith("#") && !isPlayListFile)
            {
                segment.Uri = new Uri(streamUri, line).AbsoluteUri;
                segments.Add(segment);
            }
        }
        return segments;
    }

    private bool IsM3u8Stream(string content)
    {
        return content.TrimStart().StartsWith("#EXTM3U");
    }
}
