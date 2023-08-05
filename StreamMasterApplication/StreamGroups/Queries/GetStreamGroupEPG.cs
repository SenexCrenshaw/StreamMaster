using AutoMapper;
using AutoMapper.QueryableExtensions;

using FluentValidation;

using MediatR;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

using StreamMasterApplication.Common.Extensions;
using StreamMasterApplication.M3UFiles.Commands;

using StreamMasterDomain.Attributes;
using StreamMasterDomain.Dto;
using StreamMasterDomain.Repository.EPG;

using System.Collections.Concurrent;
using System.Net;
using System.Text.RegularExpressions;
using System.Xml.Serialization;

using static StreamMasterDomain.Common.GetStreamGroupEPGHandler;

namespace StreamMasterApplication.StreamGroups.Queries;

[RequireAll]
public record GetStreamGroupEPG(int StreamGroupNumber) : IRequest<string>;

public class GetStreamGroupEPGValidator : AbstractValidator<GetStreamGroupEPG>
{
    public GetStreamGroupEPGValidator()
    {
        _ = RuleFor(v => v.StreamGroupNumber)
            .NotNull().GreaterThanOrEqualTo(0);
    }
}

public partial class GetStreamGroupEPGHandler : BaseMemoryRequestHandler, IRequestHandler<GetStreamGroupEPG, string>
{
    protected Setting _setting = FileUtil.GetSetting();

    private readonly IHttpContextAccessor _httpContextAccessor;

    private readonly object Lock = new();
    private int dummyCount = 0;

    public GetStreamGroupEPGHandler(IHttpContextAccessor httpContextAccessor, ILogger<DeleteM3UFileHandler> logger, IRepositoryWrapper repository, IMapper mapper, IPublisher publisher, ISender sender, IMemoryCache memoryCache)
        : base(logger, repository, mapper, publisher, sender, memoryCache)
    {
        _httpContextAccessor = httpContextAccessor;
    }


    public string GetIconUrl(string iconOriginalSource)
    {
        string url = _httpContextAccessor.GetUrl();

        if (string.IsNullOrEmpty(iconOriginalSource))
        {
            iconOriginalSource = $"{url}{_setting.DefaultIcon}";
            return iconOriginalSource;
        }

        string originalUrl = iconOriginalSource;

        if (iconOriginalSource.StartsWith('/'))
        {
            iconOriginalSource = iconOriginalSource[1..];
        }

        if (iconOriginalSource.StartsWith("images/"))
        {
            iconOriginalSource = $"{url}/{iconOriginalSource}";
        }
        else if (!iconOriginalSource.StartsWith("http"))
        {
            iconOriginalSource = GetApiUrl(SMFileTypes.TvLogo, originalUrl);
        }
        else if (_setting.CacheIcons)
        {
            iconOriginalSource = GetApiUrl(SMFileTypes.Icon, originalUrl);
        }

        return iconOriginalSource;
    }

