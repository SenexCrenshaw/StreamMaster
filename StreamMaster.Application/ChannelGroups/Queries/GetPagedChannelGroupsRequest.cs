namespace StreamMaster.Application.ChannelGroups.Queries;

[SMAPI]
[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public record GetPagedChannelGroupsRequest(QueryStringParameters Parameters) : IRequest<PagedResponse<ChannelGroupDto>>;

[LogExecutionTimeAspect]
internal class GetPagedChannelGroupsQueryHandler(IRepositoryWrapper Repository, IMapper Mapper)
    : IRequestHandler<GetPagedChannelGroupsRequest, PagedResponse<ChannelGroupDto>>
{
    public async Task<PagedResponse<ChannelGroupDto>> Handle(GetPagedChannelGroupsRequest request, CancellationToken cancellationToken)
    {
        if (request.Parameters.PageSize == 0)
        {
            return Repository.ChannelGroup.CreateEmptyPagedResponse();
        }
        PagedResponse<ChannelGroup> paged = await Repository.ChannelGroup.GetPagedChannelGroups(request.Parameters).ConfigureAwait(false);
        PagedResponse<ChannelGroupDto> dto = paged.ToPagedResponseDto<ChannelGroup, ChannelGroupDto>(Mapper);

        return dto;
    }
}