using FluentValidation;

namespace StreamMasterApplication.StreamGroups.Commands;

public class UpdateStreamGroupRequestValidator : AbstractValidator<UpdateStreamGroupRequest>
{
    public UpdateStreamGroupRequestValidator()
    {
        _ = RuleFor(v => v.StreamGroupId)
           .NotNull()
           .GreaterThanOrEqualTo(0);
    }
}

[LogExecutionTimeAspect]
public class UpdateStreamGroupRequestHandler : BaseMediatorRequestHandler, IRequestHandler<UpdateStreamGroupRequest, StreamGroupDto?>
{

    public UpdateStreamGroupRequestHandler(ILogger<UpdateStreamGroupRequest> logger, IRepositoryWrapper repository, IMapper mapper,ISettingsService settingsService, IPublisher publisher, ISender sender, IHubContext<StreamMasterHub, IStreamMasterHub> hubContext)
  : base(logger, repository, mapper,settingsService, publisher, sender, hubContext) { }


    public async Task<StreamGroupDto?> Handle(UpdateStreamGroupRequest request, CancellationToken cancellationToken)
    {
        if (request.StreamGroupId < 1)
        {
            return null;
        }

        StreamGroupDto? streamGroup = await Repository.StreamGroup.UpdateStreamGroupAsync(request, cancellationToken).ConfigureAwait(false);
        if (streamGroup is not null)
        {
            await Publisher.Publish(new StreamGroupUpdateEvent(), cancellationToken).ConfigureAwait(false);
        }

        return streamGroup;
    }
}
