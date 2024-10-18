using StreamMaster.Ring.API;

namespace StreamMaster.Application.Ring.Commands;

[SMAPI(JustController = true, JustHub = true)]
[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public record RingAuthenticateMFA(string MFACode) : IRequest<APIResponse>;

public class RingAuthenticateMFAHandler(ILogger<RingAuthenticateMFA> logger)
: IRequestHandler<RingAuthenticateMFA, APIResponse>
{
    public async Task<APIResponse> Handle(RingAuthenticateMFA request, CancellationToken cancellationToken)
    {
        try
        {
            Session a = new("a", "b");
            await a.Authenticate(twoFactorAuthCode: request.MFACode);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error authenticating to Ring");
        }

        return APIResponse.Ok;
    }
}