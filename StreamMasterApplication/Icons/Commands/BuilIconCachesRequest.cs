using MediatR;

namespace StreamMasterApplication.Icons.Commands;

public class BuilIconCachesRequest : IRequest
{ }

public class BuilIconCachesRequestHandler : IRequestHandler<BuilIconCachesRequest>
{
    private readonly ISender _sender;

    public BuilIconCachesRequestHandler(
        ISender sender
        )
    {
        _sender = sender;
    }

    public async Task Handle(BuilIconCachesRequest request, CancellationToken cancellationToken)
    {
        var settings = FileUtil.GetSetting();
        if (!settings.CacheIcons)
        {
            return;
        }
        _ = await _sender.Send(new BuildIconsCacheFromVideoStreamRequest(), cancellationToken).ConfigureAwait(false);
        _ = await _sender.Send(new BuildProgIconsCacheFromEPGsRequest(), cancellationToken).ConfigureAwait(false);
    }
}
