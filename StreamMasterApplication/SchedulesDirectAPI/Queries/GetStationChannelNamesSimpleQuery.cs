﻿using StreamMaster.SchedulesDirectAPI.Domain.Models;

using StreamMasterDomain.Pagination;

namespace StreamMasterApplication.SchedulesDirectAPI.Queries;
public record GetStationChannelNamesSimpleQuery(StationChannelNameParameters Parameters) : IRequest<List<StationChannelName>>;

public class GetStationChannelNamesSimpleQueryHandler(ILogger<GetStationChannelNamesSimpleQuery> logger, ISchedulesDirect schedulesDirect, IRepositoryWrapper repository, IMapper mapper, ISettingsService settingsService, IPublisher publisher, ISender sender, IHubContext<StreamMasterHub, IStreamMasterHub> hubContext, IMemoryCache memoryCache) : BaseMediatorRequestHandler(logger, repository, mapper, settingsService, publisher, sender, hubContext, memoryCache), IRequestHandler<GetStationChannelNamesSimpleQuery, List<StationChannelName>>
{
    public Task<List<StationChannelName>> Handle(GetStationChannelNamesSimpleQuery request, CancellationToken cancellationToken)
    {
        List<StationChannelName> ret = [];

        // Retrieve and filter Programmes
        var stationChannelNames = schedulesDirect.GetStationChannelNames();

        if (stationChannelNames.Any())
        {
            // Get distinct channel names in order and take the required amount
            List<string> distinctChannels = stationChannelNames
                .OrderBy(a => a.DisplayName, StringComparer.OrdinalIgnoreCase)
                .Select(a => a.Channel)
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .Skip(request.Parameters.First)
                .Take(request.Parameters.Count)
                .ToList();

            foreach (string channel in distinctChannels)
            {
                var programme = stationChannelNames.FirstOrDefault(a => a.Channel == channel);
                if (programme != null)
                {
                    ret.Add(programme);
                }
            }
        }

        return Task.FromResult(ret);
    }
}