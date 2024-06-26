namespace StreamMaster.Application.Icons.Queries;

[SMAPI]
[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public record GetIconsRequest() : IRequest<DataResponse<List<IconFileDto>>>;

internal class GetIconsRequestHandler(IIconService iconService)
    : IRequestHandler<GetIconsRequest, DataResponse<List<IconFileDto>>>
{
    public Task<DataResponse<List<IconFileDto>>> Handle(GetIconsRequest request, CancellationToken cancellationToken)
    {
        List<IconFileDto> icons = iconService.GetIcons();

        return Task.FromResult(DataResponse<List<IconFileDto>>.Success(icons));
    }
}