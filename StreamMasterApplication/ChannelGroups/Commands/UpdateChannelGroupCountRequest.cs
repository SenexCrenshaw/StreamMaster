using FluentValidation;

using Microsoft.EntityFrameworkCore;

using StreamMasterDomain.Models;

namespace StreamMasterApplication.ChannelGroups.Commands;
public record UpdateChannelGroupCountRequest(ChannelGroupDto ChannelGroupDto) : IRequest<bool> { }

public class UpdateChannelGroupCountRequestValidator : AbstractValidator<UpdateChannelGroupCountRequest>
{
    public UpdateChannelGroupCountRequestValidator()
    {
        _ = RuleFor(v => v.ChannelGroupDto).NotNull();
    }
}


[LogExecutionTimeAspect]
public class UpdateChannelGroupCountRequestHandler(ILogger<UpdateChannelGroupCountRequest> logger, IRepositoryWrapper repository, IMapper mapper, ISettingsService settingsService, IPublisher publisher, ISender sender, IHubContext<StreamMasterHub, IStreamMasterHub> hubContext, IMemoryCache memoryCache) : BaseMediatorRequestHandler(logger, repository, mapper, settingsService, publisher, sender, hubContext, memoryCache), IRequestHandler<UpdateChannelGroupCountRequest, bool>
{
    public async Task<bool> Handle(UpdateChannelGroupCountRequest request, CancellationToken cancellationToken)
    {
        if (request.ChannelGroupDto == null)
        {
            return false;
        }

        IQueryable<VideoStream> videoStreamsForGroupQuery = Repository.VideoStream.GetVideoStreamQuery().Where(vs => vs.User_Tvg_group == request.ChannelGroupDto.Name);

        int totalCount = await videoStreamsForGroupQuery.CountAsync(cancellationToken);
        int hiddenCount = await videoStreamsForGroupQuery.CountAsync(vs => vs.IsHidden, cancellationToken);

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

        if (changed)
        {
            ChannelGroupStreamCount response = new()
            {
                ChannelGroupId = request.ChannelGroupDto.Id
            };

            MemoryCache.AddOrUpdateChannelGroupVideoStreamCount(response);
            await HubContext.Clients.All.UpdateChannelGroupVideoStreamCounts([response]).ConfigureAwait(false);
        }

        return changed;
    }


}