    public async Task<string> Handle(GetStreamGroupEPG request, CancellationToken cancellationToken)
    {
        IEnumerable<VideoStreamDto> videoStreams;
        string url = _httpContextAccessor.GetUrl();
        if (request.StreamGroupNumber > 0)
        {
            StreamGroupDto? streamGroup = await Repository.StreamGroup.GetStreamGroupDtoByStreamGroupNumber(request.StreamGroupNumber, url, cancellationToken).ConfigureAwait(false);
            if (streamGroup == null)
            {
                return "";
            }
            videoStreams = streamGroup.ChildVideoStreams.Where(a => !a.IsHidden);
        }
        else
        {
            videoStreams = Repository.VideoStream.GetVideoStreamsHidden()
                .ProjectTo<VideoStreamDto>(Mapper.ConfigurationProvider);
        }

        ParallelOptions po = new()
        {
            CancellationToken = cancellationToken,
            MaxDegreeOfParallelism = Environment.ProcessorCount
        };
        ConcurrentBag<TvChannel> retChannels = new();
        ConcurrentBag<Programme> retProgrammes = new();

        if (videoStreams.Any())
        {
            List<string> epgids = videoStreams.Where(a => !a.IsHidden).SelectMany(r => new string[] { r.User_Tvg_ID.ToLower(), r.User_Tvg_ID_DisplayName.ToLower() }).Distinct().ToList();

            List<Programme> programmes = MemoryCache.Programmes()
                .Where(a =>
                a.Channel != null &&
                (
                    epgids.Contains(a.Channel.ToLower()) ||
                    epgids.Contains(a.DisplayName.ToLower())
                )
                ).ToList();

            Setting setting = FileUtil.GetSetting();

            List<IconFileDto> icons = MemoryCache.Icons();
            List<IconFileDto> progIcons = MemoryCache.ProgrammeIcons();

            _ = Parallel.ForEach(videoStreams, po, videoStream =>
            {
                if (videoStream == null)
                {
                    return;
                }

                string logo = GetIconUrl(videoStream.User_Tvg_logo);

                TvChannel t;

                int dummy = 0;

                if (IsVideoStreamADummy(videoStream) || IsNotInProgrammes(programmes, videoStream))
                {
                    videoStream.User_Tvg_ID = "dummy";
                }

                if (videoStream.User_Tvg_ID.ToLower() == "dummy")
                {
                    dummy = GetDummy();

                    t = new TvChannel
                    {
                        Id = videoStream.User_Tvg_name,
                        Icon = new TvIcon { Src = logo },
                        Displayname = new()
                        {
                            videoStream.User_Tvg_name,
                            "dummy-" + dummy
                        }
                    };
                }
                else
                {
                    t = new TvChannel
                    {
                        Id = videoStream.User_Tvg_name,
                        Icon = new TvIcon { Src = logo },
                        Displayname = new()
                        {
                            videoStream.User_Tvg_name
                        }
                    };
                }

                retChannels.Add(t);

                if (videoStream.User_Tvg_ID != null)
                {
                    if (videoStream.User_Tvg_ID.ToLower() == "dummy")
                    {
                        Programme prog = new();

                        prog.Channel = videoStream.User_Tvg_name;

                        prog.Title = new TvTitle
                        {
                            Lang = "en",
                            Text = videoStream.User_Tvg_name,
                        };
                        prog.Desc = new TvDesc
                        {
                            Lang = "en",
                            Text = videoStream.User_Tvg_name,
                        };
                        DateTime now = DateTime.Now;
                        prog.Icon.Add(new TvIcon { Height = "10", Width = "10", Src = $"{url}/images/transparent.png" });
                        prog.Start = new DateTime(now.Year, now.Month, now.Day, 0, 0, 0).ToString("yyyyMMddHHmmss zzz").Replace(":", ""); ;
                        now = now.AddDays(7);
                        prog.Stop = new DateTime(now.Year, now.Month, now.Day, 0, 0, 0).ToString("yyyyMMddHHmmss zzz").Replace(":", ""); ;
                        prog.New = null;
                        prog.Previouslyshown = null;
                        retProgrammes.Add(prog);
                    }
                    else
                    {
                        if (programmes.Any())
                        {
                            IEnumerable<Programme>? progs = programmes.Where(a => a.DisplayName.ToLower() == videoStream.User_Tvg_ID.ToLower() || a.Channel.ToLower() == videoStream.User_Tvg_ID.ToLower()).DeepCopy();

                            if (progs != null)
                            {
                                foreach (Programme? p in progs)
                                {
                                    if (p.Icon.Any())
                                    {
                                        foreach (TvIcon progIcon in p.Icon)
                                        {
                                            if (progIcon != null && !string.IsNullOrEmpty(progIcon.Src))
                                            {
                                                IconFileDto? programmeIcon = progIcons.FirstOrDefault(a => a.SMFileType == SMFileTypes.ProgrammeIcon && a.Source == progIcon.Src);

                                                if (programmeIcon == null)
                                                {
                                                    continue;
                                                }
                                                // string IconSource = $"{url}/api/files/{(int)SMFileTypes.ProgrammeIcon}/{HttpUtility.UrlEncode(programmeIcon.Source)}";
                                                string IconSource = GetApiUrl(SMFileTypes.ProgrammeIcon, programmeIcon.Source);
                                                progIcon.Src = IconSource;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        p.Icon.Add(new TvIcon { Height = "", Width = "", Src = "" });
                                    }

                                    p.Channel = videoStream.User_Tvg_name;

                                    if (videoStream.User_Tvg_ID.ToLower() == "dummy")
                                    {
                                        p.Channel = videoStream.User_Tvg_name;

                                        p.Title = new TvTitle
                                        {
                                            Lang = "en",
                                            Text = videoStream.User_Tvg_name,
                                        };
                                        p.Desc = new TvDesc
                                        {
                                            Lang = "en",
                                            Text = videoStream.User_Tvg_name,
                                        };
                                    }

                                    if (string.IsNullOrEmpty(p.New))
                                    {
                                        p.New = null;
                                    }

                                    if (string.IsNullOrEmpty(p.Live))
                                    {
                                        p.Live = null;
                                    }

                                    if (string.IsNullOrEmpty(p.Premiere))
                                    {
                                        p.Premiere = null;
                                    }

                                    if (p.Previouslyshown == null || string.IsNullOrEmpty(p.Previouslyshown.Start))
                                    {
                                        p.Previouslyshown = null;
                                    }

                                    if (videoStream.User_Tvg_ID.ToLower() == "dummy")
                                    {
                                        continue;
                                    }
                                    retProgrammes.Add(p);
                                }
                            }
                        }
                    }
                }
            });
        }

        Tv ret = new()
        {
            Channel = retChannels.ToList(),
            Programme = retProgrammes.ToList()
        };

        XmlSerializerNamespaces ns = new();
        ns.Add("", "");

        using Utf8StringWriter textWriter = new();
        XmlSerializer serializer = new(typeof(Tv));
        serializer.Serialize(textWriter, ret, ns);
        textWriter.Close();
        return textWriter.ToString();
    }

    private string GetApiUrl(SMFileTypes path, string source)
    {
        string url = _httpContextAccessor.GetUrl();
        return $"{url}/api/files/{(int)path}/{WebUtility.UrlEncode(source)}";
    }

    private int GetDummy()
    {
        lock (Lock)
        {
            ++dummyCount;
            return dummyCount;
        }
    }

    private bool IsNotInProgrammes(IEnumerable<Programme> programmes, VideoStreamDto videoStream)
    {
        string userTvgId = videoStream.User_Tvg_ID.ToLower();
        string userTvgIdDisplayName = videoStream.User_Tvg_ID_DisplayName.ToLower();

        return !programmes.Any(p =>
            string.Equals(p.Channel, userTvgId, StringComparison.InvariantCultureIgnoreCase) ||
            string.Equals(p.Channel, userTvgIdDisplayName, StringComparison.InvariantCultureIgnoreCase));
    }

    private bool IsVideoStreamADummy(VideoStreamDto videoStream)
    {
        if (string.IsNullOrEmpty(videoStream.User_Tvg_ID))
        {
            return true;
        }

        if (!string.IsNullOrEmpty(_setting.DummyRegex))
        {
            Regex regex = new(_setting.DummyRegex, RegexOptions.ECMAScript | RegexOptions.IgnoreCase);
            bool test = regex.IsMatch(videoStream.User_Tvg_ID);
            return test;
        }

        return false;
    }
}
