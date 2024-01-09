namespace StreamMaster.Application.EPGFiles.Queries;

public record GetEPGFilePreviewById(int Id) : IRequest<List<EPGFilePreviewDto>>;

internal class GetEPGFilePreviewByIdHandler(ILogger<GetEPGFilePreviewById> logger, IRepositoryWrapper Repository)
    : IRequestHandler<GetEPGFilePreviewById, List<EPGFilePreviewDto>>
{
    public async Task<List<EPGFilePreviewDto>> Handle(GetEPGFilePreviewById request, CancellationToken cancellationToken = default)
    {
        List<EPGFilePreviewDto> res = await Repository.EPGFile.GetEPGFilePreviewById(request.Id, cancellationToken).ConfigureAwait(false);

        return res;
    }
}
