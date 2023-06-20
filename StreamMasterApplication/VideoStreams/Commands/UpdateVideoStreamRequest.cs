using AutoMapper;

using FluentValidation;

using MediatR;

using Microsoft.EntityFrameworkCore;

using StreamMasterApplication.Icons.Queries;
using StreamMasterApplication.VideoStreams.Events;

using StreamMasterDomain.Dto;

namespace StreamMasterApplication.VideoStreams.Commands;

public class UpdateVideoStreamRequest : VideoStreamUpdate, IRequest<VideoStreamDto?>
{
    public string BaseHostUrl { get; set; }
}

public class UpdateVideoStreamRequestValidator : AbstractValidator<UpdateVideoStreamRequest>
{
    public UpdateVideoStreamRequestValidator()
    {
        _ = RuleFor(v => v.Id).NotNull().GreaterThan(0);
    }
}

public class UpdateVideoStreamRequestHandler : IRequestHandler<UpdateVideoStreamRequest, VideoStreamDto?>
{
    private readonly IAppDbContext _context;
    private readonly IMapper _mapper;
    private readonly IPublisher _publisher;
    private readonly ISender _sender;

    public UpdateVideoStreamRequestHandler(
        IMapper mapper,
        ISender sender,
        IPublisher publisher,
        IAppDbContext context
        )
    {
        _sender = sender;
        _publisher = publisher;
        _mapper = mapper;
        _context = context;
    }

    public async Task<VideoStreamDto?> Handle(UpdateVideoStreamRequest request, CancellationToken cancellationToken)
    {
        VideoStream? videoStream = _context.VideoStreams
            .Include(vs => vs.ParentRelationships)
            .SingleOrDefault(a => a.Id == request.Id);

        if (videoStream == null)
        {
            return null;
        }

        SettingDto setting = await _sender.Send(new GetSettings(), cancellationToken).ConfigureAwait(false);

        List<IconFileDto> icons = await _sender.Send(new GetIcons(), cancellationToken).ConfigureAwait(false);

        bool isChanged = videoStream.UpdateVideoStream(request);

        var newLogo = videoStream.User_Tvg_logo;

        if (request.Tvg_logo != null && videoStream.User_Tvg_logo != request.Tvg_logo)
        {
            isChanged = true;

            IconFileDto? logo = icons.FirstOrDefault(a => a.OriginalSource == request.Tvg_logo);

            if (logo != null)
            {
                videoStream.User_Tvg_logo = logo.OriginalSource;
                newLogo = logo.Source;
            }
            else
            {
                videoStream.User_Tvg_logo = request.Tvg_logo;
            }
        }

        _ = await _context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        if (request.ChildVideoStreams != null)
        {
            isChanged = isChanged || _context.SynchronizeChildRelationships(videoStream, request.ChildVideoStreams);
        }

        _ = await _context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        var videoStreams = _context.
            VideoStreamRelationships.
            Include(a => a.ChildVideoStream).
            Where(a => a.ParentVideoStreamId == videoStream.Id).Select(a => a.ChildVideoStream).ToList();

        VideoStreamDto ret = _mapper.Map<VideoStreamDto>(videoStream);

        ret.ChildVideoStreams = _mapper.Map<List<ChildVideoStreamDto>>(videoStreams);

        IconFileDto? icon = icons.SingleOrDefault(a => a.OriginalSource == videoStream.User_Tvg_logo);
        string Logo = icon != null ? icon.Source : "/" + setting.DefaultIcon;

        ret.User_Tvg_logo = Logo;

        if (isChanged)
        {
            await _publisher.Publish(new UpdateVideoStreamEvent(ret), cancellationToken).ConfigureAwait(false);
        }

        return ret;
    }
}
