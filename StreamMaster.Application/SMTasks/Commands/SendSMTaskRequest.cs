namespace StreamMaster.Application.SMTasks.Commands;

[SMAPI]
[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public record SendSMTasksRequest(List<SMTask> SMTasks) : IRequest<APIResponse>;

internal class SendSMTasksRequestHandler(IDataRefreshService dataRefreshService)
    : IRequestHandler<SendSMTasksRequest, APIResponse>
{
    public async Task<APIResponse> Handle(SendSMTasksRequest request, CancellationToken cancellationToken)
    {
        await dataRefreshService.SendSMTasks(request.SMTasks).ConfigureAwait(false);
        return APIResponse.Success;
    }
}
