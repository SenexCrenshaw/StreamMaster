namespace StreamMaster.Application.EPGFiles.Commands;

[SMAPI]
[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public record ProcessEPGFileRequest(int Id) : IRequest<APIResponse> { }

public class ProcessEPGFileRequestHandler(ILogger<ProcessEPGFileRequest> logger, IMessageService messageSevice, IHubContext<StreamMasterHub, IStreamMasterHub> hubContext, IRepositoryWrapper Repository)
    : IRequestHandler<ProcessEPGFileRequest, APIResponse>
{
    public async Task<APIResponse> Handle(ProcessEPGFileRequest request, CancellationToken cancellationToken)
    {
        try
        {
            EPGFile? epgFile = await Repository.EPGFile.ProcessEPGFile(request.Id).ConfigureAwait(false);
            if (epgFile == null)
            {
                await messageSevice.SendError("Process EPG Not Found");
                return APIResponse.NotFound;
            }

            await hubContext.Clients.All.DataRefresh(EPGFile.MainGet).ConfigureAwait(false);

            await messageSevice.SendSuccess("Processed EPG '" + epgFile.Name + "' successfully");
            return APIResponse.Success;
        }
        catch (Exception ex)
        {
            logger.LogCritical(ex, "Process EPG Error");
            await messageSevice.SendError("Error Processing EPG", ex.Message);
            return APIResponse.NotFound; ;
        }

    }
}