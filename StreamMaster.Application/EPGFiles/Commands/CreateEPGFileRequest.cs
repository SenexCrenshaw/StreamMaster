using System.Web;

namespace StreamMaster.Application.EPGFiles.Commands;

[SMAPI]
[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public record CreateEPGFileRequest(string Name, string FileName, int EPGNumber, int? TimeShift, int? HoursToUpdate, string? UrlSource, string? Color)
    : IRequest<APIResponse>;

public class CreateEPGFileRequestHandler(ILogger<CreateEPGFileRequest> Logger, IEPGFileService ePGFileService, IFileUtilService fileUtilService, IDataRefreshService dataRefreshService, IMessageService messageService, IRepositoryWrapper Repository, IMapper Mapper, IPublisher Publisher)
    : IRequestHandler<CreateEPGFileRequest, APIResponse>
{
    public async Task<APIResponse> Handle(CreateEPGFileRequest request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(request.UrlSource))
        {
            return APIResponse.NotFound;
        }

        string fullName = "";

        try
        {
            (EPGFile epgFile, fullName) = await ePGFileService.CreateEPGFileAsync(request);

            await messageService.SendInfo($"Adding EPG '{request.Name}'");
            Logger.LogInformation("Adding EPG '{name}'", request.Name);

            string source = HttpUtility.UrlDecode(request.UrlSource);
            epgFile.LastDownloadAttempt = SMDT.UtcNow;

            (bool success, Exception? ex) = await fileUtilService.DownloadUrlAsync(source, fullName).ConfigureAwait(false);

            if (!success)
            {
                fileUtilService.CleanUpFile(fullName);
                Logger.LogCritical("Exception M3U From URL '{ex}'", ex);
                await messageService.SendError("Exception M3U", ex?.Message);
                return APIResponse.ErrorWithMessage($"Exception M3U From URL '{ex}'");
            }

            (int channelCount, int programCount) = await fileUtilService.ReadXmlCountsFromFileAsync(fullName, epgFile.EPGNumber);
            if (channelCount == -1)
            {
                fileUtilService.CleanUpFile(fullName);
                Logger.LogCritical("Exception EPG '{name}' format is not supported", request.Name);
                await messageService.SendError($"Exception EPG '{request.Name}' format is not supported");
                return APIResponse.ErrorWithMessage($"Could not get streams from M3U file {epgFile.Name}");
            }

            epgFile.LastDownloaded = File.GetLastWriteTime(fullName);
            epgFile.FileExists = true;
            epgFile.ChannelCount = channelCount;
            epgFile.ProgrammeCount = programCount;

            Repository.EPGFile.CreateEPGFile(epgFile);
            _ = await Repository.SaveAsync().ConfigureAwait(false);
            epgFile.WriteJSON();

            EPGFileDto ret = Mapper.Map<EPGFileDto>(epgFile);
            await Publisher.Publish(new EPGFileAddedEvent(ret), cancellationToken).ConfigureAwait(false);

            await dataRefreshService.RefreshAllEPG();

            await messageService.SendSuccess("EPG '" + epgFile.Name + "' added successfully");
            return APIResponse.Success;
        }
        catch (Exception exception)
        {
            if (!string.IsNullOrEmpty(fullName))
            {
                fileUtilService.CleanUpFile(fullName);
            }

            await messageService.SendError("Exception adding M3U", exception.Message);
            Logger.LogCritical("Exception M3U From Form '{exception}'", exception);
        }
        return APIResponse.Error;
    }
}