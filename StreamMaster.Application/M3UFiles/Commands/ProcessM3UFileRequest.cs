namespace StreamMaster.Application.M3UFiles.Commands;

[SMAPI(JustHub: true, IsTask: true)]
public record ProcessM3UFileRequest(int M3UFileId, bool ForceRun = false) : IRequest<DefaultAPIResponse>;

internal class ProcessM3UFileRequestHandler(ILogger<ProcessM3UFileRequest> logger, IRepositoryWrapper Repository, IHubContext<StreamMasterHub, IStreamMasterHub> hubContext) : IRequestHandler<ProcessM3UFileRequest, DefaultAPIResponse>
{
    public async Task<DefaultAPIResponse> Handle(ProcessM3UFileRequest request, CancellationToken cancellationToken)
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
