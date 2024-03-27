using FluentValidation;

using StreamMaster.Application.VideoStreams.Events;
using StreamMaster.Domain.Pagination;
using StreamMaster.Domain.Requests;

namespace StreamMaster.Application.VideoStreams.Commands;

public record UpdateAllVideoStreamsFromParametersRequest(VideoStreamParameters Parameters, UpdateVideoStreamRequest UpdateRequest, List<int>? ChannelGroupIds) : IRequest<List<VideoStreamDto>> { }

public class UpdateAllVideoStreamsFromParametersRequestValidator : AbstractValidator<UpdateAllVideoStreamsFromParametersRequest>
{
    public UpdateAllVideoStreamsFromParametersRequestValidator()
    {
        _ = RuleFor(v => v.Parameters).NotNull().NotEmpty();
    }
}

[LogExecutionTimeAspect]
public class UpdateAllVideoStreamsFromParametersRequestHandler(ILogger<UpdateAllVideoStreamsFromParametersRequest> logger, IRepositoryWrapper Repository, IPublisher Publisher)
    : IRequestHandler<UpdateAllVideoStreamsFromParametersRequest, List<VideoStreamDto>>
{
    public async Task<List<VideoStreamDto>> Handle(UpdateAllVideoStreamsFromParametersRequest request, CancellationToken cancellationToken)
    {

        (List<VideoStreamDto> videoStreams, _) = await Repository.VideoStream.UpdateAllVideoStreamsFromParameters(request.Parameters, request.UpdateRequest, cancellationToken);
        if (videoStreams.Any())
        {
            await Publisher.Publish(new UpdateVideoStreamsEvent(videoStreams), cancellationToken).ConfigureAwait(false);
        }

        return videoStreams;
    }
}
