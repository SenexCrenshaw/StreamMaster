using StreamMaster.Application.M3UFiles.CommandsOrig;

namespace StreamMaster.Application.M3UFiles.Queries;

public record GetM3UFileQuery(int Id) : IRequest<M3UFileDto?>;

internal class GetM3UFileQueryHandler(ILogger<CreateM3UFileRequestHandler> Logger, IRepositoryWrapper Repository, IMapper Mapper) : IRequestHandler<GetM3UFileQuery, M3UFileDto?>
{
    public async Task<M3UFileDto?> Handle(GetM3UFileQuery request, CancellationToken cancellationToken = default)
    {
        M3UFile? ret = await Repository.M3UFile.GetM3UFile(request.Id);
        return ret == null ? null : Mapper.Map<M3UFileDto>(ret);
    }
}
