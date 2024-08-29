using Microsoft.AspNetCore.Http;
namespace StreamMaster.Application.M3UFiles.Commands;

[SMAPI(JustController = true, JustHub = true)]
[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public record CreateM3UFileFromFormRequest(string Name, int? MaxStreamCount, int? StartingChannelNumber, bool? AutoSetChannelNumbers, string? DefaultStreamGroupName, int? HoursToUpdate, bool? SyncChannels, IFormFile? FormFile, List<string>? VODTags) : IRequest<APIResponse>;

[LogExecutionTimeAspect]
public class CreateM3UFileFromFormRequestHandler(ILogger<CreateM3UFileFromFormRequest> Logger, ICacheManager CacheManager, IMessageService messageService, IDataRefreshService dataRefreshService, IRepositoryWrapper Repository, IPublisher Publisher)
    : IRequestHandler<CreateM3UFileFromFormRequest, APIResponse>
{
    public async Task<APIResponse> Handle(CreateM3UFileFromFormRequest request, CancellationToken cancellationToken)
    {
        if (request.FormFile == null)
        {

            return APIResponse.NotFound;
        }

        try
        {
            FileDefinition fd = FileDefinitions.M3U;
            string fullName = Path.Combine(fd.DirectoryLocation, request.Name + fd.FileExtension);

            M3UFile m3UFile = new()
            {
                Name = request.Name,
                Source = request.Name + fd.FileExtension,
                MaxStreamCount = request.MaxStreamCount ?? 0,
                VODTags = request.VODTags ?? [],
                HoursToUpdate = request.HoursToUpdate ?? 72,
                SyncChannels = request.SyncChannels ?? false,
                DefaultStreamGroupName = request.DefaultStreamGroupName,
                AutoSetChannelNumbers = request.AutoSetChannelNumbers ?? false,
                StartingChannelNumber = request.StartingChannelNumber ?? 1
            };


            Logger.LogInformation("Adding M3U From Form: '{name}'", request.Name);
            (bool success, Exception? ex) = await FormHelper.SaveFormFileAsync(request.FormFile!, fullName).ConfigureAwait(false);
            if (success)
            {
                m3UFile.LastDownloaded = File.GetLastWriteTime(fullName);
                m3UFile.FileExists = true;
            }
            else
            {

                Logger.LogCritical("Exception M3U From Form '{ex}'", ex);
                await messageService.SendError($"Exception M3U From Form", ex?.Message);
                return APIResponse.NotFound;
            }


            m3UFile.MaxStreamCount = Math.Max(0, request.MaxStreamCount ?? 0);

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
            await messageService.SendError("Exception adding M3U", exception.Message);
            Logger.LogCritical("Exception M3U From Form '{exception}'", exception);
        }
        return APIResponse.NotFound;
    }
}