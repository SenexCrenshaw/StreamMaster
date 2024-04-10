using System.Web;

namespace StreamMaster.Application.M3UFiles.Commands;

[SMAPI]
[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public record CreateM3UFileRequest(string Name, int MaxStreamCount, string? UrlSource, bool? OverWriteChannels, int? StartingChannelNumber, List<string>? VODTags) : IRequest<APIResponse> { }

[LogExecutionTimeAspect]
public class CreateM3UFileRequestHandler(ILogger<CreateM3UFileRequest> Logger, IMessageService messageService, IHubContext<StreamMasterHub, IStreamMasterHub> hubContext, IRepositoryWrapper Repository, IPublisher Publisher)
    : IRequestHandler<CreateM3UFileRequest, APIResponse>
{
    public async Task<APIResponse> Handle(CreateM3UFileRequest command, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(command.UrlSource))
        {

            return APIResponse.NotFound;
        }

        try
        {
            FileDefinition fd = FileDefinitions.M3U;
            string fullName = Path.Combine(fd.DirectoryLocation, command.Name + fd.FileExtension);

            M3UFile m3UFile = new()
            {
                Name = command.Name,
                Source = command.Name + fd.FileExtension,
                StartingChannelNumber = command.StartingChannelNumber == null ? 1 : (int)command.StartingChannelNumber,
                OverwriteChannelNumbers = command.OverWriteChannels != null && (bool)command.OverWriteChannels,
                VODTags = command.VODTags ?? [],
            };

            string source = HttpUtility.UrlDecode(command.UrlSource);
            m3UFile.Url = source;
            m3UFile.LastDownloadAttempt = SMDT.UtcNow;

            Logger.LogInformation("Add M3U From URL '{command.UrlSource}'", command.UrlSource);
            (bool success, Exception? ex) = await FileUtil.DownloadUrlAsync(source, fullName, cancellationToken).ConfigureAwait(false);
            if (success)
            {
                m3UFile.LastDownloaded = File.GetLastWriteTime(fullName);
                m3UFile.FileExists = true;
            }
            else
            {
                ++m3UFile.DownloadErrors;

                Logger.LogCritical("Exception M3U From URL '{ex}'", ex);
                await messageService.SendError($"Exception M3U", ex?.Message);

            }

            m3UFile.MaxStreamCount = Math.Max(0, command.MaxStreamCount);

            List<VideoStream>? streams = await m3UFile.GetVideoStreamsFromM3U(Logger).ConfigureAwait(false);
            if (streams == null || streams.Count == 0)
            {
                Logger.LogCritical("Exception M3U '{name}' format is not supported", command.Name);
                await messageService.SendError($"Exception M3U '{command.Name}' format is not supported");
                //Bad M3U
                if (File.Exists(fullName))
                {
                    File.Delete(fullName);
                }
                string urlPath = Path.GetFileNameWithoutExtension(fullName) + ".url";
                if (File.Exists(urlPath))
                {
                    File.Delete(urlPath);
                }
                return APIResponse.NotFound;
            }

            Repository.M3UFile.CreateM3UFile(m3UFile);
            _ = await Repository.SaveAsync().ConfigureAwait(false);

            m3UFile.WriteJSON();

            await hubContext.Clients.All.DataRefresh("GetPagedM3UFiles").ConfigureAwait(false);
            await Publisher.Publish(new M3UFileProcessEvent(m3UFile.Id, false), cancellationToken).ConfigureAwait(false);

            await messageService.SendSuccess("M3U '" + m3UFile.Name + "' added successfully");

            return APIResponse.Success;
        }
        catch (Exception exception)
        {
            await messageService.SendError("Exception adding M3U", exception.Message);
            Logger.LogCritical("Exception M3U From Form '{exception}'", exception);
        }
        return APIResponse.NotFound;
    }
}