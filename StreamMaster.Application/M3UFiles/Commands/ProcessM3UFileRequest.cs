namespace StreamMaster.Application.M3UFiles.Commands;

[SMAPI(JustHub: true, IsTask: true)]
[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public record ProcessM3UFileRequest(int M3UFileId, bool ForceRun = false) : IRequest<APIResponse>;

internal class ProcessM3UFileRequestHandler(ILogger<ProcessM3UFileRequest> logger, IMessageService messageSevice, IRepositoryWrapper Repository, IHubContext<StreamMasterHub, IStreamMasterHub> hubContext)
    : IRequestHandler<ProcessM3UFileRequest, APIResponse>
{
    public async Task<APIResponse> Handle(ProcessM3UFileRequest request, CancellationToken cancellationToken)
    {
        try
        {
            M3UFile? m3uFile = await Repository.M3UFile.ProcessM3UFile(request.M3UFileId, request.ForceRun).ConfigureAwait(false);
            if (m3uFile == null)
            {
                await messageSevice.SendError("Process M3U Not Found");
                return APIResponse.NotFound;
            }

            await hubContext.Clients.All.DataRefresh("GetPagedM3UFiles").ConfigureAwait(false);
            await hubContext.Clients.All.DataRefresh("GetPagedSMStreams").ConfigureAwait(false);
            await hubContext.Clients.All.DataRefresh("GetSMChannelStreams").ConfigureAwait(false);

            await hubContext.Clients.All.DataRefresh("GetPagedChannelGroups").ConfigureAwait(false);

            await messageSevice.SendSuccess("Processed M3U '" + m3uFile.Name + "' successfully");
            return APIResponse.Success;
        }
        catch (Exception ex)
        {
            logger.LogCritical(ex, "Process M3U Error");
            await messageSevice.SendError("Error Processing M3U", ex.Message);
            return APIResponse.NotFound; ;
        }
    }
}
