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

        if (await _context.DeleteStreamGroupsync(request.Id, cancellationToken))
        {
            await _publisher.Publish(new StreamGroupDeleteEvent(request.Id), cancellationToken).ConfigureAwait(false);
            return request.Id;
        }

        return null;
    }
}
