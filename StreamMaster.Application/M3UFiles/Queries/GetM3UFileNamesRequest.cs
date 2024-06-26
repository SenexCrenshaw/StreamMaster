namespace StreamMaster.Application.M3UFiles.Queries;

[SMAPI]
[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public record GetM3UFileNamesRequest() : IRequest<DataResponse<List<string>>>;

internal class GetM3UFileNamesRequestHandler(IRepositoryWrapper Repository)
    : IRequestHandler<GetM3UFileNamesRequest, DataResponse<List<string>>>
{
    public async Task<DataResponse<List<string>>> Handle(GetM3UFileNamesRequest request, CancellationToken cancellationToken)
    {
        var m3uNames = await Repository.M3UFile.GetQuery().OrderBy(a => a.Name).Select(a => a.Name).ToListAsync(cancellationToken: cancellationToken).ConfigureAwait(false);
        return DataResponse<List<string>>.Success(m3uNames);
    }
}
