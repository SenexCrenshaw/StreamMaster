namespace StreamMasterApplication.M3UFiles.Commands;

public record ProcessM3UFilesRequest : IRequest { }

public class ProcessM3UFilesRequestHandler : BaseMediatorRequestHandler, IRequestHandler<ProcessM3UFilesRequest>
{

    public ProcessM3UFilesRequestHandler(ILogger<ProcessM3UFilesRequest> logger, IRepositoryWrapper repository, IMapper mapper, IPublisher publisher, ISender sender, IHubContext<StreamMasterHub, IStreamMasterHub> hubContext)
 : base(logger, repository, mapper, publisher, sender, hubContext) { }


    public async Task Handle(ProcessM3UFilesRequest command, CancellationToken cancellationToken)
    {
        try
        {
            foreach (M3UFile m3uFile in await Repository.M3UFile.GetAllM3UFilesAsync().ConfigureAwait(false))
            {
                _ = await Sender.Send(new ProcessM3UFileRequest(m3uFile.Id), cancellationToken).ConfigureAwait(false);
            }
        }
        catch (Exception ex)
        {
            Logger.LogCritical(ex, "Error while processing M3U file");
        }
    }
}
