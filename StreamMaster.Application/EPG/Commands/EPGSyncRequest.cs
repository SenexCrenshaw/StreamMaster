using StreamMaster.Application.Services;

namespace StreamMaster.Application.EPG.Commands;

[SMAPI]
[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public record EPGSyncRequest : IRequest<APIResponse>;

public class EPGSyncRequestHandler(IBackgroundTaskQueue backgroundTaskQueue)
    : IRequestHandler<EPGSyncRequest, APIResponse>
{
    public async Task<APIResponse> Handle(EPGSyncRequest command, CancellationToken cancellationToken)
    {
        await backgroundTaskQueue.EPGSync(cancellationToken);
        return APIResponse.Success;
    }
}