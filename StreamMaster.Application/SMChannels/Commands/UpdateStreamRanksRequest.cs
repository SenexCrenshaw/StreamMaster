namespace StreamMaster.Application.SMChannels.Commands;


public record UpdateStreamRanksRequest(int SMChannelId, List<SMStream> Streams)
    : IRequest<DataResponse<List<SMStreamDto>>>;

internal class UpdateStreamRanksRequestHandler(IRepositoryWrapper Repository, IMapper mapper)
    : IRequestHandler<UpdateStreamRanksRequest, DataResponse<List<SMStreamDto>>>
{
    public async Task<DataResponse<List<SMStreamDto>>> Handle(UpdateStreamRanksRequest request, CancellationToken cancellationToken)
    {
        List<SMStreamDto> ret = [];

        List<SMChannelStreamLink> links = [.. Repository.SMChannelStreamLink.GetQuery(true).Where(a => a.SMChannelId == request.SMChannelId)];

        foreach (SMStream stream in request.Streams)
        {
            SMChannelStreamLink? link = links.FirstOrDefault(a => a.SMStreamId == stream.Id);

            if (link != null)
            {
                SMStreamDto sm = mapper.Map<SMStreamDto>(stream);
                sm.Rank = link.Rank;
                ret.Add(sm);
            }
        }
        return DataResponse<List<SMStreamDto>>.Success(ret);
    }
}
