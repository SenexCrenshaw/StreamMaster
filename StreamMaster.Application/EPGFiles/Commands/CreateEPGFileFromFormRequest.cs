using Microsoft.AspNetCore.Http;

namespace StreamMaster.Application.EPGFiles.Commands;

[SMAPI(JustController = true, JustHub = true)]
[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public record CreateEPGFileFromFormRequest(IFormFile? FormFile, string Name, int EPGNumber, int? HoursToUpdate, int? TimeShift, string? Color)
    : IRequest<APIResponse>;

public class CreateEPGFileFromFormRequestHandler(ILogger<CreateEPGFileFromFormRequest> Logger, IEPGFileService ePGFileService, IFileUtilService fileUtilService, IDataRefreshService dataRefreshService, IMessageService messageService, IXmltv2Mxf xmltv2Mxf, IRepositoryWrapper Repository, IMapper Mapper, IPublisher Publisher)
    : IRequestHandler<CreateEPGFileFromFormRequest, APIResponse>
{
    public async Task<APIResponse> Handle(CreateEPGFileFromFormRequest request, CancellationToken cancellationToken)
    {
        if (request.FormFile?.Length <= 0)
        {
            return APIResponse.NotFound;
        }

        string fullName = "";

        try
        {
            (EPGFile epgFile, fullName) = await ePGFileService.CreateEPGFileAsync(request);
            epgFile.LastDownloadAttempt = SMDT.UtcNow;

            await messageService.SendInfo($"Adding EPG '{request.Name}'");
            Logger.LogInformation("Adding EPG '{name}'", request.Name);

            (bool success, Exception? ex) = await fileUtilService.SaveFormFileAsync(request.FormFile!, fullName).ConfigureAwait(false);

            if (!success)
            {
                fileUtilService.CleanUpFile(fullName);
                Logger.LogCritical("Exception M3U From URL '{ex}'", ex);
                await messageService.SendError("Exception M3U", ex?.Message);
                return APIResponse.ErrorWithMessage($"Exception M3U From URL '{ex}'");
            }

            XMLTV? tv = await xmltv2Mxf.ConvertToXMLTVAsync(fullName, epgFile.EPGNumber);
            if (tv == null)
            {
                fileUtilService.CleanUpFile(fullName);
                Logger.LogCritical("Exception EPG '{name}' format is not supported", request.Name);
                await messageService.SendError($"Exception EPG '{request.Name}' format is not supported");
                return APIResponse.ErrorWithMessage($"Could not get streams from M3U file {epgFile.Name}");
            }

            epgFile.ChannelCount = (tv.Channels?.Count) ?? 0;
            epgFile.ProgrammeCount = (tv.Programs?.Count) ?? 0;
            epgFile.LastDownloaded = SMDT.UtcNow;

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

            await messageService.SendError("Exception adding EPG", exception.Message);
            Logger.LogCritical("Exception EPG From Form '{exception}'", exception);
        }
        return APIResponse.Error;
    }
}