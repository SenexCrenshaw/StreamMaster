using FluentValidation;

using StreamMaster.Application.StreamGroups.Events;

namespace StreamMaster.Application.StreamGroups.CommandsOld;

public record DeleteStreamGroupRequest(int Id) : IRequest<int?> { }

public class DeleteStreamGroupRequestValidator : AbstractValidator<DeleteStreamGroupRequest>
{
    public DeleteStreamGroupRequestValidator()
    {
        _ = RuleFor(v => v.Id)
            .NotNull()
            .GreaterThanOrEqualTo(0);
    }
}

public class DeleteStreamGroupRequestHandler(ILogger<DeleteStreamGroupRequest> logger, IRepositoryWrapper Repository, IPublisher Publisher)
    : IRequestHandler<DeleteStreamGroupRequest, int?>
{
    public async Task<int?> Handle(DeleteStreamGroupRequest request, CancellationToken cancellationToken = default)
    {
        if (request.Id < 1)
        {
            return null;
        }

        if (await Repository.StreamGroup.DeleteStreamGroup(request.Id) != null)
        {
            await Repository.SaveAsync();
            await Publisher.Publish(new StreamGroupDeleteEvent(), cancellationToken);
            return request.Id;
        }

        return null;
    }
}
