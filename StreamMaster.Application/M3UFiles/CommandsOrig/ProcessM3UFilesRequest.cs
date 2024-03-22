using StreamMaster.Application.M3UFiles.Commands;

namespace StreamMaster.Application.M3UFiles.CommandsOrig;

public record ProcessM3UFilesRequest : IRequest { }

public class ProcessM3UFilesRequestHandler(ILogger<ProcessM3UFilesRequest> Logger, IRepositoryWrapper Repository, ISender Sender) : IRequestHandler<ProcessM3UFilesRequest>
{
    public async Task Handle(ProcessM3UFilesRequest command, CancellationToken cancellationToken)
    {
        try
        {
            foreach (M3UFileDto m3uFile in await Repository.M3UFile.GetM3UFiles().ConfigureAwait(false))
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
