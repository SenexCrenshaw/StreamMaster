namespace StreamMaster.Application.EPGFiles.Commands;

[SMAPI]
[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public record RefreshEPGFileRequest(int Id) : IRequest<DefaultAPIResponse> { }

public class RefreshEPGFileRequestHandler(ILogger<RefreshEPGFileRequest> Logger, IMapper Mapper, IJobStatusService jobStatusService, IRepositoryWrapper Repository, IPublisher Publisher) : IRequestHandler<RefreshEPGFileRequest, DefaultAPIResponse>
{

    public async Task<DefaultAPIResponse> Handle(RefreshEPGFileRequest request, CancellationToken cancellationToken)
    {
        JobStatusManager jobManager = jobStatusService.GetJobManager(JobType.RefreshEPG, request.Id);
        try
        {

            if (jobManager.IsRunning)
            {
                return DefaultAPIResponse.NotFound;
            }
            jobManager.Start();


            EPGFile? epgFile = await Repository.EPGFile.GetEPGFileById(request.Id).ConfigureAwait(false);
            if (epgFile == null)
            {
                jobManager.SetError();
                return DefaultAPIResponse.NotFound;
            }


            if (epgFile.LastDownloadAttempt.AddMinutes(epgFile.MinimumMinutesBetweenDownloads) < SMDT.UtcNow)
            {

                FileDefinition fd = FileDefinitions.EPG;
                string fullName = Path.Combine(fd.DirectoryLocation, epgFile.Source);

                if (epgFile.Url != null && epgFile.Url.Contains("://"))
                {

                    Logger.LogInformation("Refresh EPG From URL {epgFile.Url}", epgFile.Url);

                    epgFile.LastDownloadAttempt = SMDT.UtcNow;

                    (bool success, Exception? ex) = await FileUtil.DownloadUrlAsync(epgFile.Url, fullName, cancellationToken).ConfigureAwait(false);
                    if (success)
                    {
                        epgFile.DownloadErrors = 0;
                        epgFile.LastDownloaded = File.GetLastWriteTime(fullName).ToUniversalTime();
                        epgFile.FileExists = true;
                    }
                    else
                    {
                        ++epgFile.DownloadErrors;
                        Logger.LogCritical("Exception EPG From URL {ex}", ex);
                    }
                }
            }

            epgFile.LastUpdated = SMDT.UtcNow;
            Repository.EPGFile.UpdateEPGFile(epgFile);

            _ = await Repository.SaveAsync().ConfigureAwait(false);

            EPGFileDto toPublish = Mapper.Map<EPGFileDto>(epgFile);

            await Publisher.Publish(new EPGFileAddedEvent(toPublish), cancellationToken).ConfigureAwait(false);

            jobManager.SetSuccessful();
            return DefaultAPIResponse.Success;

        }
        catch (Exception ex)
        {
            jobManager.SetError();
            return DefaultAPIResponse.ErrorWithMessage(ex, "Refresh EPG failed");
        }

    }
}
