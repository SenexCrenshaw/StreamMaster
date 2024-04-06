namespace StreamMaster.Application.EPGFiles.Commands;

[SMAPI]
[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public record ProcessEPGFileRequest(int Id) : IRequest<APIResponse> { }

public class ProcessEPGFileRequestHandler(ILogger<ProcessEPGFileRequest> logger, IRepositoryWrapper repository, IJobStatusService jobStatusService, IXmltv2Mxf xmltv2Mxf, IRepositoryWrapper Repository, IMapper Mapper, IPublisher Publisher, IHubContext<StreamMasterHub, IStreamMasterHub> HubContext)
    : IRequestHandler<ProcessEPGFileRequest, APIResponse>
{
    [LogExecutionTimeAspect]
    public async Task<APIResponse> Handle(ProcessEPGFileRequest request, CancellationToken cancellationToken)
    {
        JobStatusManager jobManager = jobStatusService.GetJobManager(JobType.ProcessEPG, request.Id);
        if (jobManager.IsRunning)
        {
            return APIResponse.NotFound;
        }

        EPGFile? epgFile = null;
        try
        {
            epgFile = await Repository.EPGFile.GetEPGFileById(request.Id).ConfigureAwait(false);

            if (epgFile == null)
            {
                jobManager.SetError();
                return APIResponse.NotFound;
            }

            XMLTV? tv = xmltv2Mxf.ConvertToMxf(Path.Combine(FileDefinitions.EPG.DirectoryLocation, epgFile.Source), epgFile.EPGNumber);

            if (tv != null)
            {
                epgFile.ChannelCount = tv.Channels != null ? tv.Channels.Count : 0;
                epgFile.ProgrammeCount = tv.Programs != null ? tv.Programs.Count : 0;
            }

            epgFile.LastUpdated = SMDT.UtcNow;
            Repository.EPGFile.UpdateEPGFile(epgFile);

            _ = await Repository.SaveAsync().ConfigureAwait(false);

            EPGFileDto ret = Mapper.Map<EPGFileDto>(epgFile);

            await HubContext.Clients.All.ProgrammesRefresh().ConfigureAwait(false);

            await Publisher.Publish(new EPGFileProcessedEvent(ret), cancellationToken).ConfigureAwait(false);
            jobManager.SetSuccessful();
            return APIResponse.Success;
        }

        catch (Exception ex)
        {
            jobManager.SetError();
            logger.LogCritical(ex, "Error while processing EPG file");
            return APIResponse.ErrorWithMessage(ex, "Error while processing EPG file");

        }
        finally
        {
            if (epgFile != null)
            {

                epgFile.LastUpdated = SMDT.UtcNow;

                repository.EPGFile.UpdateEPGFile(epgFile);
                _ = await repository.SaveAsync().ConfigureAwait(false);
            }
        }
    }

}