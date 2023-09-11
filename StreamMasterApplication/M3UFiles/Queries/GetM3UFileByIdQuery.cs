using StreamMasterApplication.M3UFiles.Commands;

namespace StreamMasterApplication.M3UFiles.Queries;

public record GetM3UFileByIdQuery(int Id) : IRequest<M3UFileDto?>;

internal class GetM3UFileByIdQueryHandler : BaseRequestHandler, IRequestHandler<GetM3UFileByIdQuery, M3UFileDto?>
{

    public GetM3UFileByIdQueryHandler(ILogger<CreateM3UFileRequestHandler> logger, IRepositoryWrapper repository, IMapper mapper, ISettingsService settingsService)
        : base(logger, repository, mapper, settingsService) { }

    public async Task<M3UFileDto?> Handle(GetM3UFileByIdQuery request, CancellationToken cancellationToken = default)
    {
        M3UFile m3uFile = await Repository.M3UFile.GetM3UFileByIdAsync(request.Id);
        M3UFileDto? ret = Mapper.Map<M3UFileDto?>(m3uFile);
        return ret;
    }
}
