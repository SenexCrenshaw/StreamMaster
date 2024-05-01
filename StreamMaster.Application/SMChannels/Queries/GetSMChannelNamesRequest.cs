namespace StreamMaster.Application.SMChannels.Queries;

[SMAPI]
[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public record GetSMChannelNamesRequest() : IRequest<DataResponse<List<string>>>;

internal class GetSMChannelNamesRequestHandler(IRepositoryWrapper Repository)
    : IRequestHandler<GetSMChannelNamesRequest, DataResponse<List<string>>>
{
    public async Task<DataResponse<List<string>>> Handle(GetSMChannelNamesRequest request, CancellationToken cancellationToken)
    {
        var a = Repository.SMChannel.GetQuery().ToList();
        var channelNames = await Repository.SMChannel.GetQuery().OrderBy(a => a.Name).Select(a => a.Name).ToListAsync(cancellationToken: cancellationToken).ConfigureAwait(false);
        return DataResponse<List<string>>.Success(channelNames);
    }
}
