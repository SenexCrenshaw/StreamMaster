namespace StreamMaster.Application.EPGFiles.Queries;

[SMAPI]
[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public record GetEPGFileNamesRequest() : IRequest<DataResponse<List<string>>>;

internal class GetEPGFileNamesRequestHandler(IRepositoryWrapper Repository)
    : IRequestHandler<GetEPGFileNamesRequest, DataResponse<List<string>>>
{
    public async Task<DataResponse<List<string>>> Handle(GetEPGFileNamesRequest request, CancellationToken cancellationToken)
    {
        List<string> epgNames = await Repository.EPGFile.GetQuery().OrderBy(a => a.Name).Select(a => a.Name).ToListAsync(cancellationToken: cancellationToken).ConfigureAwait(false);
        return DataResponse<List<string>>.Success(epgNames);
    }
}