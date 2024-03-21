using Microsoft.AspNetCore.Http;

namespace StreamMaster.Application.M3UFiles;

public partial class M3UFileService(ILogger<M3UFileService> logger, IRepositoryWrapper repository, IJobStatusService jobStatusService, IHttpContextAccessor httpContextAccessor, IHubContext<StreamMasterHub, IStreamMasterHub> hubContext, IOptionsMonitor<HLSSettings> inthlssettings, IOptionsMonitor<Setting> intsettings)
{
    private readonly Setting settings = intsettings.CurrentValue;
    private readonly HLSSettings hlssettings = inthlssettings.CurrentValue;

    [SMAPI]
    [LogExecutionTimeAspect]
    public async Task<M3UFile?> ProcessM3UFileRequest(int M3UFileId)
    {
        JobStatusManager jobManager = jobStatusService.GetJobManager(JobType.ProcessM3U, M3UFileId);

        jobManager.Start();
        M3UFile? m3uFile = null;
        try
        {
            m3uFile = await repository.M3UFile.GetM3UFileByTrackedId(M3UFileId).ConfigureAwait(false);
            if (m3uFile == null)
            {
                logger.LogCritical("Could not find M3U file");
                jobManager.SetError();
                return null;
            }


            return jobManager.IsRunning ? null : null;
        }
        catch (Exception ex)
        {
        }
        return null;
    }
}