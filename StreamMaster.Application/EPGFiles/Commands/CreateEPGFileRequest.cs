using StreamMaster.Domain.Color;

using System.Web;
namespace StreamMaster.Application.EPGFiles.Commands;

[SMAPI]
[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public record CreateEPGFileRequest(string Name, string FileName, int EPGNumber, int? TimeShift, int? HoursToUpdate, string? UrlSource, string? Color)
    : IRequest<APIResponse>
{ }

public class CreateEPGFileRequestHandler(ILogger<CreateEPGFileRequest> Logger, IDataRefreshService dataRefreshService, IMessageService messageService, IXmltv2Mxf xmltv2Mxf, IRepositoryWrapper Repository, IMapper Mapper, IPublisher Publisher)
    : IRequestHandler<CreateEPGFileRequest, APIResponse>
{
    public async Task<APIResponse> Handle(CreateEPGFileRequest command, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(command.UrlSource))
        {
            return APIResponse.NotFound;
        }

        try
        {
            FileDefinition fd = FileDefinitions.EPG;

            string fullName = Path.Combine(fd.DirectoryLocation, command.Name + ".xmltv");
            string name = command.Name + ".xmltv";

            int num = command.EPGNumber;

            if (num == 0 || await Repository.EPGFile.GetEPGFileByNumber(command.EPGNumber).ConfigureAwait(false) != null)
            {
                num = await Repository.EPGFile.GetNextAvailableEPGNumberAsync(cancellationToken).ConfigureAwait(false);
            }

            EPGFile epgFile = new()
            {

                Name = command.Name,
                Url = command.UrlSource,
                Source = name,
                Color = command.Color ?? ColorHelper.GetColor(command.Name),
                EPGNumber = num,
                HoursToUpdate = command.HoursToUpdate ?? 72,
                TimeShift = command.TimeShift ?? 0
            };


            string source = HttpUtility.UrlDecode(command.UrlSource);
            epgFile.Url = source;
            epgFile.LastDownloadAttempt = SMDT.UtcNow;

            Logger.LogInformation("Add EPG From URL {command.UrlSource}", command.UrlSource);
            (bool success, Exception? ex) = await FileUtil.DownloadUrlAsync(source, fullName, cancellationToken).ConfigureAwait(false);
            if (success)
            {
                epgFile.LastDownloaded = File.GetLastWriteTime(fullName);
                epgFile.FileExists = true;
            }
            else
            {
                ++epgFile.DownloadErrors;
                Logger.LogCritical("Exception EPG From URL {ex}", ex);
                await messageService.SendError($"Exception EPG ", ex?.Message);
            }


            XMLTV? tv = xmltv2Mxf.ConvertToMxf(Path.Combine(FileDefinitions.EPG.DirectoryLocation, epgFile.Source), epgFile.EPGNumber);
            if (tv == null)
            {
                Logger.LogCritical("Exception EPG {fullName} format is not supported", fullName);
                await messageService.SendError($"Exception EPG ", ex?.Message);
                //Bad EPG
                if (File.Exists(fullName))
                {
                    File.Delete(fullName);
                }
                return APIResponse.ErrorWithMessage($"Exception EPG {fullName} format is not supported");
            }

            epgFile.ChannelCount = tv.Channels != null ? tv.Channels.Count : 0;
            epgFile.ProgrammeCount = tv.Programs != null ? tv.Programs.Count : 0;


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
            Logger.LogCritical("Exception EPG {exception}", exception);
            return APIResponse.ErrorWithMessage(exception, "Exception EPG");
        }

    }
}
