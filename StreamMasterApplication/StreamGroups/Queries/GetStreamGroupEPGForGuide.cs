using AutoMapper;
using AutoMapper.QueryableExtensions;

using FluentValidation;

using MediatR;

using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

using StreamMasterApplication.Icons.Queries;

using StreamMasterDomain.Attributes;
using StreamMasterDomain.Dto;
using StreamMasterDomain.Entities.EPG;

using StreamMasterInfrastructure.Extensions;

using System.Collections.Concurrent;
using System.Diagnostics;
using System.Web;

namespace StreamMasterApplication.StreamGroups.Queries;

[RequireAll]
public class EPGGuide
{
    public List<EPGChannel> Channels { get; set; } = new();
    public DateTime EndDate { get; set; }
    public List<EPGProgram> Programs { get; set; } = new();
    public DateTime StartDate { get; set; }
}

[RequireAll]
public record GetStreamGroupEPGForGuide(int StreamGroupNumber) : IRequest<EPGGuide>;

public class GetStreamGroupEPGForGuideValidator : AbstractValidator<GetStreamGroupEPGForGuide>
{
    public GetStreamGroupEPGForGuideValidator()
    {
        _ = RuleFor(v => v.StreamGroupNumber)
            .NotNull().GreaterThanOrEqualTo(0);
    }
}

public partial class GetStreamGroupEPGForGuideHandler : IRequestHandler<GetStreamGroupEPGForGuide, EPGGuide>
{
    private readonly IAppDbContext _context;
    private readonly IMapper _mapper;
    private readonly IMemoryCache _memoryCache;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ISender _sender;
    private readonly object Lock = new();
    private int dummyCount = 0;

    public GetStreamGroupEPGForGuideHandler(
        IMapper mapper, IMemoryCache memoryCache,
        ISender sender,
         IHttpContextAccessor httpContextAccessor,
        IAppDbContext context)
    {
        _httpContextAccessor = httpContextAccessor;
        _memoryCache = memoryCache;
        _mapper = mapper;
        _context = context;
        _sender = sender;
    }

