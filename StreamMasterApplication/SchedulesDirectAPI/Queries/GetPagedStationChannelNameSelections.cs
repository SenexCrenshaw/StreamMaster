﻿using StreamMaster.SchedulesDirectAPI.Domain.Models;

using StreamMasterDomain.Filtering;
using StreamMasterDomain.Pagination;

using System.Text.Json;

namespace StreamMasterApplication.SchedulesDirectAPI.Queries;

public record GetPagedStationChannelNameSelections(StationChannelNameParameters Parameters) : IRequest<PagedResponse<StationChannelName>>;

internal class GetPagedStationChannelNamesHandler(ILogger<GetPagedStationChannelNameSelections> logger, IRepositoryWrapper repository, ISchedulesDirect schedulesDirect, IMapper mapper, ISettingsService settingsService, IPublisher publisher, ISender sender, IHubContext<StreamMasterHub, IStreamMasterHub> hubContext, IMemoryCache memoryCache) : BaseMediatorRequestHandler(logger, repository, mapper, settingsService, publisher, sender, hubContext, memoryCache), IRequestHandler<GetPagedStationChannelNameSelections, PagedResponse<StationChannelName>>
{
    public async Task<PagedResponse<StationChannelName>> Handle(GetPagedStationChannelNameSelections request, CancellationToken cancellationToken)
    {
        var stationChannelNames = schedulesDirect.GetStationChannelNames();
        if (request.Parameters.PageSize == 0)
        {          
            PagedResponse<StationChannelName> emptyResponse = new()
            {
                TotalItemCount = stationChannelNames.Count()
            };
            return emptyResponse;
        }

        var programmes = stationChannelNames.Where(a => !string.IsNullOrEmpty(a.Channel)).AsQueryable();

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
                programmes = FilterHelper<StationChannelName>.ApplyFiltersAndSort(programmes, filters, "DisplayName asc");
            }
        }

        IPagedList<StationChannelName> pagedList = await programmes.OrderBy(a => a.DisplayName)
            .ToPagedListAsync(request.Parameters.PageNumber, request.Parameters.PageSize)
            .ConfigureAwait(false);

        PagedResponse<StationChannelName> pagedResponse = pagedList.ToPagedResponse();
        return pagedResponse;
    }


}
