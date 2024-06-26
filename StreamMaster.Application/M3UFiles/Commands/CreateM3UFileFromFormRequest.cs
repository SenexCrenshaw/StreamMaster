using Microsoft.AspNetCore.Http;

namespace StreamMaster.Application.M3UFiles.Commands;

[SMAPI(JustController = true, JustHub = true)]
[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public record CreateM3UFileFromFormRequest(string Name, int MaxStreamCount, int? HoursToUpdate, bool? OverWriteChannels, int? StartingChannelNumber, IFormFile? FormFile, List<string>? VODTags) : IRequest<APIResponse> { }

[LogExecutionTimeAspect]
public class CreateM3UFileFromFormRequestHandler(ILogger<CreateM3UFileFromFormRequest> Logger, IMessageService messageService, IDataRefreshService dataRefreshService, IRepositoryWrapper Repository, IPublisher Publisher)
    : IRequestHandler<CreateM3UFileFromFormRequest, APIResponse>
{
    public async Task<APIResponse> Handle(CreateM3UFileFromFormRequest command, CancellationToken cancellationToken)
    {
        if (command.FormFile == null)
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
                MaxStreamCount = command.MaxStreamCount,
                StartingChannelNumber = command.StartingChannelNumber == null ? 1 : (int)command.StartingChannelNumber,
                OverwriteChannelNumbers = command.OverWriteChannels != null && (bool)command.OverWriteChannels,
                VODTags = command.VODTags ?? [],
                HoursToUpdate = command.HoursToUpdate ?? 72,
            };


            Logger.LogInformation("Adding M3U From Form: '{name}'", command.Name);
            (bool success, Exception? ex) = await FormHelper.SaveFormFileAsync(command.FormFile!, fullName).ConfigureAwait(false);
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


            m3UFile.MaxStreamCount = Math.Max(0, command.MaxStreamCount);

            List<SMStream>? streams = await m3UFile.GetSMStreamsFromM3U(Logger).ConfigureAwait(false);
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