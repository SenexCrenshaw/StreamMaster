namespace StreamMaster.Application.EPGFiles.Queries;

public record GetEPGFilesNeedUpdatingRequest() : IRequest<List<EPGFileDto>>;

internal class GetEPGFilesNeedUpdatingHandler(IRepositoryWrapper Repository)
    : IRequestHandler<GetEPGFilesNeedUpdatingRequest, List<EPGFileDto>>
{
    public async Task<List<EPGFileDto>> Handle(GetEPGFilesNeedUpdatingRequest request, CancellationToken cancellationToken = default)
    {
        List<EPGFileDto> epgFiles = await Repository.EPGFile.GetEPGFilesNeedUpdating();
        return epgFiles;
    }
}