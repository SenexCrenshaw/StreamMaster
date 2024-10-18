namespace StreamMaster.Application.EPGFiles.Queries;

[SMAPI]
[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public record GetEPGFilesRequest() : IRequest<DataResponse<List<EPGFileDto>>>;

public class GetEPGFilesRequestHandler(IRepositoryWrapper Repository, IMapper mapper)
    : IRequestHandler<GetEPGFilesRequest, DataResponse<List<EPGFileDto>>>
{
    public async Task<DataResponse<List<EPGFileDto>>> Handle(GetEPGFilesRequest request, CancellationToken cancellationToken = default)
    {
        var epgFiles = await Repository.EPGFile.GetQuery().OrderBy(a => a.Name).ProjectTo<EPGFileDto>(mapper.ConfigurationProvider).ToListAsync(cancellationToken: cancellationToken);

        return DataResponse<List<EPGFileDto>>.Success(epgFiles);
    }
}