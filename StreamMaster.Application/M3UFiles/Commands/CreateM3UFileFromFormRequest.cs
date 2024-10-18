using Microsoft.AspNetCore.Http;

namespace StreamMaster.Application.M3UFiles.Commands;

[SMAPI(JustController = true, JustHub = true)]
[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public record CreateM3UFileFromFormRequest(string Name, int? MaxStreamCount, string? M3U8OutPutProfile, M3UKey? M3UKey, M3UField? M3UName, int? StartingChannelNumber, bool? AutoSetChannelNumbers, string? DefaultStreamGroupName, int? HoursToUpdate, bool? SyncChannels, IFormFile? FormFile, List<string>? VODTags) : IRequest<APIResponse>;

[LogExecutionTimeAspect]
public class CreateM3UFileFromFormRequestHandler(ILogger<CreateM3UFileFromFormRequest> Logger, IM3UToSMStreamsService m3UToSMStreamsService, IM3UFileService m3UFileService, IFileUtilService fileUtilService, ICacheManager CacheManager, IMessageService messageService, IDataRefreshService dataRefreshService, IRepositoryWrapper Repository, IPublisher Publisher)
    : IRequestHandler<CreateM3UFileFromFormRequest, APIResponse>
{
    public async Task<APIResponse> Handle(CreateM3UFileFromFormRequest request, CancellationToken cancellationToken)
    {
        if (request.FormFile == null)
        {
            return APIResponse.NotFound;
        }

        FileDefinition fd = FileDefinitions.M3U;
        string fullName = "";

        try
        {
            (M3UFile m3uFile, fullName) = m3UFileService.CreateM3UFile(request);
            m3uFile.LastDownloaded = File.GetLastWriteTime(fullName);

            await messageService.SendInfo($"Adding M3U '{request.Name}'");
            Logger.LogInformation("Adding M3U '{name}'", request.Name);

            (bool success, Exception? ex) = await fileUtilService.SaveFormFileAsync(request.FormFile!, fullName).ConfigureAwait(false);

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
                await messageService.SendError($"M3U '{request.Name}' format is not supported");
                return APIResponse.ErrorWithMessage($"M3U '{request.Name}' format is not supported");
            }

            await using (IAsyncEnumerator<SMStream?> streamEnumerator = streams.GetAsyncEnumerator(cancellationToken))
            {
                if (!await streamEnumerator.MoveNextAsync())
                {
                    fileUtilService.CleanUpFile(fullName);
                    Logger.LogCritical("Exception M3U '{name}' format is not supported or no streams", request.Name);
                    await messageService.SendError($"M3U '{request.Name}' format is not supported or no streams");
                    return APIResponse.ErrorWithMessage($"M3U '{request.Name}' format is not supported or no streams");
                }
            }

            m3uFile.MaxStreamCount = Math.Max(0, request.MaxStreamCount ?? 0);

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
        return APIResponse.NotFound;
    }
}