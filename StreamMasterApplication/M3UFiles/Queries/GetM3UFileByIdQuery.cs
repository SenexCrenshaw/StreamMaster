using StreamMasterApplication.M3UFiles.Commands;

namespace StreamMasterApplication.M3UFiles.Queries;

public record GetM3UFileByIdQuery(int Id) : IRequest<M3UFileDto?>;

internal class GetM3UFileByIdQueryHandler(ILogger<CreateM3UFileRequestHandler> logger, IRepositoryWrapper repository, IMapper mapper, ISettingsService settingsService) : BaseRequestHandler(logger, repository, mapper, settingsService), IRequestHandler<GetM3UFileByIdQuery, M3UFileDto?>
{
    public async Task<M3UFileDto?> Handle(GetM3UFileByIdQuery request, CancellationToken cancellationToken = default)
    {
        StreamMasterDomain.Models.M3UFile? ret = await Repository.M3UFile.GetM3UFileById(request.Id);
        if (ret == null)
        {
            return null;
        }

        return Mapper.Map<M3UFileDto>(ret);
    }
}
