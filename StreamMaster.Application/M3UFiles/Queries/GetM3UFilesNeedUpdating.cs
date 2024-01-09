namespace StreamMaster.Application.M3UFiles.Queries;

public record GetM3UFilesNeedUpdating() : IRequest<IEnumerable<M3UFileDto>>;

internal class GetM3UFilesNeedUpdatingHandler(ILogger<GetM3UFilesNeedUpdating> logger, IRepositoryWrapper Repository)
    : IRequestHandler<GetM3UFilesNeedUpdating, IEnumerable<M3UFileDto>>
{
    public async Task<IEnumerable<M3UFileDto>> Handle(GetM3UFilesNeedUpdating request, CancellationToken cancellationToken = default)
    {
        List<M3UFileDto> M3UFilesToUpdated = await Repository.M3UFile.GetM3UFilesNeedUpdating();
        return M3UFilesToUpdated;
    }
}