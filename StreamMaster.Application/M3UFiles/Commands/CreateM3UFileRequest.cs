using System.Web;

namespace StreamMaster.Application.M3UFiles.Commands;

[SMAPI]
[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public record CreateM3UFileRequest(string Name, int MaxStreamCount, string? M3U8OutPutProfile, M3UKey? M3UKey, M3UField? M3UName, string? DefaultStreamGroupName, string? UrlSource, bool? SyncChannels, int? HoursToUpdate, int? StartingChannelNumber, bool? AutoSetChannelNumbers, List<string>? VODTags) : IRequest<APIResponse>;

[LogExecutionTimeAspect]
public class CreateM3UFileRequestHandler(ILogger<CreateM3UFileRequest> Logger, IM3UFileService m3UFileService, IM3UToSMStreamsService m3UToSMStreamsService, IFileUtilService fileUtilService, ICacheManager CacheManager, IMessageService messageService, IDataRefreshService dataRefreshService, IRepositoryWrapper Repository, IPublisher Publisher)
    : IRequestHandler<CreateM3UFileRequest, APIResponse>
{
    public async Task<APIResponse> Handle(CreateM3UFileRequest request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(request.UrlSource))
        {
            return APIResponse.NotFound;
        }

        string fullName = "";

        try
        {
            (M3UFile m3uFile, fullName) = m3UFileService.CreateM3UFile(request);

            await messageService.SendInfo($"Adding M3U '{request.Name}'");
            Logger.LogInformation("Adding M3U '{name}'", request.Name);

            string source = HttpUtility.UrlDecode(request.UrlSource);
            m3uFile.LastDownloadAttempt = SMDT.UtcNow;

            (bool success, Exception? ex) = await fileUtilService.DownloadUrlAsync(source, fullName).ConfigureAwait(false);
            if (!success)
            {
                fileUtilService.CleanUpFile(fullName);
                Logger.LogCritical("Exception M3U From URL '{ex}'", ex);
                await messageService.SendError("Exception M3U", ex?.Message);
                return APIResponse.ErrorWithMessage($"Exception M3U From URL '{ex}'");
            }

            IAsyncEnumerable<SMStream?> streams = m3UToSMStreamsService.GetSMStreamsFromM3U(m3uFile);
            if (streams == null)
            {
                fileUtilService.CleanUpFile(fullName);
                Logger.LogCritical("Exception M3U '{name}' format is not supported", request.Name);
                await messageService.SendError($"Exception M3U '{request.Name}' format is not supported");
                return APIResponse.ErrorWithMessage($"Could not get streams from M3U file {m3uFile.Name}");
            }

            await using (IAsyncEnumerator<SMStream?> streamEnumerator = streams.GetAsyncEnumerator(cancellationToken))
            {
                if (!await streamEnumerator.MoveNextAsync())
                {
                    // If there are no entries in the stream, clean up and error out
                    fileUtilService.CleanUpFile(fullName);
                    Logger.LogCritical("Exception M3U '{name}' contains no streams", request.Name);
                    await messageService.SendError($"M3U '{request.Name}' contains no streams");
                    return APIResponse.ErrorWithMessage($"M3U file {m3uFile.Name} contains no streams");
                }
            }

            m3uFile.Url = source;
            m3uFile.LastDownloaded = File.GetLastWriteTime(fullName);
            m3uFile.FileExists = true;
            m3uFile.MaxStreamCount = Math.Max(0, request.MaxStreamCount);

            Repository.M3UFile.CreateM3UFile(m3uFile);
            _ = await Repository.SaveAsync().ConfigureAwait(false);
            CacheManager.M3UMaxStreamCounts.AddOrUpdate(m3uFile.Id, m3uFile.MaxStreamCount, (_, _) => m3uFile.MaxStreamCount);

            m3uFile.WriteJSON();

            await dataRefreshService.RefreshAllM3U();

            await Publisher.Publish(new M3UFileProcessEvent(m3uFile.Id, false), cancellationToken).ConfigureAwait(false);

            await messageService.SendSuccess("M3U '" + m3uFile.Name + "' added successfully");

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