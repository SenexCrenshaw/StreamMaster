namespace StreamMaster.Application.EPGFiles.Commands;

[SMAPI]
[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public record DeleteEPGFileRequest(bool DeleteFile, int Id) : IRequest<APIResponse> { }

public class DeleteEPGFileRequestHandler(ISchedulesDirectDataService schedulesDirectDataService, IRepositoryWrapper Repository, IPublisher Publisher)
    : IRequestHandler<DeleteEPGFileRequest, APIResponse>
{
    public async Task<APIResponse> Handle(DeleteEPGFileRequest request, CancellationToken cancellationToken = default)
    {
        EPGFileDto? epgFile = await Repository.EPGFile.DeleteEPGFile(request.Id);

        if (epgFile != null)
        {
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
            if (epgFile != null)
            {
                await Publisher.Publish(new EPGFileDeletedEvent(epgFile.Id), cancellationToken).ConfigureAwait(false);
            }
        }
        _ = await Repository.SaveAsync().ConfigureAwait(false);

        //schedulesDirect.ResetEPGCache();

        //MemoryCache.SetSyncForceNextRun(Extra: true);


        return APIResponse.Success;
    }
}
