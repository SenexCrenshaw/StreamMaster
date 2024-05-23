namespace StreamMaster.Application.M3UFiles.Queries;

[SMAPI]
[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public record GetM3UFilesRequest() : IRequest<DataResponse<List<M3UFileDto>>>;

public class GetM3UFilesRequestHandler(IRepositoryWrapper Repository, IMapper mapper)
    : IRequestHandler<GetM3UFilesRequest, DataResponse<List<M3UFileDto>>>
{
    public async Task<DataResponse<List<M3UFileDto>>> Handle(GetM3UFilesRequest request, CancellationToken cancellationToken = default)
    {
        var m3uFiles = await Repository.M3UFile.GetQuery().OrderBy(a => a.Name).ProjectTo<M3UFileDto>(mapper.ConfigurationProvider).ToListAsync(cancellationToken: cancellationToken);

        return DataResponse<List<M3UFileDto>>.Success(m3uFiles);
    }
}