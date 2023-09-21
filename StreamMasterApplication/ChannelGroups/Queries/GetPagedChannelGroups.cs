using StreamMasterApplication.ChannelGroups.Commands;

using StreamMasterDomain.Pagination;

namespace StreamMasterApplication.ChannelGroups.Queries;

public record GetPagedChannelGroups(ChannelGroupParameters Parameters) : IRequest<PagedResponse<ChannelGroupDto>>;

[LogExecutionTimeAspect]
internal class GetPagedChannelGroupsQueryHandler(ILogger<GetPagedChannelGroups> logger, IRepositoryWrapper repository, IMapper mapper, ISettingsService settingsService, IPublisher publisher, ISender sender, IHubContext<StreamMasterHub, IStreamMasterHub> hubContext, IMemoryCache memoryCache) : BaseMediatorRequestHandler(logger, repository, mapper, settingsService, publisher, sender, hubContext, memoryCache), IRequestHandler<GetPagedChannelGroups, PagedResponse<ChannelGroupDto>>
{
    public async Task<PagedResponse<ChannelGroupDto>> Handle(GetPagedChannelGroups request, CancellationToken cancellationToken)
    {
        if (request.Parameters.PageSize == 0)
        {
            return Repository.ChannelGroup.CreateEmptyPagedResponse();
        }
        PagedResponse<ChannelGroup> paged = await Repository.ChannelGroup.GetPagedChannelGroups(request.Parameters).ConfigureAwait(false);
        PagedResponse<ChannelGroupDto> dto = paged.ToPagedResponseDto<ChannelGroup, ChannelGroupDto>(Mapper);
        dto.Data = await Sender.Send(new UpdateChannelGroupCountsRequest(dto.Data), cancellationToken).ConfigureAwait(false);
        // dto.Data = MemoryCache.UpdateChannelGroupsWithActives(dto.Data);
        return dto;
    }
}