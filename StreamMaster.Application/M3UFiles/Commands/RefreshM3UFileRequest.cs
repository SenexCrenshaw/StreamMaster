using FluentValidation;

namespace StreamMaster.Application.M3UFiles.Commands;

public record RefreshM3UFileRequest(int Id) : IRequest<M3UFile?> { }

public class RefreshM3UFileRequestValidator : AbstractValidator<RefreshM3UFileRequest>
{
    public RefreshM3UFileRequestValidator()
    {
        _ = RuleFor(v => v.Id).NotNull().GreaterThanOrEqualTo(0);
    }
}

[LogExecutionTimeAspect]
public class RefreshM3UFileRequestHandler : BaseMediatorRequestHandler, IRequestHandler<RefreshM3UFileRequest, M3UFile?>
{
    public RefreshM3UFileRequestHandler(ILogger<RefreshM3UFileRequest> logger, IRepositoryWrapper repository, IMapper mapper, ISettingsService settingsService, IPublisher publisher, ISender sender, IHubContext<StreamMasterHub, IStreamMasterHub> hubContext, IMemoryCache memoryCache)
 : base(logger, repository, mapper, settingsService, publisher, sender, hubContext, memoryCache) { }

    public async Task<M3UFile?> Handle(RefreshM3UFileRequest request, CancellationToken cancellationToken)
    {
        try
        {
            M3UFile? m3uFile = await Repository.M3UFile.GetM3UFileById(request.Id).ConfigureAwait(false);
            if (m3uFile == null)
            {
                return null;
            }

            if (m3uFile.LastDownloadAttempt.AddMinutes(m3uFile.MinimumMinutesBetweenDownloads) < DateTime.Now)
            {
                FileDefinition fd = FileDefinitions.M3U;
                string fullName = Path.Combine(fd.DirectoryLocation, m3uFile.Source);

                if (m3uFile.Url != null && m3uFile.Url.Contains("://"))
                {
                    Logger.LogInformation("Refresh M3U From URL {m3uFile.Url}", m3uFile.Url);

                    m3uFile.LastDownloadAttempt = DateTime.Now;

                    (bool success, Exception? ex) = await FileUtil.DownloadUrlAsync(m3uFile.Url, fullName, cancellationToken).ConfigureAwait(false);
                    if (success)
                    {
                        m3uFile.DownloadErrors = 0;
                        m3uFile.LastDownloaded = File.GetLastWriteTime(fullName);
                        m3uFile.FileExists = true;
                    }
                    else
                    {
                        ++m3uFile.DownloadErrors;
                        Logger.LogCritical("Exception M3U From URL {ex}", ex);
                    }
                }

                List<VideoStream>? streams = await m3uFile.GetM3U(Logger, cancellationToken);
                if (streams == null)
                {
                    Logger.LogCritical("Exception M3U {fullName} format is not supported", fullName);
                    //Bad M3U
                    if (File.Exists(fullName))
                    {
                        File.Delete(fullName);
                    }
                    return null;
                }

                if (m3uFile.StationCount != streams.Count)
                {
                    m3uFile.StationCount = streams.Count;
                }
                m3uFile.LastUpdated = DateTime.Now;
                Repository.M3UFile.UpdateM3UFile(m3uFile);
                _ = await Repository.SaveAsync().ConfigureAwait(false);

                M3UFileDto ret = Mapper.Map<M3UFileDto>(m3uFile);
                await Publisher.Publish(new M3UFileAddedEvent(ret.Id), cancellationToken).ConfigureAwait(false);
            }

            return m3uFile;
        }
        catch (Exception)
        {
        }

        return null;
    }
}