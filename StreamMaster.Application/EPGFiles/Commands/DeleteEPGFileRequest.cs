namespace StreamMaster.Application.EPGFiles.Commands;

[SMAPI]
[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public record DeleteEPGFileRequest(bool DeleteFile, int Id) : IRequest<APIResponse>;

public class DeleteEPGFileRequestHandler(ILogger<DeleteEPGFileRequest> logger, ICacheManager cacheManager, IEPGFileService ePGFileService, IFileUtilService fileUtilService, IDataRefreshService dataRefreshService, ISchedulesDirectDataService schedulesDirectDataService, IMessageService messageService, IRepositoryWrapper Repository, IPublisher Publisher)
    : IRequestHandler<DeleteEPGFileRequest, APIResponse>
{
    public async Task<APIResponse> Handle(DeleteEPGFileRequest request, CancellationToken cancellationToken = default)
    {
        EPGFileDto? epgFile = await Repository.EPGFile.DeleteEPGFile(request.Id).ConfigureAwait(false);
        if (epgFile == null)
        {
            await messageService.SendError("EPG file not found");
            return APIResponse.NotFound;
        }
        try
        {
            await Repository.EPGFile.DeleteEPGFile(epgFile.Id);

            if (request.DeleteFile)
            {
                string? fullName = ePGFileService.GetFileName(epgFile.Name).fullName;
                fileUtilService.CleanUpFile(fullName);
            }

            schedulesDirectDataService.Reset(epgFile.Id);

            await Publisher.Publish(new EPGFileDeletedEvent(epgFile.Id), cancellationToken).ConfigureAwait(false);

            await Repository.SaveAsync().ConfigureAwait(false);
            cacheManager.ClearEPGDataByEPGNumber(epgFile.EPGNumber);
            await messageService.SendSuccess($"Deleted EPG {epgFile.Name}");
            await dataRefreshService.RefreshAllEPG();
            return APIResponse.Success;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "DeleteM3UFileRequest {request}", request);
            await messageService.SendError("Exception deleting EPG", ex.Message);
            return APIResponse.NotFound;
        }
    }
}
