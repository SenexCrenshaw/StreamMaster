using FluentValidation;

using Microsoft.EntityFrameworkCore;

namespace StreamMaster.Application.ChannelGroups.Commands;
public record UpdateChannelGroupCountRequest(ChannelGroupDto ChannelGroupDto, bool Publish)
    : IRequest<DataResponse<ChannelGroupDto>>
{ }

[LogExecutionTimeAspect]
public class UpdateChannelGroupCountRequestHandler(ILogger<UpdateChannelGroupCountRequest> logger, IRepositoryWrapper repository, IHubContext<StreamMasterHub, IStreamMasterHub> hubContext, IMemoryCache MemoryCache)
    : IRequestHandler<UpdateChannelGroupCountRequest, DataResponse<ChannelGroupDto>>
{
    public async Task<DataResponse<ChannelGroupDto>> Handle(UpdateChannelGroupCountRequest request, CancellationToken cancellationToken)
    {
        try
        {
            IQueryable<VideoStream> videoStreamsForGroupQuery = repository.VideoStream.GetVideoStreamQuery().Where(vs => vs.User_Tvg_group == request.ChannelGroupDto.Name);

            // Optimize to reduce the database hits by fetching counts together
            var counts = await videoStreamsForGroupQuery.GroupBy(vs => vs.IsHidden).Select(g => new { IsHidden = g.Key, Count = g.Count() }).ToListAsync(cancellationToken);

            int totalCount = counts.Sum(c => c.Count);
            int hiddenCount = counts.FirstOrDefault(c => c.IsHidden)?.Count ?? 0;

            bool changed = false;

            if (request.ChannelGroupDto.TotalCount != totalCount)
            {
                request.ChannelGroupDto.TotalCount = totalCount;
                changed = true;
            }

            if (request.ChannelGroupDto.ActiveCount != totalCount - hiddenCount)
            {
                request.ChannelGroupDto.ActiveCount = totalCount - hiddenCount;
                changed = true;
            }

            if (request.ChannelGroupDto.HiddenCount != hiddenCount)
            {
                request.ChannelGroupDto.HiddenCount = hiddenCount;
                changed = true;
            }

            if (request.ChannelGroupDto.IsHidden != (request.ChannelGroupDto.HiddenCount == 0))
            {
                request.ChannelGroupDto.HiddenCount = hiddenCount;
                changed = true;
            }

            if (changed)
            {
                //ChannelGroupStreamCount response = new()
                //{
                //    ChannelGroupId = request.ChannelGroupDto.Id
                //};

                MemoryCache.AddOrUpdateChannelGroupVideoStreamCount(request.ChannelGroupDto);
                if (request.Publish)
                {
                    await hubContext.Clients.All.ChannelGroupsRefresh([request.ChannelGroupDto]).ConfigureAwait(false);
                }
            }

            return DataResponse<ChannelGroupDto>.Success(request.ChannelGroupDto);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error while processing UpdateChannelGroupCountRequest.");
            throw; // Re-throw the exception or handle as needed
        }
    }
}