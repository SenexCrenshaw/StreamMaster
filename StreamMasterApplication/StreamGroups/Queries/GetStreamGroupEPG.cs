using FluentValidation;

using Microsoft.AspNetCore.Http;

using StreamMaster.SchedulesDirectAPI.Domain.EPG;
using StreamMaster.SchedulesDirectAPI.Domain.Models;

using StreamMasterApplication.Common.Extensions;
using StreamMasterApplication.Programmes.Queries;

using System.Collections.Concurrent;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Serialization;

namespace StreamMasterApplication.StreamGroups.Queries;

[RequireAll]
public record GetStreamGroupEPG(int StreamGroupId) : IRequest<string>;

public class GetStreamGroupEPGValidator : AbstractValidator<GetStreamGroupEPG>
{
    public GetStreamGroupEPGValidator()
    {
        _ = RuleFor(v => v.StreamGroupId)
            .NotNull().GreaterThanOrEqualTo(0);
    }
}

public class GetStreamGroupEPGHandler(IHttpContextAccessor httpContextAccessor, ILogger<GetStreamGroupEPG> logger, IRepositoryWrapper repository, IMapper mapper, ISettingsService settingsService, IPublisher publisher, ISender sender, IHubContext<StreamMasterHub, IStreamMasterHub> hubContext, IMemoryCache memoryCache) : BaseMediatorRequestHandler(logger, repository, mapper, settingsService, publisher, sender, hubContext, memoryCache), IRequestHandler<GetStreamGroupEPG, string>
{
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

    private readonly ParallelOptions parallelOptions = new()
    {
        MaxDegreeOfParallelism = Environment.ProcessorCount
    };

    public string GetIconUrl(string iconOriginalSource, Setting setting)
    {
        string url = _httpContextAccessor.GetUrl();

        if (string.IsNullOrEmpty(iconOriginalSource))
        {
            return $"{url}{setting.DefaultIcon}";
        }

        string originalUrl = iconOriginalSource;

        if (iconOriginalSource.StartsWith('/'))
        {
            iconOriginalSource = iconOriginalSource[1..];
        }

        if (iconOriginalSource.StartsWith("images/"))
        {
            return $"{url}/{iconOriginalSource}";
        }
        else if (!iconOriginalSource.StartsWith("http"))
        {
            return GetApiUrl(SMFileTypes.TvLogo, originalUrl);
        }
        else if (setting.CacheIcons)
        {
            if (iconOriginalSource.StartsWith("https://json.schedulesdirect.org"))
            {
                return GetApiUrl(SMFileTypes.SDImage, originalUrl);
            }
            return GetApiUrl(SMFileTypes.Icon, originalUrl);
        }

        return iconOriginalSource;
    }

    [LogExecutionTimeAspect]
    public async Task<string> Handle(GetStreamGroupEPG request, CancellationToken cancellationToken)
    {
        List<VideoStreamDto> videoStreams = await Repository.StreamGroupVideoStream.GetStreamGroupVideoStreams(request.StreamGroupId, cancellationToken);

        if (!videoStreams.Any())
        {
            return "";
        }

        Tv epgData = await PrepareEpgData(videoStreams, cancellationToken);

        return SerializeEpgData(epgData);
    }

    [LogExecutionTimeAspect]
    private async Task<Tv> PrepareEpgData(IEnumerable<VideoStreamDto> videoStreams, CancellationToken cancellationToken)
    {
        HashSet<string> epgids = new(videoStreams.Where(a => !a.IsHidden).Select(r => r.User_Tvg_ID));

        List<EPGProgramme> cachedProgrammes = await Sender.Send(new GetProgrammesRequest(), cancellationToken).ConfigureAwait(false);
        Setting setting = await GetSettingsAsync();
        List<string> channels = [.. cachedProgrammes.ConvertAll(a => a.Channel).Distinct().Order()];
        IEnumerable<EPGProgramme> programmes = cachedProgrammes.Where(a =>
        a.StartDateTime > DateTime.Now.AddDays(-1) &&
        a.StartDateTime <= DateTime.Now.AddDays(setting.SDEPGDays) &&
                        a.Channel != null &&
                        (epgids.Contains(a.Channel) || epgids.Contains(a.DisplayName))).DeepCopy();

        List<IconFileDto> icons = [.. MemoryCache.Icons()];

        ConcurrentBag<TvChannel> retChannels = [];
        ConcurrentBag<EPGProgramme> retProgrammes = [];

        Parallel.ForEach(videoStreams, parallelOptions, videoStream =>
        {
            TvChannel? tvChannel = CreateTvChannel(videoStream, setting);
            if (tvChannel != null)
            {
                retChannels.Add(tvChannel);
            }

            foreach (EPGProgramme programme in ProcessProgrammesForVideoStream(videoStream, programmes, icons, setting))
            {
                retProgrammes.Add(programme);
            }
        });

        return new Tv
        {
            Channel = [.. retChannels.OrderBy(a => int.Parse(a.Id))],
            Programme = [.. retProgrammes.OrderBy(a => int.Parse(a.Channel)).ThenBy(a => a.StartDateTime)]
        };
    }

