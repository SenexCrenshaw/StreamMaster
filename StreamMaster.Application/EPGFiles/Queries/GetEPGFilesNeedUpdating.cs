namespace StreamMaster.Application.EPGFiles.Queries;

public record GetEPGFilesNeedUpdating() : IRequest<IEnumerable<EPGFileDto>>;

internal class GetEPGFilesNeedUpdatingHandler(ILogger<GetEPGFilesNeedUpdating> logger, IRepositoryWrapper Repository)
    : IRequestHandler<GetEPGFilesNeedUpdating, IEnumerable<EPGFileDto>>
{
    public async Task<IEnumerable<EPGFileDto>> Handle(GetEPGFilesNeedUpdating request, CancellationToken cancellationToken = default)
    {
        List<EPGFileDto> epgFiles = await Repository.EPGFile.GetEPGFilesNeedUpdating();
        return epgFiles;
    }
}