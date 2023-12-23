using FluentValidation;

using StreamMasterApplication.StreamGroups.Events;

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

    public UpdateStreamGroupRequestHandler(ILogger<UpdateStreamGroupRequest> logger, IRepositoryWrapper repository, IMapper mapper, ISettingsService settingsService, IPublisher publisher, ISender sender, IHubContext<StreamMasterHub, IStreamMasterHub> hubContext, IMemoryCache memoryCache)
  : base(logger, repository, mapper, settingsService, publisher, sender, hubContext, memoryCache) { }


    public async Task<StreamGroupDto?> Handle(UpdateStreamGroupRequest request, CancellationToken cancellationToken)
    {
        if (request.StreamGroupId < 1 || string.IsNullOrEmpty(request.Name))
        {
            return null;
        }

        StreamGroupDto? streamGroup = await Repository.StreamGroup.UpdateStreamGroup(request);
        if (streamGroup is not null)
        {

            await Publisher.Publish(new StreamGroupUpdateEvent(streamGroup), cancellationToken).ConfigureAwait(false);
        }

        return streamGroup;
    }
}