    private TvChannel? CreateTvChannel(VideoStreamDto? videoStream, Setting setting)
    {
        if (videoStream == null)
        {
            return null;
        }

        string? logo = GetIconUrl(videoStream.User_Tvg_logo, setting);

        // Check if it's a dummy stream
        bool isDummyStream = IsVideoStreamADummy(videoStream, setting);

        // Build the TvChannel based on whether it's a dummy or not
        if (isDummyStream)
        {
            return new TvChannel
            {
                Id = videoStream.User_Tvg_chno.ToString(),
                Icon = new TvIcon { Src = logo ?? string.Empty },
                Displayname =
            [
              videoStream.User_Tvg_name
            ]
            };
        }
        else
        {
            return new TvChannel
            {
                Id = videoStream.User_Tvg_chno.ToString(),
                Icon = new TvIcon { Src = logo ?? string.Empty },
                Displayname =
            [
               videoStream.User_Tvg_name
            ]
            };
        }
    }

    private List<EPGProgramme> ProcessProgrammesForVideoStream(VideoStreamDto videoStream, IEnumerable<EPGProgramme> cachedProgrammes, List<IconFileDto> cachedIcons, Setting setting)
    {
        if (videoStream.User_Tvg_ID == null)
        {
            // Decide what to do if User_Tvg_ID is null. Here, we're returning an empty list.
            return [];
        }

        //string userTvgIdLower = videoStream.User_Tvg_ID.ToLower();
        bool isDummyStream = IsVideoStreamADummy(videoStream, setting);
        if (isDummyStream)
        {
            return HandleDummyStream(videoStream);
        }
        else
        {
            return ProcessNonDummyStream(videoStream, cachedProgrammes, cachedIcons);
        }
    }

    private List<EPGProgramme> HandleDummyStream(VideoStreamDto videoStream)
    {
        List<EPGProgramme> programmesForStream = [];

        EPGProgramme prog = new()
        {
            Channel = videoStream.User_Tvg_chno.ToString(),
            Title = [
                new()
                {
                    Lang = "en",
                    Text = videoStream.User_Tvg_name
                }
            ],
            Desc = new TvDesc
            {
                Lang = "en",
                Text = videoStream.User_Tvg_name
            }
        };

        DateTime now = DateTime.Now;
        prog.Icon.Add(new TvIcon { Height = "10", Width = "10", Src = $"{_httpContextAccessor.GetUrl()}/images/transparent.png" });
        prog.Start = new DateTime(now.Year, now.Month, now.Day, 0, 0, 0).ToString("yyyyMMddHHmmss zzz").Replace(":", "");
        now = now.AddDays(7);
        prog.Stop = new DateTime(now.Year, now.Month, now.Day, 0, 0, 0).ToString("yyyyMMddHHmmss zzz").Replace(":", "");
        prog.New = null;
        prog.Previouslyshown = null;

        if (!string.IsNullOrEmpty(videoStream.TimeShift) && videoStream.TimeShift != "0000")
        {
            prog.Start = ReplaceTimezoneOffset(prog.Start, videoStream.TimeShift);
            prog.Stop = ReplaceTimezoneOffset(prog.Stop, videoStream.TimeShift);
        }

        programmesForStream.Add(prog);
        return programmesForStream;
    }

