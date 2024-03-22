namespace StreamMaster.Application.M3UFiles.Commands;

public record ProcessM3UFile(int M3UFileId, bool ForceRun = false) : IRequest<DefaultAPIResponse>;

internal class ProcessM3UFileHandler(ILogger<ProcessM3UFile> logger, IRepositoryWrapper Repository, IHubContext<StreamMasterHub, IStreamMasterHub> hubContext) : IRequestHandler<ProcessM3UFile, DefaultAPIResponse>
{
    public async Task<DefaultAPIResponse> Handle(ProcessM3UFile request, CancellationToken cancellationToken)
    {
        try
        {
            M3UFile? m3uFile = await Repository.M3UFile.ProcessM3UFile(request.M3UFileId, request.ForceRun).ConfigureAwait(false);
            if (m3uFile == null)
            {
                return APIResponseFactory.NotFound;
            }

            await hubContext.Clients.All.DataRefresh("M3UFile").ConfigureAwait(false);

            return APIResponseFactory.Ok;
        }
        catch (Exception ex)
        {
            logger.LogCritical(ex, "Error while processing M3U file");
            return APIResponseFactory.NotFound; ;
        }
    }
}
