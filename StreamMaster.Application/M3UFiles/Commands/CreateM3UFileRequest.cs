using Microsoft.AspNetCore.Http;

using System.Web;

namespace StreamMaster.Application.M3UFiles.Commands;

public record CreateM3UFileRequest(string? Description, int MaxStreamCount, bool? OverWriteChannels, int? StartingChannelNumber, IFormFile? FormFile, string Name, string? UrlSource, List<string>? VODTags) : IRequest<bool> { }

[LogExecutionTimeAspect]
public class CreateM3UFileRequestHandler(ILogger<CreateM3UFileRequest> Logger, IRepositoryWrapper Repository, IMapper Mapper, IPublisher Publisher) : IRequestHandler<CreateM3UFileRequest, bool>
{
    public async Task<bool> Handle(CreateM3UFileRequest command, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(command.UrlSource) && command.FormFile != null && command.FormFile.Length <= 0)
        {
            return false;
        }

        //Setting setting = await _settingsService.GetSettingsAsync();
        try
        {
            FileDefinition fd = FileDefinitions.M3U;
            string fullName = Path.Combine(fd.DirectoryLocation, command.Name + fd.FileExtension);

            M3UFile m3UFile = new()
            {
                Description = command.Description ?? "",
                Name = command.Name,
                Source = command.Name + fd.FileExtension,
                StartingChannelNumber = command.StartingChannelNumber == null ? 1 : (int)command.StartingChannelNumber,
                OverwriteChannelNumbers = command.OverWriteChannels != null && (bool)command.OverWriteChannels,
                VODTags = command.VODTags ?? [],
                //StreamURLPrefix = command.StreamURLPrefixInt == null ? M3UFileStreamURLPrefix.SystemDefault : (M3UFileStreamURLPrefix)command.StreamURLPrefixInt
            };

            if (command.FormFile != null)
            {
                Logger.LogInformation("Adding M3U From Form: {fullName}", fullName);
                (bool success, Exception? ex) = await FormHelper.SaveFormFileAsync(command.FormFile!, fullName).ConfigureAwait(false);
                if (success)
                {
                    m3UFile.LastDownloaded = File.GetLastWriteTime(fullName);
                    m3UFile.FileExists = true;
                }
                else
                {
                    Logger.LogCritical("Exception M3U From Form {ex}", ex);
                    return false;
                }
            }
            else if (!string.IsNullOrEmpty(command.UrlSource))
            {
                string source = HttpUtility.UrlDecode(command.UrlSource);
                m3UFile.Url = source;
                m3UFile.LastDownloadAttempt = SMDT.UtcNow;

                Logger.LogInformation("Add M3U From URL {command.UrlSource}", command.UrlSource);
                (bool success, Exception? ex) = await FileUtil.DownloadUrlAsync(source, fullName, cancellationToken).ConfigureAwait(false);
                if (success)
                {
                    m3UFile.LastDownloaded = File.GetLastWriteTime(fullName);
                    m3UFile.FileExists = true;
                }
                else
                {
                    ++m3UFile.DownloadErrors;
                    Logger.LogCritical("Exception M3U From URL {ex}", ex);
                }
            }

            m3UFile.MaxStreamCount = Math.Max(0, command.MaxStreamCount);

            List<VideoStream>? streams = await m3UFile.GetM3U(Logger, cancellationToken).ConfigureAwait(false);
            if (streams == null || streams.Count == 0)
            {
                Logger.LogCritical("Exception M3U {fullName} format is not supported", fullName);
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
                return false;
            }


            //m3UFile.StationCount = streams.Count;

            Repository.M3UFile.CreateM3UFile(m3UFile);
            _ = await Repository.SaveAsync().ConfigureAwait(false);

            m3UFile.WriteJSON(Logger);

            M3UFileDto ret = Mapper.Map<M3UFileDto>(m3UFile);
            await Publisher.Publish(new M3UFileAddedEvent(ret.Id, false), cancellationToken).ConfigureAwait(false);

            return true;
        }
        catch (Exception exception)
        {
            Logger.LogCritical("Exception M3U From Form {exception}", exception);
        }
        return false;
    }
}