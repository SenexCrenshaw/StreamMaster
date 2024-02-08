using StreamMaster.Domain.Filtering;
using StreamMaster.Domain.Pagination;

using System.Text.Json;

namespace StreamMaster.Application.Icons.Queries;

public record GetPagedIcons(IconFileParameters IconFileParameters) : IRequest<PagedResponse<IconFileDto>>;

internal class GetPagedIconsHandler(IIconService iconService) : IRequestHandler<GetPagedIcons, PagedResponse<IconFileDto>>
{
    public async Task<PagedResponse<IconFileDto>> Handle(GetPagedIcons request, CancellationToken cancellationToken)
    {
        if (request.IconFileParameters.PageSize == 0)
        {
            PagedResponse<IconFileDto> emptyResponse = new()
            {
                TotalItemCount = iconService.GetIcons().Count
            };
            return emptyResponse;
        }

        IQueryable<IconFileDto> icons = iconService.GetIcons().AsQueryable();

        if (!string.IsNullOrEmpty(request.IconFileParameters.JSONFiltersString) || !string.IsNullOrEmpty(request.IconFileParameters.OrderBy))
        {
            if (!string.IsNullOrEmpty(request.IconFileParameters.JSONFiltersString))
            {
                List<DataTableFilterMetaData>? filters = JsonSerializer.Deserialize<List<DataTableFilterMetaData>>(request.IconFileParameters.JSONFiltersString);
                icons = FilterHelper<IconFileDto>.ApplyFiltersAndSort(icons, filters, "Name asc", true);
            }
        }

        IPagedList<IconFileDto> test = await icons.ToPagedListAsync(request.IconFileParameters.PageNumber, request.IconFileParameters.PageSize).ConfigureAwait(false);

        PagedResponse<IconFileDto> pagedResponse = test.ToPagedResponse();
        return pagedResponse;
    }
}