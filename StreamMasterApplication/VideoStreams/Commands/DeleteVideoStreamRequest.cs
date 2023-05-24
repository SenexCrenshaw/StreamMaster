using FluentValidation;

using MediatR;

using Microsoft.EntityFrameworkCore;

using StreamMasterApplication.VideoStreams.Events;

namespace StreamMasterApplication.VideoStreams.Commands;

public record DeleteVideoStreamRequest(int VideoStreamId) : IRequest<int?>
{
}

public class DeleteVideoStreamRequestValidator : AbstractValidator<DeleteVideoStreamRequest>
{
    public DeleteVideoStreamRequestValidator()
    {
        _ = RuleFor(v => v.VideoStreamId).NotNull().GreaterThan(0);
    }
}

public class DeleteVideoStreamRequestHandler : IRequestHandler<DeleteVideoStreamRequest, int?>
{
    private readonly IAppDbContext _context;

    private readonly IPublisher _publisher;

    public DeleteVideoStreamRequestHandler(

         IPublisher publisher,
        IAppDbContext context
        )
    {
        _publisher = publisher;

        _context = context;
    }

    public async Task<int?> Handle(DeleteVideoStreamRequest request, CancellationToken cancellationToken)
    {
        var VideoStream = await _context.VideoStreams.FirstOrDefaultAsync(a => a.Id == request.VideoStreamId, cancellationToken).ConfigureAwait(false);
        if (VideoStream == null)
        {
            return null;
        }

        _context.VideoStreams.Remove(VideoStream);

        _ = await _context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        await _publisher.Publish(new DeleteVideoStreamEvent(VideoStream.Id), cancellationToken).ConfigureAwait(false);
        return VideoStream.Id;
    }
}
