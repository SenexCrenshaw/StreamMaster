using FluentValidation;

namespace StreamMasterApplication.StreamGroups.Commands;


public class AddStreamGroupRequestValidator : AbstractValidator<AddStreamGroupRequest>
{
    public AddStreamGroupRequestValidator()
    {
        _ = RuleFor(v => v.StreamGroupNumber)
            .NotEmpty()
            .GreaterThan(0);

        _ = RuleFor(v => v.Name)
           .MaximumLength(32)
           .NotEmpty();
    }
}

public class AddStreamGroupRequestHandler : BaseMediatorRequestHandler, IRequestHandler<AddStreamGroupRequest>
{
    public AddStreamGroupRequestHandler(ILogger<AddStreamGroupRequest> logger, IRepositoryWrapper repository, IMapper mapper, IPublisher publisher, ISender sender, IHubContext<StreamMasterHub, IStreamMasterHub> hubContext)
: base(logger, repository, mapper, publisher, sender, hubContext) { }


    public async Task Handle(AddStreamGroupRequest command, CancellationToken cancellationToken)
    {
        if (command.StreamGroupNumber < 0)
        {
            return;
        }

        _ = Repository.StreamGroup.AddStreamGroupRequestAsync(command, cancellationToken);

        await Publisher.Publish(new StreamGroupUpdateEvent(), cancellationToken).ConfigureAwait(false);

    }
}
