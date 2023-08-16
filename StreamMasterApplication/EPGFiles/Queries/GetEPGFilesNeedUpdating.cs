using AutoMapper;

using MediatR;

using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

using StreamMasterDomain.Dto;

namespace StreamMasterApplication.EPGFiles.Queries;

public record GetEPGFilesNeedUpdating() : IRequest<IEnumerable<EPGFilesDto>>;

internal class GetEPGFilesNeedUpdatingHandler : BaseMemoryRequestHandler, IRequestHandler<GetEPGFilesNeedUpdating, IEnumerable<EPGFilesDto>>
{
    public GetEPGFilesNeedUpdatingHandler(ILogger<GetEPGFilesNeedUpdatingHandler> logger, IRepositoryWrapper repository, IMapper mapper, IPublisher publisher, ISender sender, IMemoryCache memoryCache)
        : base(logger, repository, mapper, publisher, sender, memoryCache) { }

    public async Task<IEnumerable<EPGFilesDto>> Handle(GetEPGFilesNeedUpdating request, CancellationToken cancellationToken = default)
    {
        DateTime now = DateTime.Now;

        List<EPGFile> epgFiles = new();
        IEnumerable<EPGFile> epgFilesRepo = await Repository.EPGFile.GetAllEPGFilesAsync();
        IEnumerable<EPGFile> epgFilesToUpdated = epgFilesRepo.Where(a => a.AutoUpdate && !string.IsNullOrEmpty(a.Url) && a.HoursToUpdate > 0 && a.LastDownloaded.AddHours(a.HoursToUpdate) < now);
        epgFiles.AddRange(epgFilesToUpdated);
        foreach (EPGFile? epgFile in epgFilesRepo.Where(a => string.IsNullOrEmpty(a.Url)))
        {
            if (epgFile.LastWrite() >= epgFile.LastUpdated)
            {
                epgFiles.Add(epgFile);
            }
        }
        IEnumerable<EPGFilesDto> ret = Mapper.Map<IEnumerable<EPGFilesDto>>(epgFiles);
        return ret;
    }
}