using FluentValidation;

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

public class ProcessEPGFileRequestHandler(ILogger<ProcessEPGFileRequest> logger, IJobStatusService jobStatusService, IXmltv2Mxf xmltv2Mxf, IRepositoryWrapper Repository, IMapper Mapper, IPublisher Publisher, IHubContext<StreamMasterHub, IStreamMasterHub> HubContext)
    : IRequestHandler<ProcessEPGFileRequest, EPGFileDto?>
{
    [LogExecutionTimeAspect]
    public async Task<EPGFileDto?> Handle(ProcessEPGFileRequest request, CancellationToken cancellationToken)
    {
        try
        {
            EPGFile? epgFile = await Repository.EPGFile.GetEPGFileById(request.Id).ConfigureAwait(false);

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

            epgFile.LastUpdated = DateTime.Now;
            Repository.EPGFile.UpdateEPGFile(epgFile);

            _ = await Repository.SaveAsync().ConfigureAwait(false);

            EPGFileDto ret = Mapper.Map<EPGFileDto>(epgFile);

            await HubContext.Clients.All.ProgrammesRefresh().ConfigureAwait(false);

            await Publisher.Publish(new EPGFileProcessedEvent(ret), cancellationToken).ConfigureAwait(false);

            return ret;
        }

        catch (Exception ex)
        {
        }
        finally
        {
            jobStatusService.SetEPGIsRunning(false);
        }
        return null;
    }

}