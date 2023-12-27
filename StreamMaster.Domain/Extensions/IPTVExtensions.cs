
using Microsoft.Extensions.Logging;

using StreamMaster.Domain.Logging;
using StreamMaster.Domain.Models;

using System.Collections.Concurrent;
using System.Text.RegularExpressions;

namespace StreamMaster.Domain.Common;

public static partial class IPTVExtensions
{
    [LogExecutionTimeAspect]
    public static async Task<List<VideoStream>?> ConvertToVideoStreamAsync(Stream dataStream, int Id, string Name, ILogger logger, CancellationToken cancellationToken)
    {
        using var reader = new StreamReader(dataStream);
        string body = await reader.ReadToEndAsync(cancellationToken).ConfigureAwait(false);

        if (!IsValidM3UFile(body))
        {
            logger.LogWarning("Invalid M3U file, an extended M3U file is required.");
            return null;
        }

        var lines = body.Split("#EXTINF", StringSplitOptions.RemoveEmptyEntries).ToList();
        lines.RemoveAt(0);

        var totalExpectedCount = lines.Count;
        var blockingCollection = new BlockingCollection<KeyValuePair<int, VideoStream>>(new ConcurrentQueue<KeyValuePair<int, VideoStream>>());

        int processedCount = 0;
        object lockObj = new object();

        Parallel.ForEach(Partitioner.Create(0, totalExpectedCount), new ParallelOptions { CancellationToken = cancellationToken }, range =>
        {
            for (int i = range.Item1; i < range.Item2; i++)
            {
                cancellationToken.ThrowIfCancellationRequested();

                string bodyline = lines[i];
                var videoStream = bodyline.StringToVideoStream();

                if (videoStream != null)
                {
                    UpdateVideoStreamProperties(videoStream, Id, Name);
                    blockingCollection.Add(new KeyValuePair<int, VideoStream>(i, videoStream));

                    lock (lockObj)
                    {
                        processedCount++;
                        if (processedCount % 5000 == 0)
                        {
                            logger.LogInformation($"Importing {processedCount}/{totalExpectedCount} streams.");
                        }
                    }
                }
            }
        });

        blockingCollection.CompleteAdding();

        List<VideoStream> results = blockingCollection.OrderBy(kvp => kvp.Key).Select(kvp => kvp.Value).ToList();
        logger.LogInformation($"Imported {processedCount} streams.");
        return results;
    }


    private static void UpdateVideoStreamProperties(VideoStream videoStream, int m3uFileId, string m3uFileName)
    {
        // Set the M3U file-related properties
        videoStream.M3UFileId = m3uFileId;
        videoStream.M3UFileName = m3uFileName;
        videoStream.IsHidden = false;

        // Set user-defined properties
        videoStream.User_Tvg_logo = videoStream.Tvg_logo;
        videoStream.User_Tvg_name = videoStream.Tvg_name;
        videoStream.User_Tvg_ID = videoStream.Tvg_ID;
        videoStream.User_Tvg_chno = videoStream.Tvg_chno;
        videoStream.User_Tvg_group = videoStream.Tvg_group;
        videoStream.User_Url = videoStream.Url;
    }


    private static bool IsValidM3UFile(string body)
    {
        return !(body.Contains("#EXT-X-TARGETDURATION") || body.Contains("#EXT-X-MEDIA-SEQUENCE")) && body.Contains("EXTM3U");
    }

    public static (string fullName, string name) GetRandomFileName(this FileDefinition fd)
    {
        if (!Directory.Exists(fd.DirectoryLocation))
        {
            return ("", "");
        }

        DirectoryInfo dir = new(fd.DirectoryLocation);
        return dir.GetRandomFileName(fd.FileExtension);
    }

