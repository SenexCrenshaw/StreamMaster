using FluentValidation;

using MediatR;

namespace StreamMasterApplication.VideoStreams.Commands;

public record ChangeVideoStreamChannelRequest(int playingVideoStreamId, int newVideoStreamId) : IRequest
{
}

public class ChangeVideoStreamChannelRequestValidator : AbstractValidator<ChangeVideoStreamChannelRequest>
{
    public ChangeVideoStreamChannelRequestValidator()
    {
        _ = RuleFor(v => v.playingVideoStreamId).NotNull().GreaterThan(0);
        _ = RuleFor(v => v.newVideoStreamId).NotNull().GreaterThan(0);
    }
}

public class ChangeVideoStreamChannelRequestHandler : IRequestHandler<ChangeVideoStreamChannelRequest>
{
    private readonly IChannelManager _channelManager;

    public ChangeVideoStreamChannelRequestHandler(IChannelManager channelManager)
    {
        _channelManager = channelManager;
    }

    public async Task Handle(ChangeVideoStreamChannelRequest request, CancellationToken cancellationToken)
    {
        await _channelManager.ChangeVideoStreamChannel(request.playingVideoStreamId, request.newVideoStreamId);
    }
}
