using FluentValidation;

using StreamMaster.Application.StreamGroupChannelGroupLinks.Commands;
using StreamMaster.Application.VideoStreams.Events;

namespace StreamMaster.Application.VideoStreams.Commands;

public record DeleteVideoStreamRequest(string Id) : IRequest<bool> { }

public class DeleteVideoStreamRequestValidator : AbstractValidator<DeleteVideoStreamRequest>
{
    public DeleteVideoStreamRequestValidator()
    {
        _ = RuleFor(v => v.Id).NotNull().NotEmpty();
    }
}

[LogExecutionTimeAspect]
public class DeleteVideoStreamRequestHandler(ILogger<DeleteVideoStreamRequest> logger, ISender sender, IRepositoryWrapper Repository, IPublisher Publisher)
    : IRequestHandler<DeleteVideoStreamRequest, bool>
{
    public async Task<bool> Handle(DeleteVideoStreamRequest request, CancellationToken cancellationToken)
    {
        VideoStreamDto? stream = await Repository.VideoStream.DeleteVideoStreamById(request.Id, cancellationToken).ConfigureAwait(false);

        await Repository.SaveAsync().ConfigureAwait(false);
        if (stream != null)
        {
            ChannelGroup? cg = await Repository.ChannelGroup.GetChannelGroupByName(stream.User_Tvg_group);
            await sender.Send(new SyncStreamGroupChannelGroupByChannelIdRequest(cg.Id), cancellationToken).ConfigureAwait(false);

            await Publisher.Publish(new DeleteVideoStreamEvent(stream.Id, cg), cancellationToken).ConfigureAwait(false);
            return true;
        }

        return false;
    }
}
