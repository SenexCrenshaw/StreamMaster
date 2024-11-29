namespace StreamMaster.Application.Logos.Commands;

public record CacheSMChannelLogosRequest : IRequest<DataResponse<bool>>;

[LogExecutionTimeAspect]
public class CacheSMChannelLogosRequestHandler(ILogoService logoService) : IRequestHandler<CacheSMChannelLogosRequest, DataResponse<bool>>
{
    public async Task<DataResponse<bool>> Handle(CacheSMChannelLogosRequest request, CancellationToken cancellationToken)
    {
        await logoService.CacheSMChannelLogosAsync(cancellationToken);
        return DataResponse.True;
    }
}
