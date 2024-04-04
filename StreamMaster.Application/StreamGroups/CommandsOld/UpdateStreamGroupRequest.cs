using FluentValidation;

using StreamMaster.Application.StreamGroups.Events;

namespace StreamMaster.Application.StreamGroups.CommandsOld;

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
public class UpdateStreamGroupRequestHandler(ILogger<UpdateStreamGroupRequest> logger, IRepositoryWrapper Repository, IPublisher Publisher)
    : IRequestHandler<UpdateStreamGroupRequest, StreamGroupDto?>
{
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
