namespace StreamMaster.Application.Icons.Queries;

[SMAPI]
[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public record GetIconsRequest() : IRequest<List<IconFileDto>>;

internal class GetIconsRequestHandler(IIconService iconService)
    : IRequestHandler<GetIconsRequest, List<IconFileDto>>
{
    public Task<List<IconFileDto>> Handle(GetIconsRequest request, CancellationToken cancellationToken)
    {
        List<IconFileDto> icons = iconService.GetIcons();

        return Task.FromResult(icons);
    }
}