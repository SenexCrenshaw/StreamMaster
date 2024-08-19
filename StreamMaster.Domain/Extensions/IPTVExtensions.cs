using StreamMaster.Domain.Logging;

using System.Collections.Concurrent;
using System.Text;
using System.Text.RegularExpressions;

namespace StreamMaster.Domain.Common;

public static partial class IPTVExtensions
{
    [LogExecutionTimeAspect]
    public static async Task<List<SMStream>?> ConvertToSMStreamAsync(Stream dataStream, int Id, string Name, List<string> vodExclusion, ILogger logger)
    {
        BlockingCollection<KeyValuePair<int, SMStream>> blockingCollection = new(new ConcurrentQueue<KeyValuePair<int, SMStream>>());
        int segmentNumber = 0;
        int processedCount = 0;
        object lockObj = new();
        StringBuilder segmentBuilder = new();

        using StreamReader reader = new(dataStream);
        await Task.Run(() =>
        {
            try
            {
                string? clientUserAgent = null;
                while (!reader.EndOfStream)
                {
                    string? line = reader.ReadLine();

                    if (string.IsNullOrWhiteSpace(line))
                    {
                        continue;
                    }

                    if (line.StartsWith("#EXTINF"))
                    {
                        if (segmentBuilder.Length > 0)
                        {
                            if (clientUserAgent != null)
                            {
                                int commadIndex = segmentBuilder.ToString().LastIndexOf(',');

                                segmentBuilder.Insert(commadIndex, $" clientUserAgent=\"{clientUserAgent}\" ");
                            }
                            if (!ProcessSegment(segmentNumber++, segmentBuilder.ToString()))
                            {
                                logger.LogWarning("Could not create stream from: {line}", line);
                            }
                            segmentBuilder.Clear();
                            clientUserAgent = null;
                        }
                    }
                    else if (line.StartsWith("#EXTVLCOPT"))
                    {
                        if (line.StartsWith("#EXTVLCOPT:http-user-agent"))
                        {
                            clientUserAgent = line.Replace("#EXTVLCOPT:http-user-agent=", "");
                            //segmentBuilder.Insert(1, $" clientUserAgent=\"{clientUserAgent}\" ");
                            continue;
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
        });

        List<SMStream> results = blockingCollection.OrderBy(kvp => kvp.Key).Select(kvp => kvp.Value).ToList();
        logger.LogInformation("Imported {processedCount} streams.", processedCount);
        return results;

        bool ProcessSegment(int segmentNum, string segment)
        {
            SMStream? smStream = segment.StringToSMStream();
            if (smStream == null)
            {
                return false;
            }

            if (vodExclusion.Count > 0)
            {
                if (CheckExcluded(smStream.Url))
                {
                    return false;
                }
            }

            UpdateSMStreamProperties(smStream, Id, Name);

            blockingCollection.Add(new KeyValuePair<int, SMStream>(segmentNum, smStream));

            lock (lockObj)
            {
                processedCount++;
                if (processedCount % 5000 == 0)
                {
                    logger.LogInformation("Importing {processedCount} streams.", processedCount);
                }
            }
            return true;
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

    private static void UpdateSMStreamProperties(SMStream smStream, int m3uFileId, string m3uFileName)
    {
        // Set the M3U file-related properties
        smStream.M3UFileId = m3uFileId;
        smStream.M3UFileName = m3uFileName;
        smStream.IsHidden = false;
    }

    //private static bool IsValidM3UFile(string body)
    //{
    //    return !(body.Contains("#EXT-X-TARGETDURATION") || body.Contains("#EXT-X-MEDIA-SEQUENCE")) && body.Contains("EXTM3U");
    //}

    public static (string fullName, string name) GetRandomFileName(this FileDefinition fd)
    {
        if (!Directory.Exists(fd.DirectoryLocation))
        {
            return ("", "");
        }

        DirectoryInfo dir = new(fd.DirectoryLocation);
        return dir.GetRandomFileName(fd.FileExtension);
    }

    public static SMStream? StringToSMStream(this string bodyline)
    {
        SMStream SMStream = new();

        string[] lines = bodyline.Replace("\r\n", "\n").Split("\n");

        if (lines.Length < 2 || !lines[0].StartsWith('#'))
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
                SMStream.Url = line;

                continue;
            }

            if (line.StartsWith("#EXTGRP:"))
            {
                SMStream.Group = line[8..].Trim(); // Extracting EXTGRP value
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
                            if (!string.IsNullOrEmpty(parameter[1].Trim()))
                            {
                                SMStream.Name = parameter[1].Trim();
                            }
                            break;

                        case "cuid":
                            if (!string.IsNullOrEmpty(parameter[1].Trim()))
                            {
                                SMStream.CUID = parameter[1].Trim();
                            }
                            break;

                        case "tvg-chno":
                            if (!string.IsNullOrEmpty(parameter[1].Trim()))
                            {
                                string num = parameter[1].Trim();
                                SMStream.ChannelNumber = int.TryParse(num, out int chno) ? chno : 0;
                            }
                            break;

                        case "clientuseragent":
                            if (!string.IsNullOrEmpty(parameter[1].Trim()))
                            {
                                SMStream.ClientUserAgent = parameter[1].Trim();
                            }
                            break;

                        //case "channel-id":
                        //    if (!string.IsNullOrEmpty(parameter[1].Trim()))
                        //    {
                        //        SMStream.Id = parameter[1].Trim();
                        //    }
                        //    break;

                        case "channel-number":
                            if (!string.IsNullOrEmpty(parameter[1].Trim()))
                            {
                                string channum = parameter[1].Trim();
                                SMStream.ChannelNumber = int.TryParse(channum, out int chanchno) ? chanchno : 0;
                            }
                            break;

                        //case "tvg-shift":
                        //    VideoStream.Tvg_shift = parameter[1].Trim();
                        //    break;

                        case "tvg-id":
                            if (!string.IsNullOrEmpty(parameter[1].Trim()))
                            {
                                SMStream.EPGID = parameter[1].Trim();
                            }
                            break;

                        case "tvg-group":
                            if (!string.IsNullOrEmpty(parameter[1].Trim()))
                            {
                                SMStream.Group = parameter[1].Trim();
                            }

                            break;

                        case "tvg-logo":

                            SMStream.Logo = parameter.Skip(1).Aggregate((current, next) => current + next).Trim();

                            break;

                        case "tvc-guide-stationid":
                            if (!string.IsNullOrEmpty(parameter[1].Trim()))
                            {
                                string StationIdnum = parameter[1].Trim();
                                SMStream.StationId = StationIdnum;
                            }
                            break;

                        case "group-title":
                            if (!string.IsNullOrEmpty(parameter[1].Trim()))
                            {
                                SMStream.Group = parameter[1].Trim();
                            }
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

        if (string.IsNullOrEmpty(SMStream.Name))
        {
            MatchCollection nameMatch = nameRegex().Matches(newline);
            if (nameMatch.Count > 0 && nameMatch[0].Groups.Count > 1)
            {
                SMStream.Name = nameMatch[0].Groups[1].Value.Trim();
                SMStream.Name = SMStream.Name.Replace(",", "");
            }
            else
            {
                nameMatch = nameRegex().Matches(potentialname);
                if (nameMatch.Count > 0 && nameMatch[0].Groups.Count > 1)
                {
                    SMStream.Name = nameMatch[0].Groups[1].Value.Trim();
                    SMStream.Name = SMStream.Name.Replace(",", "");
                }
            }

            if (string.IsNullOrEmpty(SMStream.Name))
            {
                return null;
            }
        }

        if (string.IsNullOrEmpty(SMStream.Id))
        {
            if (string.IsNullOrEmpty(SMStream.Url))
            {
                return null;
            }
            SMStream.Id = FileUtil.EncodeToBase64(SMStream.Url);
        }

        return SMStream;
    }

    [GeneratedRegex(",([^\\n]*|,[^\\r]*)", RegexOptions.IgnoreCase | RegexOptions.Compiled, "en-US")]
    private static partial Regex nameRegex();

    [GeneratedRegex("[a-z-A-Z=]*(\".*?\")", RegexOptions.IgnoreCase | RegexOptions.Compiled, "en-US")]
    private static partial Regex paramRegex();
}
