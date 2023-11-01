using StreamMasterDomain.Models;

using System.Collections.Concurrent;
using System.Text;
using System.Text.RegularExpressions;

namespace StreamMasterDomain.Common;

public static partial class IPTVExtensions
{
    public static List<VideoStream>? ConvertToVideoStream(Stream dataStream, int Id, string Name)
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

        _ = Parallel.ForEach(extInfArray.Skip(1), (bodyline, state, index) =>
        {
            VideoStream? VideoStream = bodyline.StringToVideoStream();
            if (VideoStream == null)
            {
                return;
            }

            MatchCollection extGrp = grpRegex().Matches(bodyline); if
            (extGrp.Count > 0) { lastExtGrp = extGrp[0].Groups[1].Value.Trim(); }

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


            streamLists.TryAdd(index, VideoStream);
        });

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
                            VideoStream.Tvg_logo = parameter[1].Trim();
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
