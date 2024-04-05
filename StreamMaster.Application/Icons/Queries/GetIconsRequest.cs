namespace StreamMaster.Application.Icons.Queries;

[SMAPI]
[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public record GetIconsRequest() : IRequest<APIResponse<List<IconFileDto>>>;

internal class GetIconsRequestHandler(IIconService iconService)
    : IRequestHandler<GetIconsRequest, APIResponse<List<IconFileDto>>>
{
    public Task<APIResponse<List<IconFileDto>>> Handle(GetIconsRequest request, CancellationToken cancellationToken)
    {
        List<IconFileDto> icons = iconService.GetIcons();

        return Task.FromResult(APIResponse<List<IconFileDto>>.Success(icons));
    }
}