using Microsoft.AspNetCore.Http;

using StreamMaster.Domain.Color;
namespace StreamMaster.Application.EPGFiles.Commands;

[SMAPI(JustController = true, JustHub = true)]
[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public record CreateEPGFileFromFormRequest(IFormFile? FormFile, string Name, int EPGNumber, int? HoursToUpdate, int? TimeShift, string? Color)
    : IRequest<APIResponse>;

public class CreateEPGFileFromFormRequestHandler(ILogger<CreateEPGFileFromFormRequest> Logger, IFileUtilService fileUtilService, IDataRefreshService dataRefreshService, IMessageService messageService, IXmltv2Mxf xmltv2Mxf, IRepositoryWrapper Repository, IMapper Mapper, IPublisher Publisher)
    : IRequestHandler<CreateEPGFileFromFormRequest, APIResponse>
{
    public async Task<APIResponse> Handle(CreateEPGFileFromFormRequest request, CancellationToken cancellationToken)
    {
        if (request.FormFile?.Length <= 0)
        {
            return APIResponse.NotFound;
        }

        try
        {
            FileDefinition fd = FileDefinitions.EPG;

            string name = request.Name + fd.DefaultExtension;
            string compressedFileName = fileUtilService.CheckNeedsCompression(name);
            string fullName = Path.Combine(fd.DirectoryLocation, compressedFileName);

            int num = request.EPGNumber;

            if (num == 0 || await Repository.EPGFile.GetEPGFileByNumber(request.EPGNumber).ConfigureAwait(false) != null)
            {
                num = await Repository.EPGFile.GetNextAvailableEPGNumberAsync(cancellationToken).ConfigureAwait(false);
            }

            EPGFile epgFile = new()
            {
                Name = request.Name,
                Source = name,
                Color = request.Color ?? ColorHelper.GetColor(request.Name),
                EPGNumber = num,
                HoursToUpdate = request.HoursToUpdate ?? 72,
                TimeShift = request.TimeShift ?? 0
            };

            Logger.LogInformation("Adding EPG From Form: {fullName}", fullName);
            (bool success, Exception? ex) = await fileUtilService.SaveFormFileAsync(request.FormFile!, fullName).ConfigureAwait(false);
            if (success)
            {
                epgFile.LastDownloaded = File.GetLastWriteTime(fullName);
                epgFile.FileExists = true;
            }
            else
            {
                Logger.LogCritical("Exception EPG From Form {ex}", ex);
                await messageService.SendError("Exception EPG From Form", ex?.Message);
                return APIResponse.NotFound;
            }

            XMLTV? tv = xmltv2Mxf.ConvertToMxf(Path.Combine(FileDefinitions.EPG.DirectoryLocation, epgFile.Source), epgFile.EPGNumber);
            if (tv == null)
            {
                Logger.LogCritical("Exception EPG {fullName} format is not supported", fullName);
                await messageService.SendError("Exception EPG {fullName} format is not supported", fullName);
                //Bad EPG
                if (File.Exists(fullName))
                {
                    File.Delete(fullName);
                }
                return APIResponse.ErrorWithMessage($"Exception EPG {fullName} format is not supported");
            }

            epgFile.ChannelCount = (tv.Channels?.Count) ?? 0;
            epgFile.ProgrammeCount = (tv.Programs?.Count) ?? 0;

            Repository.EPGFile.CreateEPGFile(epgFile);
            _ = await Repository.SaveAsync().ConfigureAwait(false);
            epgFile.WriteJSON(Logger);

            EPGFileDto ret = Mapper.Map<EPGFileDto>(epgFile);
            await Publisher.Publish(new EPGFileAddedEvent(ret), cancellationToken).ConfigureAwait(false);

            await dataRefreshService.RefreshAllEPG();
            await messageService.SendSuccess("EPG '" + epgFile.Name + "' added successfully");

            return APIResponse.Success;
        }
        catch (Exception exception)
        {
            Logger.LogCritical("Exception EPG From Form {exception}", exception);
            return APIResponse.ErrorWithMessage(exception, "Exception EPG From Form");
        }
    }
}
