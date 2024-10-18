namespace StreamMaster.Application.General.Commands;

[SMAPI]
[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public record SetTestTaskRequest(int DelayInSeconds) : IRequest<APIResponse>;

public class SetTestTaskRequestHandler() : IRequestHandler<SetTestTaskRequest, APIResponse>
{
    public async Task<APIResponse> Handle(SetTestTaskRequest request, CancellationToken cancellationToken)
    {
        await Task.Delay(1000 * request.DelayInSeconds);
        return APIResponse.Ok;
    }
}
