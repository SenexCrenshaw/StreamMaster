﻿using StreamMaster.Domain.Filtering;

using System.Text.Json;

using X.Extensions.PagedList.EF;

namespace StreamMaster.Application.SchedulesDirect.Queries;

public record GetPagedStationChannelNameSelections(QueryStringParameters Parameters) : IRequest<PagedResponse<StationChannelName>>;

internal class GetPagedStationChannelNamesHandler(ISchedulesDirectDataService schedulesDirectDataService)
    : IRequestHandler<GetPagedStationChannelNameSelections, PagedResponse<StationChannelName>>
{
    public async Task<PagedResponse<StationChannelName>> Handle(GetPagedStationChannelNameSelections request, CancellationToken cancellationToken)
    {
        IEnumerable<StationChannelName> stationChannelNames = schedulesDirectDataService.GetStationChannelNames();
        if (request.Parameters.PageSize == 0)
        {
            PagedResponse<StationChannelName> emptyResponse = new()
            {
                TotalItemCount = stationChannelNames.Count()
            };
            return emptyResponse;
        }

        IQueryable<StationChannelName> programmes = stationChannelNames.Where(a => !string.IsNullOrEmpty(a.Channel)).AsQueryable();

        if (!string.IsNullOrEmpty(request.Parameters.JSONFiltersString))
        {
            List<DataTableFilterMetaData>? filters = JsonSerializer.Deserialize<List<DataTableFilterMetaData>>(request.Parameters.JSONFiltersString);
            if (filters != null)
            {
                DataTableFilterMetaData? nameFilter = filters.FirstOrDefault(a => a.FieldName == "name");
                if (nameFilter != null)
                {
                    nameFilter.FieldName = "DisplayName";
                }
                programmes = FilterHelper<StationChannelName>.ApplyFiltersAndSort(programmes, filters, "DisplayName asc", true);
            }
        }

        IPagedList<StationChannelName> pagedList = await programmes.OrderBy(a => a.DisplayName)
            .ToPagedListAsync(request.Parameters.PageNumber, request.Parameters.PageSize)
            .ConfigureAwait(false);

        PagedResponse<StationChannelName> pagedResponse = pagedList.ToPagedResponse();
        return pagedResponse;
    }


}
