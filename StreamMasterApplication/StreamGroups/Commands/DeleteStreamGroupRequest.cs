using FluentValidation;

namespace StreamMasterApplication.StreamGroups.Commands;

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

public class DeleteStreamGroupRequestHandler : BaseMediatorRequestHandler, IRequestHandler<DeleteStreamGroupRequest, int?>
{

    public DeleteStreamGroupRequestHandler(ILogger<DeleteStreamGroupRequest> logger, IRepositoryWrapper repository, IMapper mapper,ISettingsService settingsService, IPublisher publisher, ISender sender, IHubContext<StreamMasterHub, IStreamMasterHub> hubContext)
  : base(logger, repository, mapper,settingsService, publisher, sender, hubContext) { }


    public async Task<int?> Handle(DeleteStreamGroupRequest request, CancellationToken cancellationToken = default)
    {
        if (request.Id < 1)
        {
            return null;
        }

        if (await Repository.StreamGroup.DeleteStreamGroupsync(request.Id, cancellationToken))
        {
            await Publisher.Publish(new StreamGroupDeleteEvent(), cancellationToken).ConfigureAwait(false);
            return request.Id;
        }

        return null;
    }
}
