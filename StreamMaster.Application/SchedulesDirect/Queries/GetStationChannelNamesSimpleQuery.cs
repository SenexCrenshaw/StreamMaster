﻿using StreamMaster.Domain.Pagination;

namespace StreamMaster.Application.SchedulesDirect.Queries;
public record GetStationChannelNamesSimpleQuery(StationChannelNameParameters Parameters) : IRequest<List<StationChannelName>>;

public class GetStationChannelNamesSimpleQueryHandler(ISchedulesDirectDataService schedulesDirectDataService)
    : IRequestHandler<GetStationChannelNamesSimpleQuery, List<StationChannelName>>
{
    public Task<List<StationChannelName>> Handle(GetStationChannelNamesSimpleQuery request, CancellationToken cancellationToken)
    {
        List<StationChannelName> ret = [];

        // Retrieve and filter Programmes
        List<StationChannelName> stationChannelNames = schedulesDirectDataService.GetStationChannelNames();

        if (stationChannelNames.Count != 0)
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
                StationChannelName? programme = stationChannelNames.FirstOrDefault(a => a.Channel == channel);
                if (programme != null)
                {
                    ret.Add(programme);
                }
            }
        }

        return Task.FromResult(ret);
    }
}