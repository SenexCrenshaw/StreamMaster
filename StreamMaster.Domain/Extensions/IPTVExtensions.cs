
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
        //var a = body.Contains("#EXT-X-TARGETDURATION");
        //var b = body.Contains("#EXT-X-MEDIA-SEQUENCE");
        //var c = !body.Contains("EXTM3U");
        return body.Contains("#EXT-X-TARGETDURATION") || body.Contains("#EXT-X-MEDIA-SEQUENCE") || body.Contains("EXTM3U");
    }


    public static List<VideoStream>? ConvertToVideoStream2(Stream dataStream, int Id, string Name)
    {
        StringBuilder bodyBuilder = new();

        using (StreamReader reader = new(dataStream))
        {
            while (!reader.EndOfStream)
            {
                bodyBuilder.AppendLine(reader.ReadLine());
            }
        }

        string body = bodyBuilder.ToString();
        if (body.Contains("#EXT-X-TARGETDURATION") || body.Contains("#EXT-X-MEDIA-SEQUENCE") ||
            !body.Contains("EXTM3U"))
        {
            Console.WriteLine("Invalid M3U file, an extended M3U file is required.");
            return null;
        }

        ConcurrentDictionary<long, VideoStream> streamLists = new();

        string lastExtGrp = "";

        string[] extInfArray = body.Split("#EXTINF", StringSplitOptions.RemoveEmptyEntries);

        int index = -1;
        foreach (string? bodyline in extInfArray.Skip(1))
        {
            ++index;
            VideoStream? VideoStream = bodyline.StringToVideoStream();
            if (VideoStream == null)
            {
                continue;
            }

            MatchCollection extGrp = grpRegex().Matches(bodyline);
            if (extGrp.Count > 0)
            {
                lastExtGrp = extGrp[0].Groups[1].Value.Trim();
            }

            if (string.IsNullOrEmpty(VideoStream.Tvg_group))
            {
                //VideoStream.Tvg_group = lastExtGrp;
                VideoStream.Tvg_group = "(None)";
            }

            VideoStream.M3UFileId = Id;
            VideoStream.M3UFileName = Name;
            VideoStream.IsHidden = false;

            VideoStream.User_Tvg_logo = VideoStream.Tvg_logo;
            VideoStream.User_Tvg_name = VideoStream.Tvg_name;
            VideoStream.User_Tvg_ID = VideoStream.Tvg_ID;
            VideoStream.User_Tvg_chno = VideoStream.Tvg_chno;
            VideoStream.User_Tvg_group = VideoStream.Tvg_group;
            VideoStream.User_Url = VideoStream.Url;

            // Add the VideoStream to a list directly instead of using a ConcurrentDictionary
            streamLists.TryAdd(index, VideoStream);
        }

        List<VideoStream> results = streamLists.OrderBy(s => s.Key).Select(s => s.Value).ToList();

        return results;

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
        if (bodyline.Contains("https://tmsimg.fancybits.co/assets/s97051"))
        {
        }
        string[] lines = bodyline.Replace("\r\n", "\n").Split("\n");

        // Remove lines with # and blank lines
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
