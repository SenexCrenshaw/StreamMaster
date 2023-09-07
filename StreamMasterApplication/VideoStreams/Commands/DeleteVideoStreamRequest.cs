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

public class DeleteVideoStreamRequestHandler(ILogger<DeleteVideoStreamRequest> logger, IRepositoryWrapper repository, IMapper mapper, IPublisher publisher, ISender sender, IHubContext<StreamMasterHub, IStreamMasterHub> hubContext) : BaseMediatorRequestHandler(logger, repository, mapper, publisher, sender, hubContext), IRequestHandler<DeleteVideoStreamRequest, bool>
{
    public async Task<bool> Handle(DeleteVideoStreamRequest request, CancellationToken cancellationToken)
    {
        VideoStreamDto? stream = await Repository.VideoStream.DeleteVideoStreamAsync(request.Id, cancellationToken).ConfigureAwait(false);
        if (stream != null)
        {
            await Publisher.Publish(new DeleteVideoStreamEvent(stream.Id), cancellationToken).ConfigureAwait(false);
            return true;
        }

        return false;
    }
}
