namespace StreamMaster.Application.SMChannelStreamLinks.Commands;


public record UpdateStreamRanksRequest(int SMChannelId, List<string> Streams)
    : IRequest<DataResponse<List<SMStreamDto>>>;

internal class UpdateStreamRanksRequestHandler(IRepositoryWrapper Repository, IMapper mapper)
    : IRequestHandler<UpdateStreamRanksRequest, DataResponse<List<SMStreamDto>>>
{
    public async Task<DataResponse<List<SMStreamDto>>> Handle(UpdateStreamRanksRequest request, CancellationToken cancellationToken)
    {
        List<SMStreamDto> ret = [];

        List<SMChannelStreamLink> links = [.. Repository.SMChannelStreamLink.GetQuery(true).Where(a => a.SMChannelId == request.SMChannelId).Include(a => a.SMStream)];

        foreach (string streamId in request.Streams)
        {
            SMChannelStreamLink? link = links.FirstOrDefault(a => a.SMStreamId == streamId);

            if (link != null)
            {
                SMStreamDto sm = mapper.Map<SMStreamDto>(link.SMStream);
                sm.Rank = link.Rank;
                ret.Add(sm);
            }
        }
        return DataResponse<List<SMStreamDto>>.Success(ret.OrderBy(a => a.Rank).ToList());
    }
}
