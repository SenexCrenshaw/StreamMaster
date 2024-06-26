using StreamMaster.Application.Services;

namespace StreamMaster.Application.General.Queries;

[SMAPI]
[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public record GetTaskIsRunningRequest : IRequest<DataResponse<bool>>;

internal class GetTaskIsRunningRequestHandler(IBackgroundTaskQueue backgroundTaskQueue) : IRequestHandler<GetTaskIsRunningRequest, DataResponse<bool>>
{
    public Task<DataResponse<bool>> Handle(GetTaskIsRunningRequest request, CancellationToken cancellationToken)
    {
        return Task.FromResult(backgroundTaskQueue.IsRunning ? DataResponse.True : DataResponse.False);
    }
}
