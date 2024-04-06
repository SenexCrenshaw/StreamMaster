namespace StreamMaster.Application.EPGFiles.Queries;

public record GetEPGFilesNeedUpdatingRequest() : IRequest<DataResponse<List<EPGFileDto>>>;

internal class GetEPGFilesNeedUpdatingHandler(IRepositoryWrapper Repository)
    : IRequestHandler<GetEPGFilesNeedUpdatingRequest, DataResponse<List<EPGFileDto>>>
{
    public async Task<DataResponse<List<EPGFileDto>>> Handle(GetEPGFilesNeedUpdatingRequest request, CancellationToken cancellationToken = default)
    {
        List<EPGFileDto> epgFiles = await Repository.EPGFile.GetEPGFilesNeedUpdating();
        return DataResponse<List<EPGFileDto>>.Success(epgFiles);
    }
}