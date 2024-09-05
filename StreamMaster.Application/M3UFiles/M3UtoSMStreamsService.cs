using System.Text;
using System.Text.RegularExpressions;

namespace StreamMaster.Application.M3UFiles;

public partial class M3UToSMStreamsService(ILogger<M3UToSMStreamsService> logger, IFileUtilService fileUtilService) : IM3UToSMStreamsService
{
    public async IAsyncEnumerable<SMStream> GetSMStreamsFromM3U(M3UFile m3UFile)
    {
        logger.LogInformation("Reading m3uFile {Name} and ignoring URLs with {VODS}", m3UFile.Name, string.Join(',', m3UFile.VODTags));
        await using Stream dataStream = fileUtilService.GetFileDataStream(Path.Combine(FileDefinitions.M3U.DirectoryLocation, m3UFile.Source));

        int segmentNumber = 0;
        StringBuilder segmentBuilder = new();

        using StreamReader reader = new(dataStream);
        string? clientUserAgent = null;

        while (!reader.EndOfStream)
        {
            string? line = await reader.ReadLineAsync();

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

                    SMStream? smStream = ProcessSegment(segmentNumber++, segmentBuilder.ToString(), m3UFile, logger);
                    if (smStream != null)
                    {
                        yield return smStream;
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
            SMStream? smStream = ProcessSegment(segmentNumber, segmentBuilder.ToString(), m3UFile, logger);
            if (smStream != null)
            {
                yield return smStream;
            }
        }
    }

    private static SMStream? ProcessSegment(int segmentNum, string segment, M3UFile m3UFile, ILogger logger)
    {
        int? chNo = null;
        if (m3UFile.AutoSetChannelNumbers)
        {
            chNo = segmentNum + m3UFile.StartingChannelNumber;
        }
        SMStream? smStream = StringToSMStream(segment, chNo);
        if (smStream == null)
        {
            logger.LogWarning("Could not create stream from segment {segmentNum}", segmentNum);
            return null;
        }

        if (m3UFile.VODTags.Count > 0 && CheckExcluded(smStream.Url, m3UFile.VODTags))
        {
            return null;
        }

        UpdateSMStreamProperties(smStream, m3UFile);

        return smStream;
    }

    public static SMStream? StringToSMStream(string bodyline, int? channelNumber)
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
            SMStream.Id = FileUtil.EncodeToMD5(SMStream.Url);
        }

        if (channelNumber.HasValue)
        {
            SMStream.ChannelNumber = channelNumber.Value;
        }

        return SMStream;
    }

    [GeneratedRegex(",([^\\n]*|,[^\\r]*)", RegexOptions.IgnoreCase | RegexOptions.Compiled, "en-US")]
    private static partial Regex nameRegex();

    [GeneratedRegex("[a-z-A-Z=]*(\".*?\")", RegexOptions.IgnoreCase | RegexOptions.Compiled, "en-US")]
    private static partial Regex paramRegex();

    private static bool CheckExcluded(string url, List<string> vodExclusion)
    {
        foreach (string vodPattern in vodExclusion)
        {
            string regexPattern = vodPattern.Any(ch => char.IsPunctuation(ch) && ch != '_')
                ? vodPattern
                : ".*" + Regex.Escape(vodPattern) + ".*";

            Regex regex = new(regexPattern, RegexOptions.IgnoreCase);

            if (regex.IsMatch(url))
            {
                return true;
            }
        }
        return false;
    }

    private static void UpdateSMStreamProperties(SMStream smStream, M3UFile m3UFile)
    {
        smStream.M3UFileId = m3UFile.Id;
        smStream.M3UFileName = m3UFile.Name;
        smStream.IsHidden = false;
    }
}
