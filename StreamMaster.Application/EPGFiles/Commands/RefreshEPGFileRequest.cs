using FluentValidation;

namespace StreamMaster.Application.EPGFiles.Commands;

[RequireAll]
public record RefreshEPGFileRequest(int Id) : IRequest<EPGFileDto?> { }

public class RefreshEPGFileRequestValidator : AbstractValidator<RefreshEPGFileRequest>
{
    public RefreshEPGFileRequestValidator()
    {
        _ = RuleFor(v => v.Id).NotNull().GreaterThanOrEqualTo(0);
    }
}

public class RefreshEPGFileRequestHandler(ILogger<RefreshEPGFileRequest> Logger, IMapper Mapper, IJobStatusService jobStatusService, IRepositoryWrapper Repository, IPublisher Publisher) : IRequestHandler<RefreshEPGFileRequest, EPGFileDto?>
{
    private readonly object lockObject = new();
    public async Task<EPGFileDto?> Handle(RefreshEPGFileRequest request, CancellationToken cancellationToken)
    {
        try
        {
            lock (lockObject)
            {
                if (jobStatusService.GetEPGJobStatus().IsRunning)
                {
                    return null;
                }
                jobStatusService.SetEPGIsRunning(true);

            }

            EPGFile? epgFile = await Repository.EPGFile.GetEPGFileById(request.Id).ConfigureAwait(false);
            if (epgFile == null)
            {
                return null;
            }

            bool publish = false;
            if (epgFile.LastDownloadAttempt.AddMinutes(epgFile.MinimumMinutesBetweenDownloads) < DateTime.Now)
            {

                FileDefinition fd = FileDefinitions.EPG;
                string fullName = Path.Combine(fd.DirectoryLocation, epgFile.Source);

                if (epgFile.Url != null && epgFile.Url.Contains("://"))
                {
                    publish = true;
                    Logger.LogInformation("Refresh EPG From URL {epgFile.Url}", epgFile.Url);

                    epgFile.LastDownloadAttempt = DateTime.Now;

                    (bool success, Exception? ex) = await FileUtil.DownloadUrlAsync(epgFile.Url, fullName, cancellationToken).ConfigureAwait(false);
                    if (success)
                    {
                        epgFile.DownloadErrors = 0;
                        epgFile.LastDownloaded = File.GetLastWriteTime(fullName);
                        epgFile.FileExists = true;
                    }
                    else
                    {
                        ++epgFile.DownloadErrors;
                        Logger.LogCritical("Exception EPG From URL {ex}", ex);
                    }
                }
            }

            epgFile.LastUpdated = DateTime.Now;
            Repository.EPGFile.UpdateEPGFile(epgFile);

            _ = await Repository.SaveAsync().ConfigureAwait(false);

            EPGFileDto toPublish = Mapper.Map<EPGFileDto>(epgFile);
            if (publish)
            {
                await Publisher.Publish(new EPGFileAddedEvent(toPublish), cancellationToken).ConfigureAwait(false);
            }
            return toPublish;

        }
        finally
        {
            jobStatusService.SetEPGIsRunning(false);
        }
    }
}
