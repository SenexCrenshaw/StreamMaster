using FluentValidation;

using StreamMaster.Application.StreamGroups.Events;
using StreamMaster.Domain.Requests;

namespace StreamMaster.Application.StreamGroups.CommandsOld;

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
public class CreateStreamGroupRequestHandler(ILogger<CreateStreamGroupRequest> logger, IRepositoryWrapper Repository, IPublisher Publisher, IMapper Mapper)
    : IRequestHandler<CreateStreamGroupRequest>
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
