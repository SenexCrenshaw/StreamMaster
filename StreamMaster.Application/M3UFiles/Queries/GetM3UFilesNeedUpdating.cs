namespace StreamMaster.Application.M3UFiles.Queries;

public record GetM3UFilesNeedUpdating() : IRequest<DataResponse<List<M3UFileDto>>>;

internal class GetM3UFilesNeedUpdatingHandler(IRepositoryWrapper Repository)
    : IRequestHandler<GetM3UFilesNeedUpdating, DataResponse<List<M3UFileDto>>>
{
    public async Task<DataResponse<List<M3UFileDto>>> Handle(GetM3UFilesNeedUpdating request, CancellationToken cancellationToken = default)
    {
        List<M3UFileDto> M3UFilesToUpdated = await Repository.M3UFile.GetM3UFilesNeedUpdating();
        return DataResponse<List<M3UFileDto>>.Success(M3UFilesToUpdated);
    }
}