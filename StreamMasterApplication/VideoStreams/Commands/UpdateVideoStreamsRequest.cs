using AutoMapper;

using FluentValidation;

using MediatR;

using StreamMasterApplication.VideoStreams.Events;

using StreamMasterDomain.Dto;

namespace StreamMasterApplication.VideoStreams.Commands;

public record UpdateVideoStreamsRequest(IEnumerable<UpdateVideoStreamRequest> VideoStreamUpdates) : IRequest<IEnumerable<VideoStreamDto>>
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
        List<VideoStreamDto> results = new();

        foreach (var request in requests.VideoStreamUpdates)
        {
            var ret = await _context.UpdateVideoStreamAsync(request, cancellationToken).ConfigureAwait(false);
            if (ret is not null)
            {
                results.Add(ret);
            }
        }

        if (results.Any())
        {
            await _publisher.Publish(new UpdateVideoStreamsEvent(results), cancellationToken).ConfigureAwait(false);
        }

        return results;
    }
}
