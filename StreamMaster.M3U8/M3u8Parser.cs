using System.Text.RegularExpressions;

public class M3u8Parser
{
    public async Task<M3u8Playlist?> ParsePlaylistAsync(string playlistUri)
    {
        var playlist = new M3u8Playlist
        {
            Uri = playlistUri,
            Streams = new List<M3u8Stream>()
        };

        using var httpClient = new HttpClient();

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

        string contentType = response.Content.Headers.ContentType?.MediaType;
        if (contentType.ToLower() != "application/vnd.apple.mpegurl" && contentType.ToLower() == "application/x-mpegURL")
        {
            httpClient.Dispose();
            return null;
        }

        var lines = (await httpClient.GetStringAsync(playlistUri).ConfigureAwait(false)).Split("\n");

        for (int i = 0; i < lines.Length; i++)
        {
            var line = lines[i].Trim();

            if (line.StartsWith("#EXT-X-STREAM-INF:"))
            {
                var attributeString = line.Substring("#EXT-X-STREAM-INF:".Length).Trim();

                var attributeMatches = Regex.Matches(attributeString, @"([A-Z\-]+)=(""([^""]+)""|([^"",]+))");

                var attributes = new Dictionary<string, string>();

                foreach (Match match in attributeMatches)
                {
                    var attributeName = match.Groups[1].Value;
                    var attributeValue = match.Groups[3].Success ? match.Groups[3].Value : match.Groups[4].Value;

                    attributes[attributeName] = attributeValue;
                }

                var streamUri = new Uri(new Uri(playlistUri), lines[++i].Trim());

                int.TryParse(GetValueByKey("AVERAGE-BANDWIDTH"), out int averageBandwidth);
                int.TryParse(GetValueByKey("BANDWIDTH"), out int bandwidth);
                int.TryParse(GetValueByKey("FRAME-RATE"), out int frameRate);

                var stream = new M3u8Stream
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

                string GetValueByKey(string key)
                {
                    return attributes.ContainsKey(key) ? attributes[key] : null;
                }
            }
            else if (line.StartsWith("#EXT-X-TARGETDURATION:"))
            {
                playlist.TargetDuration = int.Parse(line.Substring("#EXT-X-TARGETDURATION:".Length).Trim());
            }
            else if (line.StartsWith("#EXT-X-VERSION:"))
            {
                playlist.Version = int.Parse(line.Substring("#EXT-X-VERSION:".Length).Trim());
            }
            else if (line.StartsWith("#EXT-X-MEDIA-SEQUENCE:"))
            {
                playlist.MediaSequence = int.Parse(line.Substring("#EXT-X-MEDIA-SEQUENCE:".Length).Trim());
            }
            else if (line.StartsWith("#EXT-X-PLAYLIST-TYPE:"))
            {
                playlist.PlaylistType = line.Substring("#EXT-X-PLAYLIST-TYPE:".Length).Trim();
            }
            else if (line.StartsWith("#EXT-X-INDEPENDENT-SEGMENTS"))
            {
                playlist.IndependentSegments = true;
            }
        }

        var baseUri = new Uri(playlistUri);

        var test = GetM3U8Segment(lines, playlist.MediaSequence, baseUri);

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
        foreach (var stream in playlist.Streams)
        {
            stream.Segments = new List<M3u8Segment>();

            var streamUri = new Uri(baseUri, stream.Uri);
            var streamLines = (await httpClient.GetStringAsync(streamUri).ConfigureAwait(false)).Split("\n");

            stream.Segments = GetM3U8Segment(streamLines, playlist.MediaSequence, streamUri);

            // Sort segments based on BYTERANGE start position
            //stream.Segments.Sort((seg1, seg2) => seg1.ByterangeStart.CompareTo(seg2.ByterangeStart));
        }

        return playlist;
    }

    private new List<M3u8Segment> GetM3U8Segment(string[] streamLines, int mediaSequence, Uri streamUri)
    {
        var segments = new List<M3u8Segment>();

        M3u8Segment segment = null;

        var isPlayListFile = streamLines.Any(a=>a.Contains("#EXT-X-STREAM-INF"));

        for (int i = 0; i < streamLines.Length; i++)
        {
            var line = streamLines[i].Trim();
            if (string.IsNullOrEmpty(line))
            {
                continue;
            }
            if (line.StartsWith("#EXTINF:"))
            {
                segment = new M3u8Segment
                {
                    Duration = double.Parse(line.Substring("#EXTINF:".Length, line.IndexOf(",") - "#EXTINF:".Length)),
                    MediaSequence = mediaSequence++,
                };
            }
            else if (line.StartsWith("#EXT-X-BYTERANGE:"))
            {
                var byterangeParts = line.Substring("#EXT-X-BYTERANGE:".Length)
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
