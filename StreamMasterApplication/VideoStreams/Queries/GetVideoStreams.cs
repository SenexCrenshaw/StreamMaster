using AutoMapper;

using MediatR;

using Microsoft.EntityFrameworkCore;

using StreamMasterApplication.Icons.Queries;

using StreamMasterDomain.Dto;

using System.Collections.Concurrent;

namespace StreamMasterApplication.VideoStreams.Queries;

public record GetVideoStreams : IRequest<IEnumerable<VideoStreamDto>>;

internal class GetVideoStreamsHandler : IRequestHandler<GetVideoStreams, IEnumerable<VideoStreamDto>>
{
    private readonly IAppDbContext _context;
    private readonly IMapper _mapper;

    private readonly ISender _sender;

    public GetVideoStreamsHandler(

         IMapper mapper,
         ISender sender,
        IAppDbContext context)
    {
        _sender = sender;
        _mapper = mapper;

        _context = context;
    }

    public async Task<IEnumerable<VideoStreamDto>> Handle(GetVideoStreams request, CancellationToken cancellationToken)
    {
        SettingDto setting = await _sender.Send(new GetSettings(), cancellationToken).ConfigureAwait(false);
        List<IconFileDto> icons = await _sender.Send(new GetIcons(), cancellationToken).ConfigureAwait(false);

        ConcurrentBag<VideoStreamDto> streamsDtos = new();

        var streams = _context.VideoStreams.ToList();

        ParallelOptions po = new()
        {
            CancellationToken = cancellationToken,
            MaxDegreeOfParallelism = Environment.ProcessorCount
        };

        var allStreams = _context.
                 VideoStreamRelationships.
                 Include(a => a.ChildVideoStream).
                 AsNoTracking().ToList();

        var relationsShips = _context.VideoStreamRelationships.Include(a => a.ChildVideoStream).ToList();

        _ = Parallel.ForEach(streams, po, videoStream =>
        {
            VideoStreamDto videoStreamDto = _mapper.Map<VideoStreamDto>(videoStream);

            if (setting.CacheIcons && !string.IsNullOrEmpty(videoStreamDto.User_Tvg_logo))
            {
                IconFileDto? icon = icons.SingleOrDefault(a => a.OriginalSource == videoStreamDto.User_Tvg_logo || a.Name == videoStreamDto.User_Tvg_logo);
                string Logo = icon != null ? icon.Source : setting.BaseHostURL + setting.DefaultIcon;

                videoStreamDto.User_Tvg_logo = Logo;
            }

            var videoStreams = relationsShips
                .Where(a => a.ParentVideoStreamId == videoStream.Id)
                .Select(a => new
                {
                    ChildVideoStream = a.ChildVideoStream,
                    Rank = a.Rank
                }
                )
                .ToList();

            var childVideoStreams = new List<ChildVideoStreamDto>();

            foreach (var child in videoStreams)
            {
                if (!string.IsNullOrEmpty(child.ChildVideoStream.User_Tvg_logo))
                {
                    if (setting.CacheIcons)
                    {
                        IconFileDto? icon = icons.SingleOrDefault(a => a.OriginalSource == child.ChildVideoStream.User_Tvg_logo);
                        string Logo = icon != null ? icon.Source : setting.BaseHostURL + setting.DefaultIcon;
                        child.ChildVideoStream.User_Tvg_logo = Logo;
                    }

                    var cto = _mapper.Map<ChildVideoStreamDto>(child.ChildVideoStream);
                    cto.Rank = child.Rank;
                    childVideoStreams.Add(cto);
                }
            }

            videoStreamDto.ChildVideoStreams = childVideoStreams;

            streamsDtos.Add(videoStreamDto);
        });

        return streamsDtos;
    }
}
