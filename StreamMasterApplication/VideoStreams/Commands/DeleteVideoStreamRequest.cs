using FluentValidation;

using StreamMasterApplication.VideoStreams.Events;

namespace StreamMasterApplication.VideoStreams.Commands;

public record DeleteVideoStreamRequest(string Id) : IRequest<bool> { }

public class DeleteVideoStreamRequestValidator : AbstractValidator<DeleteVideoStreamRequest>
{
    public DeleteVideoStreamRequestValidator()
    {
        _ = RuleFor(v => v.Id).NotNull().NotEmpty();
    }
}

[LogExecutionTimeAspect]
public class DeleteVideoStreamRequestHandler(ILogger<DeleteVideoStreamRequest> logger, IRepositoryWrapper repository, IMapper mapper, ISettingsService settingsService, IPublisher publisher, ISender sender, IHubContext<StreamMasterHub, IStreamMasterHub> hubContext, IMemoryCache memoryCache) : BaseMediatorRequestHandler(logger, repository, mapper, settingsService, publisher, sender, hubContext, memoryCache), IRequestHandler<DeleteVideoStreamRequest, bool>
{
    public async Task<bool> Handle(DeleteVideoStreamRequest request, CancellationToken cancellationToken)
    {
        VideoStreamDto? stream = await Repository.VideoStream.DeleteVideoStreamById(request.Id, cancellationToken).ConfigureAwait(false);
        await Repository.SaveAsync().ConfigureAwait(false);
        if (stream != null)
        {
            await Publisher.Publish(new DeleteVideoStreamEvent(stream.Id), cancellationToken).ConfigureAwait(false);
            return true;
        }

        return false;
    }
}
