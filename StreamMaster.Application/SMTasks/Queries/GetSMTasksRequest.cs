using StreamMaster.Application.Services;

namespace StreamMaster.Application.SMTasks.Queries;

[SMAPI]
[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public record GetSMTasksRequest() : IRequest<DataResponse<List<SMTask>>>;

internal class GetSMTasksRequestHandler(IBackgroundTaskQueue backgroundTaskQueue)
    : IRequestHandler<GetSMTasksRequest, DataResponse<List<SMTask>>>
{
    public async Task<DataResponse<List<SMTask>>> Handle(GetSMTasksRequest request, CancellationToken cancellationToken)
    {
        List<SMTask> smTasks = backgroundTaskQueue.GetSMTasks();

        return await Task.FromResult(DataResponse<List<SMTask>>.Success(smTasks));
    }
}
