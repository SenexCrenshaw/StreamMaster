using FluentValidation;

using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

using StreamMaster.SchedulesDirectAPI.Domain.EPG;

using StreamMasterApplication.Common.Extensions;
using StreamMasterApplication.Programmes.Queries;

using System.Collections.Concurrent;
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
public record GetStreamGroupEPGForGuide(int streamGroupId) : IRequest<EPGGuide>;

public class GetStreamGroupEPGForGuideValidator : AbstractValidator<GetStreamGroupEPGForGuide>
{
    public GetStreamGroupEPGForGuideValidator()
    {
        _ = RuleFor(v => v.streamGroupId)
            .NotNull().GreaterThanOrEqualTo(0);
    }
}

public partial class GetStreamGroupEPGForGuideHandler(IHttpContextAccessor httpContextAccessor, ILogger<GetStreamGroupEPGForGuide> logger, IRepositoryWrapper repository, IMapper mapper, ISettingsService settingsService, IPublisher publisher, ISender sender, IHubContext<StreamMasterHub, IStreamMasterHub> hubContext, IMemoryCache memoryCache) : BaseMediatorRequestHandler(logger, repository, mapper, settingsService, publisher, sender, hubContext, memoryCache), IRequestHandler<GetStreamGroupEPGForGuide, EPGGuide>
{

    private readonly object Lock = new();
    private int dummyCount = 0;

    public async Task<EPGGuide> Handle(GetStreamGroupEPGForGuide request, CancellationToken cancellationToken)
    {

        IEnumerable<VideoStreamDto> videoStreams;
        if (request.streamGroupId > 0)
        {
            StreamGroup? streamGroup = await Repository.StreamGroup
                     .GetStreamGroupQuery()
                     .Include(a => a.ChildVideoStreams)
                     .FirstOrDefaultAsync(a => a.Id == request.streamGroupId);

            if (streamGroup == null)
            {
                return new()
                {
                    Channels = new(),
                    Programs = new()
                };
            }
            videoStreams = mapper.Map<IEnumerable<VideoStreamDto>>(streamGroup.ChildVideoStreams.Select(a => a.ChildVideoStream).Where(a => !a.IsHidden));
        }
        else
        {
            videoStreams = await Repository.VideoStream.GetVideoStreamsNotHidden().ConfigureAwait(false);
        }

        string Url = httpContextAccessor.GetUrl();

        ParallelOptions po = new()
        {
            CancellationToken = cancellationToken,
            MaxDegreeOfParallelism = Environment.ProcessorCount
        };

        ConcurrentBag<EPGChannel> retChannels = new();
        ConcurrentBag<EPGProgram> retProgrammes = new();
        EPGGuide ret = new();
        Setting setting = await GetSettingsAsync();
        int epgdays = setting.SDEPGDays;
        if (videoStreams.Any())
        {
            List<string> epgids = videoStreams.Where(a => !a.IsHidden).Select(a => a.User_Tvg_ID.ToLower()).Distinct().ToList();

            List<Programme> c = await Sender.Send(new GetProgrammesRequest(), cancellationToken).ConfigureAwait(false);

            List<Programme> programmes = c.Where(a => a.Channel != null && epgids.Contains(a.Channel.ToLower())).ToList();

            if (programmes.Any())
            {
                ret.StartDate = programmes.Min(a => a.StartDateTime);
                ret.EndDate = programmes.Max(a => a.StopDateTime);
            }
            else
            {
                ret.StartDate = DateTime.Now.AddHours(-1);
                ret.EndDate = DateTime.Now.AddDays(epgdays);
            }

            //ret.StartDate = programmes.Min(a => a.StartDateTime);
            //ret.EndDate = programmes.Max(a => a.StopDateTime);

            List<IconFileDto> icons = MemoryCache.Icons();

            List<IconFileDto> progIcons = icons.Where(a => a.SMFileType == SMFileTypes.ProgrammeIcon).ToList();

            _ = Parallel.ForEach(videoStreams, po, videoStream =>
            {
                if (videoStream == null)
                {
                    return;
                }

                IconFileDto? icon = icons.SingleOrDefault(a => a.Source == videoStream.User_Tvg_logo);
                string Logo = icon != null ? Url + icon.Source : Url + "/" + setting.DefaultIcon;

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
                        Programme prog = new()
                        {
                            Channel = videoStream.User_Tvg_ID + "-" + dummy,

                            Title = new List<TvTitle>{
                                new TvTitle
                            {
                                Lang = "en",
                                Text = videoStream.User_Tvg_name,
                            }
                            }
                            ,
                            Desc = new TvDesc
                            {
                                Lang = "en",
                                Text = videoStream.User_Tvg_name,
                            }
                        };
                        prog.Icon.Add(new TvIcon { Height = "10", Width = "10", Src = $"{Url}/images/transparent.png" });
                        prog.StartDateTime = DateTime.Now.AddHours(-1);
                        prog.StopDateTime = DateTime.Now.AddDays(epgdays);

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
                                                IconFileDto? programmeIcon = progIcons.FirstOrDefault(a => a.SMFileType == SMFileTypes.ProgrammeIcon && a.Source == progIcon.Src);

                                                if (programmeIcon == null)
                                                {
                                                    continue;
                                                }
                                                string IconSource = $"{Url}/api/files/{(int)SMFileTypes.ProgrammeIcon}/{HttpUtility.UrlEncode(programmeIcon.Source)}";
                                                progIcon.Src = IconSource;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        p.Icon.Add(new TvIcon { Height = "10", Width = "10", Src = $"{Url}/images/transparent.png" });
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

    private static EPGProgram GetEPGProgramFromProgramme(Programme programme, string videoStreamId)
    {
        int largest = 0;
        TvIcon? Icon = null;

        foreach (TvIcon icon in programme.Icon)
        {

            int.TryParse(icon.Width, out int w);
            int.TryParse(icon.Height, out int h);

            int res = w * h;

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
            Title = programme.Title[0].Text ?? "",
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
