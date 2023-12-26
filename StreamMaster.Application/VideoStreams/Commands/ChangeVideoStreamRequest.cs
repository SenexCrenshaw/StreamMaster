using FluentValidation;

namespace StreamMaster.Application.VideoStreams.Commands;

public record ChangeVideoStreamChannelRequest(string playingVideoStreamId, string newVideoStreamId) : IRequest
{
}

public class ChangeVideoStreamChannelRequestValidator : AbstractValidator<ChangeVideoStreamChannelRequest>
{
    public ChangeVideoStreamChannelRequestValidator()
    {
        _ = RuleFor(v => v.playingVideoStreamId).NotNull().NotEmpty();
        _ = RuleFor(v => v.newVideoStreamId).NotNull().NotEmpty();
    }
}

public class ChangeVideoStreamChannelRequestHandler(IChannelManager channelManager) : IRequestHandler<ChangeVideoStreamChannelRequest>
{
    public async Task Handle(ChangeVideoStreamChannelRequest request, CancellationToken cancellationToken)
    {
        await channelManager.ChangeVideoStreamChannel(request.playingVideoStreamId, request.newVideoStreamId);
    }
}
