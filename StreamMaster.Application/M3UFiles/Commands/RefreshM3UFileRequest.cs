namespace StreamMaster.Application.M3UFiles.Commands;

[SMAPI]
[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public record RefreshM3UFileRequest(int Id, bool ForceRun = false) : IRequest<APIResponse>;

[LogExecutionTimeAspect]
public class RefreshM3UFileRequestHandler(ILogger<RefreshM3UFileRequest> Logger, IMessageService messageService, IJobStatusService jobStatusService, IRepositoryWrapper Repository, IMapper Mapper, IPublisher Publisher)
    : IRequestHandler<RefreshM3UFileRequest, APIResponse>
{

    public async Task<APIResponse> Handle(RefreshM3UFileRequest request, CancellationToken cancellationToken)
    {
        JobStatusManager jobManager = jobStatusService.GetJobManagerRefreshM3U(request.Id);
        try
        {
            if (jobManager.IsRunning)
            {
                return APIResponse.NotFound;
            }
            jobManager.Start();


            M3UFile? m3uFile = await Repository.M3UFile.GetM3UFile(request.Id).ConfigureAwait(false);
            if (m3uFile == null)
            {
                jobManager.SetError();
                return APIResponse.NotFound;
            }

            if (request.ForceRun || m3uFile.LastDownloadAttempt.AddMinutes(m3uFile.MinimumMinutesBetweenDownloads) < SMDT.UtcNow)
            {
                FileDefinition fd = FileDefinitions.M3U;
                string fullName = Path.Combine(fd.DirectoryLocation, m3uFile.Source);

                if (m3uFile.Url != null && m3uFile.Url.Contains("://"))
                {
                    Logger.LogInformation("Refresh M3U From URL {m3uFile.Url}", m3uFile.Url);

                    m3uFile.LastDownloadAttempt = SMDT.UtcNow;

                    (bool success, Exception? ex) = await FileUtil.DownloadUrlAsync(m3uFile.Url, fullName, cancellationToken).ConfigureAwait(false);
                    if (success)
                    {
                        m3uFile.DownloadErrors = 0;
                        m3uFile.LastDownloaded = File.GetLastWriteTime(fullName).ToUniversalTime();
                        m3uFile.FileExists = true;
                    }
                    else
                    {
                        ++m3uFile.DownloadErrors;
                        await messageService.SendError("Exception M3U From URL ", ex?.Message);
                        Logger.LogCritical("Exception M3U From URL {ex}", ex);
                    }
                }

                List<SMStream>? streams = await m3uFile.GetSMStreamsFromM3U(Logger);
                if (streams == null)
                {
                    Logger.LogCritical("Exception M3U {fullName} format is not supported", fullName);
                    await messageService.SendError($"Exception M3U {fullName} format is not supported");
                    //Bad M3U
                    if (File.Exists(fullName))
                    {
                        File.Delete(fullName);
                    }
                    jobManager.SetError();
                    return APIResponse.NotFound;
                }
            }

            //m3uFile.LastUpdated = SMDT.UtcNow;
            Repository.M3UFile.UpdateM3UFile(m3uFile);
            _ = await Repository.SaveAsync().ConfigureAwait(false);

            M3UFileDto ret = Mapper.Map<M3UFileDto>(m3uFile);
            //if (publish)
            //{
            await Publisher.Publish(new M3UFileProcessEvent(ret.Id, request.ForceRun), cancellationToken).ConfigureAwait(false);
            //}
            jobManager.SetSuccessful();

            return APIResponse.Success;
        }
        catch
        {
            jobManager.SetError();
            return APIResponse.NotFound;
        }

    }
}