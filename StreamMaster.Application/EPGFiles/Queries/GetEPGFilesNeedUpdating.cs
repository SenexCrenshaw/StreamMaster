using StreamMaster.Domain.Dto;
using StreamMaster.Domain.Repository;
using StreamMaster.Domain.Services;

namespace StreamMaster.Application.EPGFiles.Queries;

public record GetEPGFilesNeedUpdating() : IRequest<IEnumerable<EPGFileDto>>;

internal class GetEPGFilesNeedUpdatingHandler : BaseMediatorRequestHandler, IRequestHandler<GetEPGFilesNeedUpdating, IEnumerable<EPGFileDto>>
{

    public GetEPGFilesNeedUpdatingHandler(ILogger<GetEPGFilesNeedUpdating> logger, IRepositoryWrapper repository, IMapper mapper, ISettingsService settingsService, IPublisher publisher, ISender sender, IHubContext<StreamMasterHub, IStreamMasterHub> hubContext, IMemoryCache memoryCache)
 : base(logger, repository, mapper, settingsService, publisher, sender, hubContext, memoryCache) { }

    public async Task<IEnumerable<EPGFileDto>> Handle(GetEPGFilesNeedUpdating request, CancellationToken cancellationToken = default)
    {
        List<EPGFileDto> epgFiles = await Repository.EPGFile.GetEPGFilesNeedUpdating();
        return epgFiles;
    }
}