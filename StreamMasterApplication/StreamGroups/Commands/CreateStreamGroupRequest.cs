using FluentValidation;

namespace StreamMasterApplication.StreamGroups.Commands;

public class CreateStreamGroupRequestValidator : AbstractValidator<CreateStreamGroupRequest>
{
    public CreateStreamGroupRequestValidator()
    {
        _ = RuleFor(v => v.StreamGroupNumber)
            .NotEmpty()
            .GreaterThan(0);

        _ = RuleFor(v => v.Name)
           .MaximumLength(32)
           .NotEmpty();
    }
}

[LogExecutionTimeAspect]
public class CreateStreamGroupRequestHandler(ILogger<CreateStreamGroupRequest> logger, IRepositoryWrapper repository, IMapper mapper,ISettingsService settingsService, IPublisher publisher, ISender sender, IHubContext<StreamMasterHub, IStreamMasterHub> hubContext) : BaseMediatorRequestHandler(logger, repository, mapper,settingsService, publisher, sender, hubContext), IRequestHandler<CreateStreamGroupRequest>
{
    public async Task Handle(CreateStreamGroupRequest command, CancellationToken cancellationToken)
    {
        if (command.StreamGroupNumber < 0)
        {
            return;
        }

        _ = Repository.StreamGroup.CreateStreamGroupRequestAsync(command, cancellationToken);

        await Publisher.Publish(new StreamGroupUpdateEvent(), cancellationToken).ConfigureAwait(false);

    }
}
