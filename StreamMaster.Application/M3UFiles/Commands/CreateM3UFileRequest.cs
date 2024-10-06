using System.Web;

namespace StreamMaster.Application.M3UFiles.Commands;

[SMAPI]
[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public record CreateM3UFileRequest(string Name, int MaxStreamCount, M3UKey? M3UKey, string? DefaultStreamGroupName, string? UrlSource, bool? SyncChannels, int? HoursToUpdate, int? StartingChannelNumber, bool? AutoSetChannelNumbers, List<string>? VODTags) : IRequest<APIResponse>;

[LogExecutionTimeAspect]
public class CreateM3UFileRequestHandler(ILogger<CreateM3UFileRequest> Logger, IFileUtilService fileUtilService, ICacheManager CacheManager, IMessageService messageService, IDataRefreshService dataRefreshService, IRepositoryWrapper Repository, IPublisher Publisher)
    : IRequestHandler<CreateM3UFileRequest, APIResponse>
{
    public async Task<APIResponse> Handle(CreateM3UFileRequest request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(request.UrlSource))
        {
            return APIResponse.NotFound;
        }

        try
        {
            FileDefinition fd = FileDefinitions.M3U;

            string name = request.Name + fd.DefaultExtension;
            string compressedFileName = fileUtilService.CheckNeedsCompression(name);
            string fullName = Path.Combine(fd.DirectoryLocation, compressedFileName);

            M3UFile m3UFile = new()
            {
                Name = request.Name,
                Url = request.UrlSource,
                MaxStreamCount = request.MaxStreamCount,
                Source = name,
                VODTags = request.VODTags ?? [],
                HoursToUpdate = request.HoursToUpdate ?? 72,
                SyncChannels = request.SyncChannels ?? false,
                DefaultStreamGroupName = request.DefaultStreamGroupName,
                AutoSetChannelNumbers = request.AutoSetChannelNumbers ?? false,
                StartingChannelNumber = request.StartingChannelNumber ?? 1,
                M3UKey = request.M3UKey ?? M3UKey.URL
            };

            string source = HttpUtility.UrlDecode(request.UrlSource);
            m3UFile.Url = source;
            m3UFile.LastDownloadAttempt = SMDT.UtcNow;

            await messageService.SendInfo($"Adding M3U '{request.Name}'");
            Logger.LogInformation("Adding M3U '{name}'", request.Name);

            (bool success, Exception? ex) = await fileUtilService.DownloadUrlAsync(source, fullName).ConfigureAwait(false);
            if (success)
            {
                m3UFile.LastDownloaded = File.GetLastWriteTime(fullName);
                m3UFile.FileExists = true;
            }
            else
            {
                ++m3UFile.DownloadErrors;

                Logger.LogCritical("Exception M3U From URL '{ex}'", ex);
                await messageService.SendError("Exception M3U", ex?.Message);
            }

            m3UFile.MaxStreamCount = Math.Max(0, request.MaxStreamCount);

            //List<SMStream>? streams = await m3UFile.GetSMStreamsFromM3U(Logger).ConfigureAwait(false);
            //if (streams == null || streams.Count == 0)
            //{
            //    Logger.LogCritical("Exception M3U '{name}' format is not supported", request.Name);
            //    await messageService.SendError($"Exception M3U '{request.Name}' format is not supported");
            //    //Bad M3U
            //    if (File.Exists(fullName))
            //    {
            //        File.Delete(fullName);
            //    }
            //    string urlPath = Path.GetFileNameWithoutExtension(fullName) + ".url";
            //    if (File.Exists(urlPath))
            //    {
            //        File.Delete(urlPath);
            //    }
            //    return APIResponse.NotFound;
            //}

            Repository.M3UFile.CreateM3UFile(m3UFile);
            _ = await Repository.SaveAsync().ConfigureAwait(false);
            CacheManager.M3UMaxStreamCounts.AddOrUpdate(m3UFile.Id, m3UFile.MaxStreamCount, (_, _) => m3UFile.MaxStreamCount);
            m3UFile.WriteJSON();

            await dataRefreshService.RefreshAllM3U();

            await Publisher.Publish(new M3UFileProcessEvent(m3UFile.Id, false), cancellationToken).ConfigureAwait(false);

            await messageService.SendSuccess("M3U '" + m3UFile.Name + "' added successfully");

            return APIResponse.Success;
        }
        catch (Exception exception)
        {
            //    if (File.Exists(fullName))
            //    {
            //        File.Delete(fullName);
            //    }
            //    string urlPath = Path.GetFileNameWithoutExtension(fullName) + ".url";
            //    if (File.Exists(urlPath))
            //    {
            //        File.Delete(urlPath);
            //    }

            await messageService.SendError("Exception adding M3U", exception.Message);
            Logger.LogCritical("Exception M3U From Form '{exception}'", exception);
        }
        return APIResponse.NotFound;
    }
}