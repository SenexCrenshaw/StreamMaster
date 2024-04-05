namespace StreamMaster.Application.EPGFiles.Queries;

public record GetEPGFilesNeedUpdatingRequest() : IRequest<APIResponse<List<EPGFileDto>>>;

internal class GetEPGFilesNeedUpdatingHandler(IRepositoryWrapper Repository)
    : IRequestHandler<GetEPGFilesNeedUpdatingRequest, APIResponse<List<EPGFileDto>>>
{
    public async Task<APIResponse<List<EPGFileDto>>> Handle(GetEPGFilesNeedUpdatingRequest request, CancellationToken cancellationToken = default)
    {
        List<EPGFileDto> epgFiles = await Repository.EPGFile.GetEPGFilesNeedUpdating();
        return APIResponse<List<EPGFileDto>>.Success(epgFiles);
    }
}