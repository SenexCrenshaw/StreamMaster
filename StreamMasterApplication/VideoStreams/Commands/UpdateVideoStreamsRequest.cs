using AutoMapper;

using FluentValidation;

using MediatR;

using Microsoft.EntityFrameworkCore;

using StreamMasterApplication.Icons.Queries;
using StreamMasterApplication.VideoStreams.Events;

using StreamMasterDomain.Dto;

namespace StreamMasterApplication.VideoStreams.Commands;

public record UpdateVideoStreamsRequest(IEnumerable<VideoStreamUpdate> VideoStreamUpdates) : IRequest<IEnumerable<VideoStreamDto>>
{
}

public class UpdateVideoStreamsRequestValidator : AbstractValidator<UpdateVideoStreamsRequest>
{
    public UpdateVideoStreamsRequestValidator()
    {
        _ = RuleFor(v => v.VideoStreamUpdates).NotNull().NotEmpty();
    }
}

public class UpdateVideoStreamsRequestHandler : IRequestHandler<UpdateVideoStreamsRequest, IEnumerable<VideoStreamDto>>
{
    private readonly IAppDbContext _context;
    private readonly IMapper _mapper;
    private readonly IPublisher _publisher;
    private readonly ISender _sender;

    public UpdateVideoStreamsRequestHandler(
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

    public async Task<IEnumerable<VideoStreamDto>> Handle(UpdateVideoStreamsRequest requests, CancellationToken cancellationToken)
    {
        bool isChanged = false;
        List<VideoStreamDto> results = new();
        List<IconFileDto> icons = await _sender.Send(new GetIcons(), cancellationToken).ConfigureAwait(false);
        SettingDto setting = await _sender.Send(new GetSettings(), cancellationToken).ConfigureAwait(false);
        foreach (var request in requests.VideoStreamUpdates)
        {
            VideoStream? videoStream = _context.VideoStreams
           .Include(vs => vs.ParentRelationships)
           .FirstOrDefault(a => a.Id == request.Id);

            if (videoStream == null)
            {
                continue;
            }

            isChanged = videoStream.UpdateVideoStream(request);

            if (request.ChildVideoStreams != null)
            {
                _context.SynchronizeChildRelationships(videoStream, request.ChildVideoStreams);
            }

            var newLogo = request.Tvg_logo;
            if (request.Tvg_logo != null && videoStream.User_Tvg_logo != request.Tvg_logo)
            {
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

            if (isChanged)
            {
                _ = await _context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

                VideoStreamDto videoStreamDto = _mapper.Map<VideoStreamDto>(videoStream);

                IconFileDto? icon = icons.SingleOrDefault(a => a.OriginalSource == videoStream.User_Tvg_logo);
                string Logo = icon != null ? icon.Source : "/" + setting.DefaultIcon;

                videoStreamDto.User_Tvg_logo = newLogo;

                results.Add(videoStreamDto);
            }
        }
        if (results.Any())
        {
            await _publisher.Publish(new UpdateVideoStreamsEvent(results), cancellationToken).ConfigureAwait(false);
        }

        return results;
    }
}
