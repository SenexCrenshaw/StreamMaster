namespace StreamMaster.Application.M3UFiles.Queries;

public record GetM3UFilesNeedUpdating() : IRequest<APIResponse<List<M3UFileDto>>>;

internal class GetM3UFilesNeedUpdatingHandler(ILogger<GetM3UFilesNeedUpdating> logger, IRepositoryWrapper Repository)
    : IRequestHandler<GetM3UFilesNeedUpdating, APIResponse<List<M3UFileDto>>>
{
    public async Task<APIResponse<List<M3UFileDto>>> Handle(GetM3UFilesNeedUpdating request, CancellationToken cancellationToken = default)
    {
        List<M3UFileDto> M3UFilesToUpdated = await Repository.M3UFile.GetM3UFilesNeedUpdating();
        return APIResponse<List<M3UFileDto>>.Success(M3UFilesToUpdated);
    }
}