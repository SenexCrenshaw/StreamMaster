using Microsoft.AspNetCore.Http;

using StreamMaster.Domain.Color;
namespace StreamMaster.Application.EPGFiles.Commands;

[SMAPI(JustController = true, JustHub = true)]
[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public record CreateEPGFileFromFormRequest(IFormFile? FormFile, string Name, int EPGNumber, int? HoursToUpdate, int? TimeShift, string? Color)
    : IRequest<APIResponse>
{ }

public class CreateEPGFileFromFormRequestHandler(ILogger<CreateEPGFileFromFormRequest> Logger, IDataRefreshService dataRefreshService, IMessageService messageService, IXmltv2Mxf xmltv2Mxf, IRepositoryWrapper Repository, IMapper Mapper, IPublisher Publisher)
    : IRequestHandler<CreateEPGFileFromFormRequest, APIResponse>
{
    public async Task<APIResponse> Handle(CreateEPGFileFromFormRequest command, CancellationToken cancellationToken)
    {
        if (command.FormFile != null && command.FormFile.Length <= 0)
        {
            return APIResponse.NotFound;
        }

        try
        {
            FileDefinition fd = FileDefinitions.EPG;

            string fullName = Path.Combine(fd.DirectoryLocation, command.Name + ".xmltv");

            int num = command.EPGNumber;

            if (num == 0 || await Repository.EPGFile.GetEPGFileByNumber(command.EPGNumber).ConfigureAwait(false) != null)
            {
                num = await Repository.EPGFile.GetNextAvailableEPGNumberAsync(cancellationToken).ConfigureAwait(false);
            }

            EPGFile epgFile = new()
            {

                Name = command.Name,
                Source = command.Name + ".xmltv",
                Color = command.Color ?? ColorHelper.GetColor(command.Name),
                EPGNumber = num,
                HoursToUpdate = command.HoursToUpdate ?? 72,
                TimeShift = command.TimeShift ?? 0
            };


            fullName = Path.Combine(fd.DirectoryLocation, command.Name + ".xmltv");


            Logger.LogInformation("Adding EPG From Form: {fullName}", fullName);
            (bool success, Exception? ex) = await FormHelper.SaveFormFileAsync(command.FormFile!, fullName).ConfigureAwait(false);
            if (success)
            {
                epgFile.LastDownloaded = File.GetLastWriteTime(fullName);
                epgFile.FileExists = true;
            }
            else
            {
                Logger.LogCritical("Exception EPG From Form {ex}", ex);
                await messageService.SendError($"Exception EPG From Form", ex?.Message);
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
