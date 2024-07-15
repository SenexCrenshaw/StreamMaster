namespace StreamMaster.Application.EPGFiles.Commands;

[SMAPI]
[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public record ProcessEPGFileRequest(int Id) : IRequest<APIResponse>;

public class ProcessEPGFileRequestHandler(ILogger<ProcessEPGFileRequest> logger, IMessageService messageService, IDataRefreshService dataRefreshService, IRepositoryWrapper Repository)
    : IRequestHandler<ProcessEPGFileRequest, APIResponse>
{
    public async Task<APIResponse> Handle(ProcessEPGFileRequest request, CancellationToken cancellationToken)
    {
        try
        {
            EPGFile? epgFile = await Repository.EPGFile.ProcessEPGFile(request.Id).ConfigureAwait(false);
            if (epgFile == null)
            {
                await messageService.SendError("Process EPG Not Found");
                return APIResponse.NotFound;
            }

            await dataRefreshService.RefreshAllEPG();

            await messageService.SendSuccess("Processed EPG '" + epgFile.Name + "' successfully");
            return APIResponse.Success;
        }
        catch (Exception ex)
        {
            logger.LogCritical(ex, "Process EPG Error");
            await messageService.SendError("Error Processing EPG", ex.Message);
            return APIResponse.NotFound; ;
        }

    }
}