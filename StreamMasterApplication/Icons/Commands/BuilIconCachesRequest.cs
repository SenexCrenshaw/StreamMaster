using MediatR;

namespace StreamMasterApplication.Icons.Commands;

public class BuildIconCachesRequest : IRequest
{ }

public class BuildIconCachesRequestHandler : IRequestHandler<BuildIconCachesRequest>
{
    private readonly ISender _sender;

    public BuildIconCachesRequestHandler(
        ISender sender
        )
    {
        _sender = sender;
    }

    public async Task Handle(BuildIconCachesRequest request, CancellationToken cancellationToken)
    {
        Setting settings = FileUtil.GetSetting();
        if (!settings.CacheIcons)
        {
            return;
        }
        _ = await _sender.Send(new BuildIconsCacheFromVideoStreamRequest(), cancellationToken).ConfigureAwait(false);
        _ = await _sender.Send(new BuildProgIconsCacheFromEPGsRequest(), cancellationToken).ConfigureAwait(false);
    }
}
