namespace StreamMaster.Application.SMChannels.Queries;

[SMAPI]
[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public record GetSMChannelRequest(int SMChannelId) : IRequest<DataResponse<SMChannelDto>>;

internal class GetSMChannelRequestHandler(IRepositoryWrapper Repository, IMapper mapper)
    : IRequestHandler<GetSMChannelRequest, DataResponse<SMChannelDto>>
{
    public async Task<DataResponse<SMChannelDto>> Handle(GetSMChannelRequest request, CancellationToken cancellationToken)
    {
        var smChannel = await Repository.SMChannel.FirstOrDefaultAsync(a => a.Id == request.SMChannelId, cancellationToken: cancellationToken).ConfigureAwait(false);
        var smChannelDTO = mapper.Map<SMChannelDto>(smChannel);

        List<SMChannelStreamLink> links = Repository.SMChannelStreamLink.GetQuery(true).Where(a => a.SMChannelId == smChannel.Id).ToList();

        foreach (SMStreamDto stream in smChannelDTO.SMStreams)
        {
            SMChannelStreamLink? link = links.FirstOrDefault(a => a.SMStreamId == stream.Id);

            if (link != null)
            {
                stream.Rank = link.Rank;
            }
        }


        return DataResponse<SMChannelDto>.Success(smChannelDTO);
    }
}
