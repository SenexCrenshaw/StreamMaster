using MediatR;

namespace StreamMasterApplication.Icons.Commands;

public class CacheAllIconsRequest : IRequest
{ }

public class CacheAllIconsRequestHandler : IRequestHandler<CacheAllIconsRequest>
{
    private readonly ISender _sender;

    public CacheAllIconsRequestHandler(
        ISender sender
        )
    {
        _sender = sender;
    }

    public async Task Handle(CacheAllIconsRequest request, CancellationToken cancellationToken)
    {
        var settings = FileUtil.GetSetting();
        if (!settings.CacheIcons)
        {
            return;
        }
        _ = await _sender.Send(new CacheIconsFromVideoStreamsRequest(), cancellationToken).ConfigureAwait(false);
        _ = await _sender.Send(new CacheIconsFromEPGsRequest(), cancellationToken).ConfigureAwait(false);
    }
}
