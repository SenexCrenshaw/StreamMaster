using FluentValidation;

using StreamMaster.Application.VideoStreams.Events;
using StreamMaster.Domain.Pagination;

namespace StreamMaster.Application.VideoStreams.Commands;

public record DeleteAllVideoStreamsFromParametersRequest(VideoStreamParameters Parameters) : IRequest<bool> { }

public class DeleteAllVideoStreamsFromParametersRequestValidator : AbstractValidator<DeleteAllVideoStreamsFromParametersRequest>
{
    public DeleteAllVideoStreamsFromParametersRequestValidator()
    {
        _ = RuleFor(v => v.Parameters).NotNull().NotEmpty();
    }
}

[LogExecutionTimeAspect]
public class DeleteAllVideoStreamsFromParametersRequestHandler(ILogger<DeleteAllVideoStreamsFromParametersRequest> logger, IRepositoryWrapper Repository, IPublisher Publisher)
    : IRequestHandler<DeleteAllVideoStreamsFromParametersRequest, bool>
{
    public async Task<bool> Handle(DeleteAllVideoStreamsFromParametersRequest request, CancellationToken cancellationToken)
    {
        IEnumerable<string> ret = await Repository.VideoStream.DeleteAllVideoStreamsFromParameters(request.Parameters, cancellationToken).ConfigureAwait(false);
        if (ret.Any())
        {
            await Publisher.Publish(new DeleteVideoStreamsEvent(ret), cancellationToken).ConfigureAwait(false);
            return true;
        }

        return false;
    }
}
