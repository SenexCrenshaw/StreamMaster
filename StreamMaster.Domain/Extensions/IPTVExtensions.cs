
using Microsoft.Extensions.Logging;

using StreamMaster.Domain.Logging;
using StreamMaster.Domain.Models;

using System.Collections.Concurrent;
using System.Text;
using System.Text.RegularExpressions;

namespace StreamMaster.Domain.Common;

public static partial class IPTVExtensions
{
    [LogExecutionTimeAspect]
    public static async Task<List<VideoStream>?> ConvertToVideoStreamAsync(Stream dataStream, int Id, string Name, List<string> vodExclusion, ILogger logger, CancellationToken cancellationToken)
    {

        BlockingCollection<KeyValuePair<int, VideoStream>> blockingCollection = new(new ConcurrentQueue<KeyValuePair<int, VideoStream>>());
        int segmentNumber = 0;
        int processedCount = 0;
        object lockObj = new();
        StringBuilder segmentBuilder = new();

        using StreamReader reader = new(dataStream);
        await Task.Run(() =>
        {
            try
            {
                while (!reader.EndOfStream)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    string? line = reader.ReadLine();

                    if (string.IsNullOrWhiteSpace(line))
                    {
                        continue;
                    }

                    if (line.StartsWith("#EXTINF"))
                    {
                        if (segmentBuilder.Length > 0)
                        {
                            ProcessSegment(segmentNumber++, segmentBuilder.ToString());
                            segmentBuilder.Clear();
                        }
                    }
                    else
                    {
                        if (segmentBuilder.Length == 0)
                        {
                            continue;
                        }
                    }

                    segmentBuilder.AppendLine(line);
                }

                // Process the last segment
                if (segmentBuilder.Length > 0)
                {
                    ProcessSegment(segmentNumber, segmentBuilder.ToString());
                }
            }
            finally
            {
                blockingCollection.CompleteAdding();
            }
        }, cancellationToken);

        List<VideoStream> results = blockingCollection.OrderBy(kvp => kvp.Key).Select(kvp => kvp.Value).ToList();
        logger.LogInformation($"Imported {processedCount} streams.");
        return results;

        void ProcessSegment(int segmentNum, string segment)
        {
            VideoStream? videoStream = segment.StringToVideoStream();

            if (videoStream != null)
            {
                if (vodExclusion.Count > 0)
                {
                    if (CheckExcluded(videoStream.Url))
                    {
                        return;
                    }
                }

                UpdateVideoStreamProperties(videoStream, Id, Name);

                blockingCollection.Add(new KeyValuePair<int, VideoStream>(segmentNum, videoStream), cancellationToken);

                lock (lockObj)
                {
                    processedCount++;
                    if (processedCount % 5000 == 0)
                    {
                        logger.LogInformation($"Importing {processedCount} streams.");
                    }
                }
            }
        }

        bool CheckExcluded(string URL)
        {
            foreach (string vodPattern in vodExclusion)
            {
                string regexPattern;

                // Check if vodPattern is a simple substring (no special regex characters)
                if (vodPattern.Any(ch => char.IsPunctuation(ch) && ch != '_'))
                {
                    // vodPattern contains special characters, so use it as a regex pattern
                    regexPattern = vodPattern;
                }
                else
                {
                    // vodPattern is a simple substring, so create a regex pattern for 'string.Contains' behavior
                    regexPattern = ".*" + Regex.Escape(vodPattern) + ".*";
                }

                Regex regex = new(regexPattern, RegexOptions.IgnoreCase);

                if (regex.IsMatch(URL))
                {
                    return true;
                }
            }
            return false;
        }

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

        if (lines.Length < 2 || !lines[0].StartsWith("#"))
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
                VideoStream.Tvg_group = line[8..].Trim(); // Extracting EXTGRP value
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
                            //VideoStream.Id = parameter[1].Trim();
                            break;

                        case "tvg-chno":
                            string num = parameter[1].Trim();
                            VideoStream.Tvg_chno = int.TryParse(num, out int chno) ? chno : 0;

                            break;

                        case "channel-id":
                            //VideoStream.Id = parameter[1].Trim();
                            break;

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

                        case "tvc-guide-stationid":
                            string StationIdnum = parameter[1].Trim();
                            VideoStream.StationId = StationIdnum;// int.TryParse(StationIdnum, out int stationId) ? chanchno : 0;
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
