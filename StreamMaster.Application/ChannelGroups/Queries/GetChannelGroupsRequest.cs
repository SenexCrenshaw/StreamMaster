namespace StreamMaster.Application.ChannelGroups.Queries;

[SMAPI]
[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public record GetChannelGroupsRequest() : IRequest<DataResponse<List<ChannelGroupDto>>>;

internal class GetChannelGroupsRequestHandler(IRepositoryWrapper Repository, IMapper mapper)
    : IRequestHandler<GetChannelGroupsRequest, DataResponse<List<ChannelGroupDto>>>
{
    public async Task<DataResponse<List<ChannelGroupDto>>> Handle(GetChannelGroupsRequest request, CancellationToken cancellationToken)
    {
        List<ChannelGroupDto> channelGroups = await Repository.ChannelGroup.GetQuery().OrderBy(a => a.Name).ProjectTo<ChannelGroupDto>(mapper.ConfigurationProvider).ToListAsync(cancellationToken: cancellationToken);
        return DataResponse<List<ChannelGroupDto>>.Success(channelGroups);
    }
}