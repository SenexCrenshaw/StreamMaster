using FluentValidation;

namespace StreamMaster.Application.M3UFiles.Commands;

public record RefreshM3UFileRequest(int Id, bool forceRun = false) : IRequest<M3UFile?> { }

public class RefreshM3UFileRequestValidator : AbstractValidator<RefreshM3UFileRequest>
{
    public RefreshM3UFileRequestValidator()
    {
        _ = RuleFor(v => v.Id).NotNull().GreaterThanOrEqualTo(0);
    }
}

[LogExecutionTimeAspect]
public class RefreshM3UFileRequestHandler(ILogger<RefreshM3UFileRequest> Logger, IJobStatusService jobStatusService, IRepositoryWrapper Repository, IMapper Mapper, IPublisher Publisher) : IRequestHandler<RefreshM3UFileRequest, M3UFile?>
{
    private readonly object lockObject = new();
    public async Task<M3UFile?> Handle(RefreshM3UFileRequest request, CancellationToken cancellationToken)
    {
        try
        {
            lock (lockObject)
            {
                if (jobStatusService.GetM3UJobStatus().IsRunning)
                {
                    return null;
                }
                jobStatusService.SetM3UIsRunning(true);
            }

            M3UFile? m3uFile = await Repository.M3UFile.GetM3UFileById(request.Id).ConfigureAwait(false);
            if (m3uFile == null)
            {
                return null;
            }

            bool publish = false;

            if (request.forceRun || m3uFile.LastDownloadAttempt.AddMinutes(m3uFile.MinimumMinutesBetweenDownloads) < DateTime.Now)
            {
                publish = true;
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
            }

            //m3uFile.LastUpdated = DateTime.Now;
            Repository.M3UFile.UpdateM3UFile(m3uFile);
            _ = await Repository.SaveAsync().ConfigureAwait(false);

            M3UFileDto ret = Mapper.Map<M3UFileDto>(m3uFile);
            //if (publish)
            //{
            await Publisher.Publish(new M3UFileAddedEvent(ret.Id, request.forceRun), cancellationToken).ConfigureAwait(false);
            //}

            return m3uFile;
        }
        finally
        {

        }

    }
}