    public async Task<EPGGuide> Handle(GetStreamGroupEPGForGuide command, CancellationToken cancellationToken)
    {
        Stopwatch sw = Stopwatch.StartNew();

        List<VideoStreamDto> videoStreams = new();
        if (command.StreamGroupNumber > 0)
        {
            StreamGroupDto? sg = await _sender.Send(new GetStreamGroupByStreamNumber(command.StreamGroupNumber), cancellationToken).ConfigureAwait(false);
            if (sg == null)
            {
                return new()
                {
                    Channels = new(),
                    Programs = new()
                };
            }
            videoStreams = sg.VideoStreams.Where(a => !a.IsHidden).ToList();
        }
        else
        {
            videoStreams = _context.VideoStreams
                .Where(a => !a.IsHidden)
                .AsNoTracking()
                .ProjectTo<VideoStreamDto>(_mapper.ConfigurationProvider)
                .ToList();
        }

        ParallelOptions po = new()
        {
            CancellationToken = cancellationToken,
            MaxDegreeOfParallelism = System.Environment.ProcessorCount
        };

        ConcurrentBag<EPGChannel> retChannels = new();
        ConcurrentBag<EPGProgram> retProgrammes = new();
        EPGGuide ret = new();

        if (videoStreams.Any())
        {
            string url = _httpContextAccessor.GetUrl();

            List<string> epgids = videoStreams.Where(a => !a.IsHidden).Select(a => a.User_Tvg_ID.ToLower()).Distinct().ToList();

            List<Programme> programmes = _memoryCache.Programmes().Where(a => a.Channel != null && epgids.Contains(a.Channel.ToLower())).ToList();

            ret.StartDate = programmes.Min(a => a.StartDateTime);
            ret.EndDate = programmes.Max(a => a.StopDateTime);

            SettingDto setting = await _sender.Send(new GetSettings(), cancellationToken).ConfigureAwait(false);

            List<IconFile> progIcons = _context.Icons.Where(a => a.SMFileType == SMFileTypes.ProgrammeIcon && a.FileExists).ToList();

            var icons = await _sender.Send(new GetIcons(), cancellationToken).ConfigureAwait(false);

            _ = Parallel.ForEach(videoStreams, po, videoStream =>
            {
                if (videoStream == null)
                {
                    return;
                }

                IconFileDto? icon = icons.SingleOrDefault(a => a.OriginalSource == videoStream.User_Tvg_logo);
                string Logo = icon != null ? url+icon.Source : url+"/" + setting.DefaultIcon;

                EPGChannel t;
                int dummy = 0;
                if (string.IsNullOrEmpty(videoStream.User_Tvg_ID) || !programmes.Any(a => a.Channel.ToLower() == videoStream.User_Tvg_ID.ToLower()))
                {
                    videoStream.User_Tvg_ID = "dummy";
                }

                if (videoStream.User_Tvg_ID.ToLower() == "dummy")
                {
                    dummy = GetDummy();

                    t = new EPGChannel
                    {
                        UUID = videoStream.User_Tvg_ID + "-" + dummy,
                        Logo = Logo,
                        channelNumber = videoStream.User_Tvg_chno
                    };
                }
                else
                {
                    t = new EPGChannel
                    {
                        UUID = videoStream.User_Tvg_ID.ToString(),
                        Logo = Logo,
                        channelNumber = videoStream.User_Tvg_chno
                    };
                }

                retChannels.Add(t);

                if (videoStream.User_Tvg_ID != null)
                {
                    if (videoStream.User_Tvg_ID.ToLower() == "dummy")
                    {
                        var prog = new Programme();
                        prog.Channel = videoStream.User_Tvg_ID + "-" + dummy;

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
                        prog.Icon.Add(new TvIcon { Height = "10", Width = "10", Src = $"{url}images / transparent.png" });
                        prog.StartDateTime = DateTime.Now.AddHours(-1);
                        prog.StopDateTime = DateTime.Now.AddDays(7);

                        retProgrammes.Add(GetEPGProgramFromProgramme(prog, videoStream.Id));
                    }
                    else
                    {
                        if (programmes.Any())
                        {
                            IEnumerable<Programme>? progs = programmes.Where(a => a.Channel.ToLower() == videoStream.User_Tvg_ID.ToLower()).DeepCopy();

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
                                                IconFile? programmeIcon = progIcons.FirstOrDefault(a => a.SMFileType == SMFileTypes.ProgrammeIcon && a.Source == progIcon.Src);

                                                if (programmeIcon == null)
                                                {
                                                    continue;
                                                }
                                                string IconSource = $"{url}/api/files/{(int)SMFileTypes.ProgrammeIcon}/{HttpUtility.UrlEncode(programmeIcon.Source)}";
                                                progIcon.Src = IconSource;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        p.Icon.Add(new TvIcon { Height = "10", Width = "10", Src = "images/transparent.png" });
                                    }

                                    p.Channel = videoStream.User_Tvg_ID;

                                    retProgrammes.Add(GetEPGProgramFromProgramme(p, videoStream.Id));
                                }
                            }
                        }
                    }
                }
            });
        }

        ret.Channels = retChannels.OrderBy(a => a.channelNumber).ToList();
        ret.Programs = retProgrammes.OrderBy(a => a.Since).ToList();

        return ret;
    }

    private static EPGProgram GetEPGProgramFromProgramme(Programme programme, int videoStreamId)
    {
        var largest = 0;
        TvIcon? Icon = null;

        foreach (var icon in programme.Icon)
        {
            int w, h = 0;

            int.TryParse(icon.Width, out w);
            int.TryParse(icon.Height, out h);

            var res = w * h;

            if (res > largest)
            {
                Icon = icon;
                largest = res;
            }
        }

        return new EPGProgram
        {
            Id = Guid.NewGuid().ToString(),
            ChannelUuid = programme.Channel,
            Description = programme.Desc.Text ?? "",
            Since = programme.StartDateTime,
            Till = programme.StopDateTime,
            Title = programme.Title.Text ?? "",
            Image = Icon?.Src ?? "",
            VideoStreamId = videoStreamId,
        };
    }

    private int GetDummy()
    {
        lock (Lock)
        {
            ++dummyCount;
            return dummyCount;
        }
    }
}
