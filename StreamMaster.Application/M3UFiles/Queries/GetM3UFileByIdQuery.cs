using StreamMaster.Application.M3UFiles.Commands;

namespace StreamMaster.Application.M3UFiles.Queries;

public record GetM3UFileByIdQuery(int Id) : IRequest<M3UFileDto?>;

internal class GetM3UFileByIdQueryHandler(ILogger<CreateM3UFileRequestHandler> Logger, IRepositoryWrapper Repository, IMapper Mapper) : IRequestHandler<GetM3UFileByIdQuery, M3UFileDto?>
{
    public async Task<M3UFileDto?> Handle(GetM3UFileByIdQuery request, CancellationToken cancellationToken = default)
    {
        M3UFile? ret = await Repository.M3UFile.GetM3UFileById(request.Id);
        return ret == null ? null : Mapper.Map<M3UFileDto>(ret);
    }
}