    private List<EPGProgramme> ProcessNonDummyStream(VideoStreamDto videoStream, IEnumerable<EPGProgramme> cachedProgrammes, List<IconFileDto> cachedIcons)
    {
        List<EPGProgramme> programmesForStream = [];

        foreach (EPGProgramme? aprog in cachedProgrammes.Where(p => p.Channel == videoStream.User_Tvg_ID))
        {
            EPGProgramme prog = aprog.DeepCopy();
            AdjustProgrammeIcons(prog, cachedIcons);

            prog.Channel = videoStream.User_Tvg_chno.ToString();
            //if (string.IsNullOrEmpty(prog.New))
            //{
            //    prog.New = null;
            //}

            //if (string.IsNullOrEmpty(prog.Live))
            //{
            //    prog.Live = null;
            //}

            //if (string.IsNullOrEmpty(prog.Premiere))
            //{
            //    prog.Premiere = null;
            //}

            //if (prog.Previouslyshown == null || string.IsNullOrEmpty(prog.Previouslyshown.Start))
            //{
            //    prog.Previouslyshown = null;
            //}

            if (!string.IsNullOrEmpty(videoStream.TimeShift) && videoStream.TimeShift != "0000")
            {
                prog.Start = ReplaceTimezoneOffset(prog.Start, videoStream.TimeShift);
                prog.Stop = ReplaceTimezoneOffset(prog.Stop, videoStream.TimeShift);
            }

            programmesForStream.Add(prog);
        }

        return programmesForStream;
    }

    private static string ReplaceTimezoneOffset(string input, string newOffset)
    {
        // The new offset should be exactly 4 digits
        if (newOffset.Length != 4 || !Regex.IsMatch(newOffset, @"^\d{4}$"))
        {
            throw new ArgumentException("The new offset should be exactly 4 digits", nameof(newOffset));
        }

        // Use regex to replace the offset after the '+'
        return Regex.Replace(input, @"\+\d{4}", $"+{newOffset}");
    }

    private void AdjustProgrammeIcons(EPGProgramme prog, List<IconFileDto> cachedIcons)
    {
        if (!prog.Icon.Any())
        {
            prog.Icon.Add(new TvIcon { Height = "", Width = "", Src = "" });
        }
        else
        {
            foreach (TvIcon icon in prog.Icon)
            {
                if (!string.IsNullOrEmpty(icon.Src))
                {
                    if (icon.Src.StartsWith("https://json.schedulesdirect.org"))
                    {
                        ImageInfo? programmeIcon = MemoryCache.ImageInfos().Find(a => a.RealUrl == icon.Src);
                        if (programmeIcon != null)
                        {
                            icon.Src = GetApiUrl(SMFileTypes.SDImage, programmeIcon.IconUri);
                        }
                    }
                    else
                    {
                        IconFileDto? programmeIcon = cachedIcons.Find(a => a.SMFileType == SMFileTypes.ProgrammeIcon && a.Source == icon.Src);
                        if (programmeIcon != null)
                        {
                            icon.Src = GetApiUrl(SMFileTypes.ProgrammeIcon, programmeIcon.Source);
                        }
                    }
                }
            }
        }
    }

    [LogExecutionTimeAspect]
    private static string SerializeEpgData(Tv epgData)
    {
        XmlSerializerNamespaces ns = new();
        ns.Add("", "");

        XmlWriterSettings settings = new()
        {
            //settings.NewLineHandling = NewLineHandling.Entitize;
            Indent = true,
            OmitXmlDeclaration = true,
            NewLineHandling = NewLineHandling.Entitize,
            NewLineChars = "\n"
        };

        //using Utf8StringWriter textWriter = new();
        UTF8Encoding utf8NoBOM = new(false);

        //XmlSerializer serializer = new(typeof(Tv));
        string xmlText = "";

        using (MemoryStream stream = new())
        {
            using XmlWriter writer = XmlWriter.Create(stream, settings); // Create XmlWriter with settings
            XmlSerializer xml = new(typeof(Tv));

            xml.Serialize(writer, epgData, ns);
            xmlText = utf8NoBOM.GetString(stream.ToArray());
        }
        return xmlText;
    }

    private string GetApiUrl(SMFileTypes path, string source)
    {
        string url = _httpContextAccessor.GetUrl();
        return $"{url}/api/files/{(int)path}/{WebUtility.UrlEncode(source)}";
    }

    private static bool IsVideoStreamADummy(VideoStreamDto videoStream, Setting setting)
    {
        if (videoStream.User_Tvg_ID?.ToLower() == "dummy")
        {
            return true;
        }

        if (string.IsNullOrEmpty(videoStream.User_Tvg_ID))
        {
            return true;
        }

        return !string.IsNullOrEmpty(setting.DummyRegex) &&
               new Regex(setting.DummyRegex, RegexOptions.ECMAScript | RegexOptions.IgnoreCase).IsMatch(videoStream.User_Tvg_ID);
    }
}