using FluentValidation;

using StreamMaster.Application.VideoStreams.Events;
using StreamMaster.Domain.Pagination;

namespace StreamMaster.Application.SMStreams.CommandsOld;

public record DeleteAllSMStreamsFromParametersRequest(SMStreamParameters Parameters) : IRequest<bool> { }

public class DeleteAllSMStreamsFromParametersRequestValidator : AbstractValidator<DeleteAllSMStreamsFromParametersRequest>
{
    public DeleteAllSMStreamsFromParametersRequestValidator()
    {
        _ = RuleFor(v => v.Parameters).NotNull().NotEmpty();
    }
}

[LogExecutionTimeAspect]
public class DeleteAllSMStreamsFromParametersRequestHandler(ILogger<DeleteAllSMStreamsFromParametersRequest> logger, IRepositoryWrapper Repository, IPublisher Publisher)
    : IRequestHandler<DeleteAllSMStreamsFromParametersRequest, bool>
{
    public async Task<bool> Handle(DeleteAllSMStreamsFromParametersRequest request, CancellationToken cancellationToken)
    {
        IEnumerable<string> ret = await Repository.SMStream.DeleteAllSMStreamsFromParameters(request.Parameters, cancellationToken).ConfigureAwait(false);
        if (ret.Any())
        {
            await Publisher.Publish(new DeleteVideoStreamsEvent(ret), cancellationToken).ConfigureAwait(false);
            return true;
        }

        return false;
    }
}
