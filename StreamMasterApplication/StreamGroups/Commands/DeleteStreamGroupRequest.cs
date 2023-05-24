using FluentValidation;

using MediatR;

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

        StreamGroup? entity = await _context.StreamGroups.FindAsync(new object?[] { request.Id }, cancellationToken: cancellationToken).ConfigureAwait(false);
        if (entity == null)
        {
            return null;
        }

        if (entity.VideoStreams is not null && entity.VideoStreams.Any())
        {
            entity.VideoStreams.Clear();
            _ = await _context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        }

        _ = _context.StreamGroups.Remove(entity);

        if (await _context.SaveChangesAsync(cancellationToken).ConfigureAwait(false) > 0)
        {
            await _publisher.Publish(new StreamGroupDeleteEvent(entity.Id), cancellationToken).ConfigureAwait(false);
            return entity.Id;
        }

        return null;
    }
}
