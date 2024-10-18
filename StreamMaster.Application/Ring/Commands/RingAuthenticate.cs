using StreamMaster.Ring.API;

namespace StreamMaster.Application.Ring.Commands;

[SMAPI(JustController = true, JustHub = true)]
[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public record RingAuthenticate() : IRequest<APIResponse>;

public class RingAuthenticateHandler(ILogger<RingAuthenticate> logger)
: IRequestHandler<RingAuthenticate, APIResponse>
{
    public async Task<APIResponse> Handle(RingAuthenticate request, CancellationToken cancellationToken)
    {
        try
        {
            Session a = new("a", "nx");
            await a.Authenticate();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error authenticating to Ring");
        }

        return APIResponse.Ok;
    }
}