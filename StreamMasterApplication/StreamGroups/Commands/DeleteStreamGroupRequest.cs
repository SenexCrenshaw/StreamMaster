using FluentValidation;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace StreamMasterApplication.StreamGroups.Commands;

public class DeleteStreamGroupRequest : IRequest<int?>
{
    public int Id { get; set; }
}

public class DeleteStreamGroupRequestValidator : AbstractValidator<DeleteStreamGroupRequest>
{
    public DeleteStreamGroupRequestValidator()
    {
        _ = RuleFor(v => v.Id)
            .NotNull()
            .GreaterThanOrEqualTo(0);
    }
}

public class DeleteStreamGroupHandler : IRequestHandler<DeleteStreamGroupRequest, int?>
{
    private readonly IAppDbContext _context;
    private readonly IPublisher _publisher;

    public DeleteStreamGroupHandler(
        IPublisher publisher,
         IAppDbContext context
        )
    {
        _publisher = publisher;
        _context = context;
    }

    public async Task<int?> Handle(DeleteStreamGroupRequest request, CancellationToken cancellationToken = default)
    {
        if (request.Id < 1)
        {
            return null;
        }

        StreamGroup? streamGroup = await _context.StreamGroups
            .Include(a => a.VideoStreams)
            .Include(a => a.ChannelGroups)
            .FirstOrDefaultAsync(a => a.Id == request.Id, cancellationToken: cancellationToken).ConfigureAwait(false);

        if (streamGroup == null)
        {
            return null;
        }

        if (streamGroup.ChannelGroups is not null && streamGroup.ChannelGroups.Any())
        {
            streamGroup.ChannelGroups.Clear();
            _ = await _context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        }

        if (streamGroup.VideoStreams is not null && streamGroup.VideoStreams.Any())
        {
            streamGroup.VideoStreams.Clear();
            _ = await _context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        }

        _ = _context.StreamGroups.Remove(streamGroup);

        if (await _context.SaveChangesAsync(cancellationToken).ConfigureAwait(false) > 0)
        {
            await _publisher.Publish(new StreamGroupDeleteEvent(streamGroup.Id), cancellationToken).ConfigureAwait(false);
            return streamGroup.Id;
        }

        return null;
    }
}
