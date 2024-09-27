namespace StreamMaster.Application.SMChannelChannelLinks.Commands;


public record UpdateSMChannelRanksRequest(int ParentSMChannelId, List<int> Channels)
    : IRequest<DataResponse<List<SMChannelDto>>>;

internal class UpdateSMChannelRanksRequestHandler(IRepositoryWrapper Repository, IMapper mapper)
    : IRequestHandler<UpdateSMChannelRanksRequest, DataResponse<List<SMChannelDto>>>
{
    public async Task<DataResponse<List<SMChannelDto>>> Handle(UpdateSMChannelRanksRequest request, CancellationToken cancellationToken)
    {
        List<SMChannelDto> ret = [];

        List<SMChannelChannelLink> links = [.. Repository.SMChannelChannelLink.GetQuery(true).Where(a => a.ParentSMChannelId == request.ParentSMChannelId).Include(a => a.SMChannel)];

        foreach (int channelId in request.Channels)
        {
            SMChannelChannelLink? link = links.Find(a => a.SMChannelId == channelId);

            if (link != null)
            {
                SMChannelDto sm = mapper.Map<SMChannelDto>(link.SMChannel);
                sm.Rank = link.Rank;
                ret.Add(sm);
            }
        }
        List<SMChannelDto> test = ret.OrderBy(a => a.Rank).ToList();
        return await Task.FromResult(DataResponse<List<SMChannelDto>>.Success(test));
    }
}
