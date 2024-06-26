namespace StreamMaster.Application.EPGFiles.Commands;

[SMAPI]
[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public record DeleteEPGFileRequest(bool DeleteFile, int Id) : IRequest<APIResponse> { }

public class DeleteEPGFileRequestHandler(ILogger<DeleteEPGFileRequest> logger, IDataRefreshService dataRefreshService, ISchedulesDirectDataService schedulesDirectDataService, IMessageService messageService, IRepositoryWrapper Repository, IPublisher Publisher)
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
                string fullName = Path.Combine(FileDefinitions.EPG.DirectoryLocation, epgFile.Source);
                if (File.Exists(fullName))
                {
                    File.Delete(fullName);
                    string txtName = Path.Combine(FileDefinitions.EPG.DirectoryLocation, Path.GetFileNameWithoutExtension(epgFile.Source) + ".json");
                    if (File.Exists(txtName))
                    {
                        File.Delete(txtName);
                    }
                    txtName = Path.Combine(FileDefinitions.EPG.DirectoryLocation, Path.GetFileNameWithoutExtension(epgFile.Source) + ".url");
                    if (File.Exists(txtName))
                    {
                        File.Delete(txtName);
                    }
                }
                else
                {
                    //_logger.LogError("DeleteEPGFile File {fulleName} does not exist", fulleName);
                }
            }
            schedulesDirectDataService.Reset(epgFile.Id);


            await Publisher.Publish(new EPGFileDeletedEvent(epgFile.Id), cancellationToken).ConfigureAwait(false);

            await Repository.SaveAsync().ConfigureAwait(false);

            await messageService.SendSuccess($"Deleted EPG {epgFile.Name}");
            await dataRefreshService.RefreshAllEPG();
            return APIResponse.Success;

        }
        catch (Exception ex)
        {
            logger.LogError(ex, "DeleteM3UFileRequest {request}", request);
            await messageService.SendError("Exception deleting M3U", ex.Message);
            return APIResponse.NotFound;
        }
    }
}
