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
        return DataResponse<SMChannelDto>.Success(smChannelDTO);
    }
}
