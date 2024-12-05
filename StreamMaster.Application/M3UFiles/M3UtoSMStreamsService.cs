using System.Text;
using System.Text.RegularExpressions;

namespace StreamMaster.Application.M3UFiles;

public partial class M3UToSMStreamsService(ILogger<M3UToSMStreamsService> logger, IFileUtilService fileUtilService) : IM3UToSMStreamsService
{
    public async IAsyncEnumerable<SMStream?> GetSMStreamsFromM3U(M3UFile m3UFile)
    {
        if (m3UFile.VODTags.Count != 0)
        {
            logger.LogInformation("Reading m3uFile {Name} and ignoring URLs with {VODS}", m3UFile.Name, string.Join(',', m3UFile.VODTags));
        }
        else
        {
            logger.LogInformation("Reading m3uFile {Name}", m3UFile.Name);
        }

        string? filepath = fileUtilService.GetFilePath(Path.Combine(FileDefinitions.M3U.DirectoryLocation, m3UFile.Source));

        if (filepath == null)
        {
            yield return null;
            yield break;  // Early exit if stream is null
        }

        await using Stream? dataStream = await fileUtilService.GetFileDataStream(filepath);

        if (dataStream == null)
        {
            yield return null;
            yield break;  // Early exit if stream is null
        }

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
                // Process the previous segment
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

            // Add the current line to the segment being built
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
            string truncatedSegment = segment.Length > 40 ? segment[..40] + "..." : segment;

            logger.LogWarning("Could not create stream from m3u line # {segmentNum} {segment}", segmentNum, truncatedSegment);
            return null;
        }

        string? id = GenerateM3UKeyValue(m3UFile.M3UKey, smStream);
        if (string.IsNullOrEmpty(id))
        {
            return null;
        }
        smStream.Id = id;

        if (m3UFile.VODTags.Count > 0 && CheckExcluded(smStream.Url, m3UFile.VODTags))
        {
            return null;
        }

        UpdateSMStreamProperties(smStream, m3UFile);

        return smStream;
    }

    private static string? GenerateM3UKeyValue(M3UKey m3uKey, SMStream smStream)
    {
        string? key = m3uKey switch
        {
            M3UKey.URL => smStream.Url,
            M3UKey.CUID => smStream.CUID,
            M3UKey.ChannelId => smStream.ChannelId,
            M3UKey.TvgID => smStream?.EPGID,
            M3UKey.TvgName => smStream.TVGName,
            M3UKey.Name => smStream.Name,
            M3UKey.TvgName_TvgID =>
            (!string.IsNullOrEmpty(smStream?.TVGName) && !string.IsNullOrEmpty(smStream?.EPGID))
                ? $"{smStream.TVGName}_{smStream.EPGID}"
                : null,
            M3UKey.Name_TvgID =>
        (!string.IsNullOrEmpty(smStream?.Name) && !string.IsNullOrEmpty(smStream?.EPGID))
            ? $"{smStream.Name}_{smStream.EPGID}"
            : null,
            _ => throw new ArgumentOutOfRangeException(nameof(m3uKey), m3uKey, null),
        };
        return string.IsNullOrEmpty(key) ? null : FileUtil.EncodeToMD5(key);
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
                                SMStream.TVGName = parameter[1].Trim();
                            }
                            break;

                        case "channel-name":
                            if (!string.IsNullOrEmpty(parameter[1].Trim()))
                            {
                                SMStream.ChannelName = parameter[1].Trim();
                            }
                            break;

                        case "cuid":
                            if (!string.IsNullOrEmpty(parameter[1].Trim()))
                            {
                                SMStream.CUID = parameter[1].Trim();
                            }
                            break;

                        case "channel-id":
                            if (!string.IsNullOrEmpty(parameter[1].Trim()))
                            {
                                SMStream.ChannelId = parameter[1].Trim();
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