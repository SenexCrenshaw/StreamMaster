using AutoMapper;

using MediatR;

using Microsoft.EntityFrameworkCore;

using StreamMasterApplication.Icons.Queries;

using StreamMasterDomain.Dto;

namespace StreamMasterApplication.VideoStreams.Queries;

public record GetVideoStream(int Id) : IRequest<VideoStreamDto?>;

internal class GetVideoStreamHandler : IRequestHandler<GetVideoStream, VideoStreamDto?>
{
    private readonly IAppDbContext _context;
    private readonly IMapper _mapper;
    private readonly ISender _sender;

    public GetVideoStreamHandler(
        IMapper mapper,
        ISender sender,
        IAppDbContext context
    )
    {
        _sender = sender;
        _mapper = mapper;
        _context = context;
    }

    public async Task<VideoStreamDto?> Handle(GetVideoStream request, CancellationToken cancellationToken)
    {
        var videoStream = _context.VideoStreams.FirstOrDefault(a => a.Id == request.Id);

        if (videoStream == null)
        {
            return null;
        }

        SettingDto setting = await _sender.Send(new GetSettings(), cancellationToken).ConfigureAwait(false);
        List<IconFileDto> icons = await _sender.Send(new GetIcons(), cancellationToken).ConfigureAwait(false);

        var videoStreams = _context.
            VideoStreamRelationships.
            Include(c => c.ChildVideoStream).
            Where(a => a.ParentVideoStreamId == videoStream.Id).
            Select(a => new
            {
                ChildVideoStream = a.ChildVideoStream,
                Rank = a.Rank
            }).ToList();

        VideoStreamDto videoStreamDto = _mapper.Map<VideoStreamDto>(videoStream);

        if (setting.CacheIcons && !string.IsNullOrEmpty(videoStreamDto.User_Tvg_logo))
        {
            IconFileDto? icon = icons.SingleOrDefault(a => a.OriginalSource == videoStreamDto.User_Tvg_logo || a.Name == videoStreamDto.User_Tvg_logo);
            string Logo = icon != null ? icon.Source : setting.BaseHostURL + setting.DefaultIcon;

            videoStreamDto.User_Tvg_logo = Logo;
        }

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

        return videoStreamDto;
    }
}
