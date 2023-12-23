using FluentValidation;

using StreamMasterApplication.StreamGroups.Events;

using StreamMasterDomain.Requests;

namespace StreamMasterApplication.StreamGroups.Commands;

public class CreateStreamGroupRequestValidator : AbstractValidator<CreateStreamGroupRequest>
{
    public CreateStreamGroupRequestValidator()
    {

        _ = RuleFor(v => v.Name)
           .MaximumLength(32)
           .NotEmpty();
    }
}

[LogExecutionTimeAspect]
public class CreateStreamGroupRequestHandler(ILogger<CreateStreamGroupRequest> logger, IRepositoryWrapper repository, IMapper mapper, ISettingsService settingsService, IPublisher publisher, ISender sender, IHubContext<StreamMasterHub, IStreamMasterHub> hubContext, IMemoryCache memoryCache) : BaseMediatorRequestHandler(logger, repository, mapper, settingsService, publisher, sender, hubContext, memoryCache), IRequestHandler<CreateStreamGroupRequest>
{
    public async Task Handle(CreateStreamGroupRequest request, CancellationToken cancellationToken)
    {

        StreamGroup streamGroup = new()
        {
            Name = request.Name,
        };

        Repository.StreamGroup.CreateStreamGroup(streamGroup);
        await Repository.SaveAsync();
        StreamGroupDto dto = Mapper.Map<StreamGroupDto>(streamGroup);
        await Publisher.Publish(new StreamGroupCreateEvent(dto), cancellationToken);
    }
}
