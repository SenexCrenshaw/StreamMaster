using FluentValidation;

using StreamMaster.Application.StreamGroupChannelGroups.Commands;

namespace StreamMaster.Application.SMStreams.CommandsOld;

public record DeleteSMStreamRequest(string Id) : IRequest<bool> { }

public class DeleteSMStreamRequestValidator : AbstractValidator<DeleteSMStreamRequest>
{
    public DeleteSMStreamRequestValidator()
    {
        _ = RuleFor(v => v.Id).NotNull().NotEmpty();
    }
}

[LogExecutionTimeAspect]
public class DeleteSMStreamRequestHandler(ILogger<DeleteSMStreamRequest> logger, ISender sender, IRepositoryWrapper Repository, IPublisher Publisher)
    : IRequestHandler<DeleteSMStreamRequest, bool>
{
    public async Task<bool> Handle(DeleteSMStreamRequest request, CancellationToken cancellationToken)
    {
        SMStreamDto? stream = await Repository.SMStream.DeleteSMStreamById(request.Id, cancellationToken).ConfigureAwait(false);

        await Repository.SaveAsync().ConfigureAwait(false);
        if (stream != null)
        {
            ChannelGroup? cg = await Repository.ChannelGroup.GetChannelGroupByName(stream.Group);
            if (cg != null)
            {
                await sender.Send(new SyncStreamGroupChannelGroupByChannelIdRequest(cg.Id), cancellationToken).ConfigureAwait(false);
            }
            //await Publisher.Publish(new DeleteStreamEvent(stream.Id, cg), cancellationToken).ConfigureAwait(false);
            return true;
        }

        return false;
    }
}
