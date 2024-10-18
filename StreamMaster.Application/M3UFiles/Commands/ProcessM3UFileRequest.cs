namespace StreamMaster.Application.M3UFiles.Commands;

[SMAPI]
[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public record ProcessM3UFileRequest(int M3UFileId, bool ForceRun = false) : IRequest<APIResponse>;

internal class ProcessM3UFileRequestHandler(ILogger<ProcessM3UFileRequest> logger, IM3UFileService m3UFileService, IChannelGroupService channelGroupService, IMessageService messageService, IDataRefreshService dataRefreshService)
    : IRequestHandler<ProcessM3UFileRequest, APIResponse>
{
    public async Task<APIResponse> Handle(ProcessM3UFileRequest request, CancellationToken cancellationToken)
    {
        try
        {
            M3UFile? m3uFile = await m3UFileService.GetM3UFileAsync(request.M3UFileId);
            if (m3uFile == null)
            {
                await messageService.SendError("Process M3U Not Found");
                return APIResponse.NotFound;
            }

            m3uFile = await m3UFileService.ProcessM3UFile(request.M3UFileId, request.ForceRun).ConfigureAwait(false);
            if (m3uFile == null)
            {
                await messageService.SendError("Process M3U Not Found");
                return APIResponse.NotFound;
            }

            await channelGroupService.UpdateChannelGroupCountsRequestAsync();

            await dataRefreshService.RefreshAllM3U();

            await messageService.SendSuccess("Processed M3U '" + m3uFile.Name + "' successfully");
            return APIResponse.Success;
        }
        catch (Exception ex)
        {
            logger.LogCritical(ex, "Process M3U Error");
            await messageService.SendError("Error Processing M3U", ex.Message);
            return APIResponse.NotFound;
        }
    }
}
