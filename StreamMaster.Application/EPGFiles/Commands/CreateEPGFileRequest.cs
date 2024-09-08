using StreamMaster.Domain.Color;

using System.Web;
namespace StreamMaster.Application.EPGFiles.Commands;

[SMAPI]
[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public record CreateEPGFileRequest(string Name, string FileName, int EPGNumber, int? TimeShift, int? HoursToUpdate, string? UrlSource, string? Color)
    : IRequest<APIResponse>;

public class CreateEPGFileRequestHandler(ILogger<CreateEPGFileRequest> Logger, IFileUtilService fileUtilService, IDataRefreshService dataRefreshService, IMessageService messageService, IXmltv2Mxf xmltv2Mxf, IRepositoryWrapper Repository, IMapper Mapper, IPublisher Publisher)
    : IRequestHandler<CreateEPGFileRequest, APIResponse>
{
    public async Task<APIResponse> Handle(CreateEPGFileRequest request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(request.UrlSource))
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
                Url = request.UrlSource,
                Source = name,
                Color = request.Color ?? ColorHelper.GetColor(request.Name),
                EPGNumber = num,
                HoursToUpdate = request.HoursToUpdate ?? 72,
                TimeShift = request.TimeShift ?? 0
            };

            string source = HttpUtility.UrlDecode(request.UrlSource);
            epgFile.Url = source;
            epgFile.LastDownloadAttempt = SMDT.UtcNow;

            await messageService.SendInfo($"Adding EPG '{request.Name}'");
            Logger.LogInformation("Adding EPG '{name}'", request.Name);

            (bool success, Exception? ex) = await fileUtilService.DownloadUrlAsync(source, fullName).ConfigureAwait(false);
            if (success)
            {
                epgFile.LastDownloaded = File.GetLastWriteTime(fullName);
                epgFile.FileExists = true;
            }
            else
            {
                ++epgFile.DownloadErrors;
                Logger.LogCritical("Exception EPG From URL {ex}", ex);
                await messageService.SendError("Exception EPG ", ex?.Message);
            }

            XMLTV? tv = xmltv2Mxf.ConvertToMxf(fullName, epgFile.EPGNumber);
            if (tv == null)
            {
                Logger.LogCritical("Exception EPG {fullName} format is not supported", fullName);
                await messageService.SendError("Exception EPG ", ex?.Message);
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
            epgFile.WriteJSON();

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
