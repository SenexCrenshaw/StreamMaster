namespace StreamMaster.Application.SMStreams.Commands;

[SMAPI]
[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public record DeleteSMStreamRequest(string SMStreamId) : IRequest<APIResponse>;

[LogExecutionTimeAspect]
public class DeleteSMStreamRequestHandler(ILogger<DeleteSMStreamRequest> Logger, IMessageService messageService, IDataRefreshService dataRefreshService, IRepositoryWrapper Repository)
    : IRequestHandler<DeleteSMStreamRequest, APIResponse>
{
    public async Task<APIResponse> Handle(DeleteSMStreamRequest request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(request.SMStreamId))
        {
            return APIResponse.NotFound;
        }

        var smStream = await Repository.SMStream.FirstOrDefaultAsync(a => a.Id == request.SMStreamId).ConfigureAwait(false);
        if (smStream == null)
        {
            return APIResponse.NotFound;
        }


        Repository.SMStream.Delete(smStream);
        await Repository.SaveAsync().ConfigureAwait(false);

        await dataRefreshService.RefreshSMStreams();

        await messageService.SendSuccess("Stream deleted", $"Stream '{smStream.Name}' successfully");
        return APIResponse.Success;
    }

}
