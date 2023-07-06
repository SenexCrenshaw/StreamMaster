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
        var VideoStream = await _context.VideoStreams.Include(a => a.ChildRelationships).FirstOrDefaultAsync(a => a.Id == request.VideoStreamId, cancellationToken).ConfigureAwait(false);
        if (VideoStream == null)
        {
            return null;
        }

        var relationsShips = _context.VideoStreamRelationships
            .Include(a => a.ChildVideoStream)
            .Where(a =>
            a.ParentVideoStreamId == request.VideoStreamId ||
            a.ChildVideoStreamId == request.VideoStreamId
            )
            .ToList();

        var sgStreams = _context.StreamGroups.Include(a => a.VideoStreams).Where(a => a.VideoStreams.Any(a => a.Id == request.VideoStreamId)).ToList();

        _context.VideoStreamRelationships.RemoveRange(relationsShips);
        _context.StreamGroups.RemoveRange(sgStreams);
        _context.VideoStreams.Remove(VideoStream);

        _ = await _context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        await _publisher.Publish(new DeleteVideoStreamEvent(VideoStream.Id), cancellationToken).ConfigureAwait(false);
        return VideoStream.Id;
    }
}