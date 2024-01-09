namespace StreamMaster.Application.Icons.Commands;

public record BuildIconCachesRequest : IRequest { }

public class BuildIconCachesRequestHandler(ISender Sender, IMemoryCache memoryCache) : IRequestHandler<BuildIconCachesRequest>

{
    public async Task Handle(BuildIconCachesRequest request, CancellationToken cancellationToken)
    {

        if (!memoryCache.GetSetting().CacheIcons)
        {
            return;
        }
        _ = await Sender.Send(new BuildIconsCacheFromVideoStreamRequest(), cancellationToken).ConfigureAwait(false);
        //_ = await Sender.Send(new BuildProgIconsCacheFromEPGsRequest(), cancellationToken).ConfigureAwait(false);
    }
}
