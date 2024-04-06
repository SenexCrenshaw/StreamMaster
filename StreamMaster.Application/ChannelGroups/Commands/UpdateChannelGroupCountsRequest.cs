using FluentValidation;

using Microsoft.EntityFrameworkCore;

namespace StreamMaster.Application.ChannelGroups.Commands;

public record UpdateChannelGroupCountsRequest(List<ChannelGroupDto>? ChannelGroups)
    : IRequest<DataResponse<List<ChannelGroupDto>>>
{ }

[LogExecutionTimeAspect]
public class UpdateChannelGroupCountsRequestHandler(ILogger<UpdateChannelGroupCountsRequest> Logger, IRepositoryWrapper Repository, IMapper mapper, IMemoryCache MemoryCache)
    : IRequestHandler<UpdateChannelGroupCountsRequest, DataResponse<List<ChannelGroupDto>>>
{

    public async Task<DataResponse<List<ChannelGroupDto>>> Handle(UpdateChannelGroupCountsRequest request, CancellationToken cancellationToken)
    {
        if (request == null)
        {
            throw new ArgumentNullException(nameof(request));
        }

        try
        {

            List<ChannelGroupDto> dtos = mapper.Map<List<ChannelGroupDto>>(request.ChannelGroups);

            ChannelGroupDto? c = dtos.Find(a => a.Id == 29);

            if (dtos.Any())
            {

                List<string> cgNames = dtos.ConvertAll(a => a.Name);

                // Fetch relevant video streams.
                var allVideoStreams = await Repository.VideoStream.GetVideoStreamQuery()
                    .Where(a => cgNames.Contains(a.User_Tvg_group))
                    .Select(vs => new
                    {
                        vs.Id,
                        vs.User_Tvg_group,
                        vs.IsHidden
                    }).ToListAsync(cancellationToken: cancellationToken).ConfigureAwait(false);

                Dictionary<string, List<string>> videoStreamsForGroups = [];
                Dictionary<string, int> hiddenCounts = [];

                foreach (ChannelGroupDto cg in dtos)
                {
                    if (cg == null)
                    {
                        continue;
                    }

                    var relevantStreams = allVideoStreams.Where(vs => vs.User_Tvg_group == cg.Name).ToList();

                    videoStreamsForGroups[cg.Name] = relevantStreams.Select(vs => vs.Id).ToList();
                    hiddenCounts[cg.Name] = relevantStreams.Count(vs => vs.IsHidden);

                    cg.TotalCount = videoStreamsForGroups[cg.Name].Count;
                    cg.ActiveCount = videoStreamsForGroups[cg.Name].Count - hiddenCounts[cg.Name];
                    cg.HiddenCount = hiddenCounts[cg.Name];
                    cg.IsHidden = hiddenCounts[cg.Name] != 0;
                }

                MemoryCache.AddOrUpdateChannelGroupVideoStreamCounts(dtos);

                //await HubContext.Clients.All.ChannelGroupsRefresh(dtos.ToArray()).ConfigureAwait(false);

                //if (channelGroupStreamCounts.Any())
                //{
                //    var a = channelGroupStreamCounts.FirstOrDefault(a => a.ChannelGroupId == 29);
                //    MemoryCache.AddOrUpdateChannelGroupVideoStreamCounts(channelGroupStreamCounts);
                //    var b = channelGroupStreamCounts.FirstOrDefault(a => a.ChannelGroupId == 29);
                //    await HubContext.Clients.All.UpdateChannelGroups(cgs).ConfigureAwait(false);
                //}
            }

            //if (channelGroupStreamCounts.Any())
            //{
            //    MemoryCache.AddOrUpdateChannelGroupVideoStreamCounts(channelGroupStreamCounts);
            //    await HubContext.Clients.All.UpdateChannelGroupVideoStreamCounts(channelGroupStreamCounts).ConfigureAwait(false);
            //}
            return DataResponse<List<ChannelGroupDto>>.Success(dtos);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error while handling UpdateChannelGroupCountsRequest.");
            return DataResponse<List<ChannelGroupDto>>.ErrorWithMessage(ex, "Error while handling UpdateChannelGroupCountsRequest.");
            throw; // Re-throw the exception if needed or handle accordingly.
        }

    }
}