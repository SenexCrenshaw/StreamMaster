using AutoMapper;

using MediatR;

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
        Setting setting = FileUtil.GetSetting();
        List<IconFileDto> icons = await _context.GetIcons(cancellationToken).ConfigureAwait(false);

        ConcurrentBag<VideoStreamDto> streamsDtos = new();

        var streams = await _context.GetAllVideoStreamsWithChildrenAsync().ConfigureAwait(false);

        ParallelOptions po = new()
        {
            CancellationToken = cancellationToken,
            MaxDegreeOfParallelism = Environment.ProcessorCount
        };

        _ = Parallel.ForEach(streams, po, videoStream =>
        {
            VideoStreamDto videoStreamDto = _mapper.Map<VideoStreamDto>(videoStream);

            if (setting.CacheIcons && !string.IsNullOrEmpty(videoStreamDto.User_Tvg_logo))
            {
                IconFileDto? icon = icons.SingleOrDefault(a => a.OriginalSource == videoStreamDto.User_Tvg_logo || a.Name == videoStreamDto.User_Tvg_logo);
                string Logo = icon != null ? icon.Source : "/" + setting.DefaultIcon;

                videoStreamDto.User_Tvg_logo = Logo;
            }

            foreach (var child in videoStream.ChildVideoStreams)
            {
                if (!string.IsNullOrEmpty(child.ChildVideoStream.User_Tvg_logo))
                {
                    if (setting.CacheIcons)
                    {
                        IconFileDto? icon = icons.SingleOrDefault(a => a.OriginalSource == child.ChildVideoStream.User_Tvg_logo);
                        string Logo = icon != null ? icon.Source : "/" + setting.DefaultIcon;
                        child.ChildVideoStream.User_Tvg_logo = Logo;
                    }
                }
            }

            streamsDtos.Add(videoStreamDto);
        });

        return streamsDtos;
    }
}