    public static VideoStream? StringToVideoStream(this string bodyline)
    {

        VideoStream VideoStream = new();

        string[] lines = bodyline.Replace("\r\n", "\n").Split("\n");

        if (lines.Length < 2 || lines[0].StartsWith("#"))
        {
            return null;
        }

        string value = "";
        string newline = "";
        string potentialname = "";

        foreach (string line in lines)
        {
            if (string.IsNullOrWhiteSpace(line.Trim()))
            {
                continue;
            }

            if (Uri.IsWellFormedUriString(line, UriKind.Absolute))
            {
                VideoStream.Url = line;
                continue;
            }

            if (line.StartsWith("#EXTGRP:"))
            {
                VideoStream.Tvg_group = line.Substring(8).Trim(); // Extracting EXTGRP value
                continue;
            }

            newline = line;

            foreach (Match param in paramRegex().Matches(line).Cast<Match>())
            {
                newline = newline.Replace(param.Value, "");
                if (newline.Contains(','))
                {
                    potentialname = newline;
                }
                string[] parameter = param.Value.Replace("\"", "").Split('=');

                if (parameter.Length >= 2)
                {
                    switch (parameter[0].ToLower())
                    {
                        case "tvg-name":
                            VideoStream.Tvg_name = parameter[1].Trim();
                            break;

                        case "cuid":
                            VideoStream.Id = parameter[1].Trim();
                            break;

                        case "tvg-chno":
                            string num = parameter[1].Trim();
                            VideoStream.Tvg_chno = int.TryParse(num, out int chno) ? chno : 0;

                            break;

                        //case "channel-id":
                        //    VideoStream.Id = parameter[1].Trim();
                        //    break;

                        case "channel-number":
                            string channum = parameter[1].Trim();
                            VideoStream.Tvg_chno = int.TryParse(channum, out int chanchno) ? chanchno : 0;

                            break;

                        //case "tvg-shift":
                        //    VideoStream.Tvg_shift = parameter[1].Trim();
                        //    break;

                        case "tvg-id":
                            VideoStream.Tvg_ID = parameter[1].Trim();
                            break;

                        case "tvg-group":
                            VideoStream.Tvg_group = parameter[1].Trim();
                            break;

                        case "tvg-logo":
                            VideoStream.Tvg_logo = parameter.Skip(1).Aggregate((current, next) => current + next).Trim();
                            break;

                        case "group-title":
                            VideoStream.Tvg_group = parameter[1].Trim();
                            break;

                        default:
                            Console.WriteLine($"Parsing VideoStream: no match for parameter {parameter[0]} with setting {parameter[1].Trim()}");
                            break;
                    }

                    if (parameter[1].Length > 0 && !parameter[1].Contains("://"))
                    {
                        value += parameter[1] + " ";
                    }
                }
            }
        }

        if (string.IsNullOrEmpty(VideoStream.Tvg_name))
        {
            MatchCollection nameMatch = nameRegex().Matches(newline);
            if (nameMatch.Count > 0 && nameMatch[0].Groups.Count > 1)
            {
                VideoStream.Tvg_name = nameMatch[0].Groups[1].Value.Trim();
                VideoStream.Tvg_name = VideoStream.Tvg_name.Replace(",", "");
            }
            else
            {
                nameMatch = nameRegex().Matches(potentialname);
                if (nameMatch.Count > 0 && nameMatch[0].Groups.Count > 1)
                {
                    VideoStream.Tvg_name = nameMatch[0].Groups[1].Value.Trim();
                    VideoStream.Tvg_name = VideoStream.Tvg_name.Replace(",", "");
                }
            }

            if (string.IsNullOrEmpty(VideoStream.Tvg_name))
            {
                return null;
            }
        }

        if (string.IsNullOrEmpty(VideoStream.Id))
        {
            VideoStream.Id = VideoStream.Url.ConvertStringToId();
        }

        return VideoStream;
    }

    [GeneratedRegex("#EXTGRP: *(.*)", RegexOptions.IgnoreCase | RegexOptions.Compiled, "en-US")]
    private static partial Regex grpRegex();

    [GeneratedRegex(",([^\\n]*|,[^\\r]*)", RegexOptions.IgnoreCase | RegexOptions.Compiled, "en-US")]
    private static partial Regex nameRegex();

    [GeneratedRegex("[a-z-A-Z=]*(\".*?\")", RegexOptions.IgnoreCase | RegexOptions.Compiled, "en-US")]
    private static partial Regex paramRegex();
}
