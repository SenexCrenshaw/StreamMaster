using FluentValidation;

using MediatR;

using StreamMasterApplication.VideoStreams.Events;

using StreamMasterDomain.Dto;

namespace StreamMasterApplication.VideoStreams.Commands;

public class UpdateVideoStreamRequest : VideoStreamUpdate, IRequest<VideoStreamDto?>
{
}

public class UpdateVideoStreamRequestValidator : AbstractValidator<UpdateVideoStreamRequest>
{
    public UpdateVideoStreamRequestValidator()
    {
        _ = RuleFor(v => v.Id).NotNull().NotEmpty();
    }
}

public class UpdateVideoStreamRequestHandler : IRequestHandler<UpdateVideoStreamRequest, VideoStreamDto?>
{
    private readonly IAppDbContext _context;
    private readonly IPublisher _publisher;

    public UpdateVideoStreamRequestHandler(
        IPublisher publisher,
        IAppDbContext context
        )
    {
        _publisher = publisher;
        _context = context;
    }

    public async Task<VideoStreamDto?> Handle(UpdateVideoStreamRequest request, CancellationToken cancellationToken)
    {
        var ret = await _context.UpdateVideoStreamAsync(request, cancellationToken).ConfigureAwait(false);

        if (ret is not null)
        {
            await _publisher.Publish(new UpdateVideoStreamEvent(ret), cancellationToken).ConfigureAwait(false);
        }

        return ret;
    }
}
