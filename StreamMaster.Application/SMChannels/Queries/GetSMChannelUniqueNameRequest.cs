namespace StreamMaster.Application.SMChannels.Queries;

[SMAPI]
[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public record GetSMChannelUniqueNameRequest(string SMChannelName) : IRequest<DataResponse<string>>;

internal class GetSMChannelUniqueNameRequestHandler(IRepositoryContext repositoryContext)
    : IRequestHandler<GetSMChannelUniqueNameRequest, DataResponse<string>>
{
    public async Task<DataResponse<string>> Handle(GetSMChannelUniqueNameRequest request, CancellationToken cancellationToken)
    {
        List<string> results = await repositoryContext.SqlQueryRaw<string>($"SELECT get_unique_smchannel_name('{request.SMChannelName}');").ToListAsync(cancellationToken: cancellationToken);
        return results.Count != 0 ? DataResponse<string>.Success(results[0]) : DataResponse<string>.NotFound;
    }
}
