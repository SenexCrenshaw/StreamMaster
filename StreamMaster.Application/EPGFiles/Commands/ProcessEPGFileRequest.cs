﻿using FluentValidation;

namespace StreamMaster.Application.EPGFiles.Commands;

public record ProcessEPGFileRequest(int Id) : IRequest<EPGFileDto?> { }

public class ProcessEPGFileRequestValidator : AbstractValidator<ProcessEPGFileRequest>
{
    public ProcessEPGFileRequestValidator()
    {
        _ = RuleFor(v => v.Id)
            .NotNull()
            .GreaterThanOrEqualTo(0);
    }
}

public class ProcessEPGFileRequestHandler(ILogger<ProcessEPGFileRequest> logger, IRepositoryWrapper repository, IJobStatusService jobStatusService, IXmltv2Mxf xmltv2Mxf, IRepositoryWrapper Repository, IMapper Mapper, IPublisher Publisher, IHubContext<StreamMasterHub, IStreamMasterHub> HubContext)
    : IRequestHandler<ProcessEPGFileRequest, EPGFileDto?>
{
    [LogExecutionTimeAspect]
    public async Task<EPGFileDto?> Handle(ProcessEPGFileRequest request, CancellationToken cancellationToken)
    {
        JobStatusManager jobManager = jobStatusService.GetJobManager(JobType.ProcessEPG, request.Id);
        if (jobManager.IsRunning)
        {
            return null;
        }

        EPGFile? epgFile = null;
        try
        {
            epgFile = await Repository.EPGFile.GetEPGFileById(request.Id).ConfigureAwait(false);

            if (epgFile == null)
            {
                return null;
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
            return ret;
        }

        catch (Exception ex)
        {
            jobManager.SetError();
            logger.LogCritical(ex, "Error while processing EPG file");
            return null;
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