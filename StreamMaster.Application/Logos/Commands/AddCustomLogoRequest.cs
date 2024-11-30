namespace StreamMaster.Application.Logos.Commands;

[SMAPI]
[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public record AddCustomLogoRequest(string Name, string Source) : IRequest<APIResponse>;

[LogExecutionTimeAspect]
public class AddCustomLogoRequestHandler(ILogoService logoService, IMessageService messageService, IDataRefreshService dataRefreshService)
    : IRequestHandler<AddCustomLogoRequest, APIResponse>
{
    public async Task<APIResponse> Handle(AddCustomLogoRequest request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(request.Name) || string.IsNullOrEmpty(request.Source))
        {
            return APIResponse.NotFound;
        }

        try
        {
            logoService.AddCustomLogo(request.Name, request.Source);

            await messageService.SendSuccess("Custom logo added successfully");
            await dataRefreshService.RefreshLogos();

            return APIResponse.Success;
        }
        catch (Exception exception)
        {
        }
        return APIResponse.Error;
    }
}