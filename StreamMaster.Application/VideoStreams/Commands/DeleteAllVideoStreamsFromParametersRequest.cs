using FluentValidation;

using StreamMaster.Domain.Pagination;
using StreamMaster.Domain.Repository;
using StreamMaster.Domain.Services;

using StreamMaster.Application.VideoStreams.Events;

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
public class DeleteAllVideoStreamsFromParametersRequestHandler : BaseMediatorRequestHandler, IRequestHandler<DeleteAllVideoStreamsFromParametersRequest, bool>
{

    public DeleteAllVideoStreamsFromParametersRequestHandler(ILogger<DeleteAllVideoStreamsFromParametersRequest> logger, IRepositoryWrapper repository, IMapper mapper,ISettingsService settingsService, IPublisher publisher, ISender sender, IHubContext<StreamMasterHub, IStreamMasterHub> hubContext, IMemoryCache memoryCache)
: base(logger, repository, mapper,settingsService, publisher, sender, hubContext, memoryCache) { }

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
