namespace StreamMaster.Application.Logos.Commands;

[SMAPI]
[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public record RemoveCustomLogoRequest(string Source) : IRequest<APIResponse>;

[LogExecutionTimeAspect]
public class RemoveCustomLogoRequestHandler(ILogoService logoService, IMessageService messageService, IDataRefreshService dataRefreshService)
    : IRequestHandler<RemoveCustomLogoRequest, APIResponse>
{
    public async Task<APIResponse> Handle(RemoveCustomLogoRequest request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(request.Source))
        {
            return APIResponse.NotFound;
        }

        try
        {
            logoService.RemoveCustomLogo(request.Source);

            await messageService.SendSuccess("Custom logo removed successfully");

            await dataRefreshService.RefreshLogos();

            return APIResponse.Success;
        }
        catch (Exception)
        {
        }
        return APIResponse.Error;
    }
}