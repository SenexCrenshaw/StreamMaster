using StreamMasterApplication.M3UFiles.Commands;

namespace StreamMasterApplication.M3UFiles.Queries;

public record GetM3UFileByIdQuery(int Id) : IRequest<M3UFileDto?>;

internal class GetM3UFileByIdQueryHandler : BaseRequestHandler, IRequestHandler<GetM3UFileByIdQuery, M3UFileDto?>
{

    public GetM3UFileByIdQueryHandler(ILogger<CreateM3UFileRequestHandler> logger, IRepositoryWrapper repository, IMapper mapper, ISettingsService settingsService)
        : base(logger, repository, mapper, settingsService) { }

    public async Task<M3UFileDto?> Handle(GetM3UFileByIdQuery request, CancellationToken cancellationToken = default)
    {
        M3UFileDto? ret = await Repository.M3UFile.GetM3UFileById(request.Id);
        return ret;
    }
}
