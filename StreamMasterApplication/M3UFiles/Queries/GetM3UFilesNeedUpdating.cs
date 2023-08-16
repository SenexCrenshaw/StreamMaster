using AutoMapper;

using MediatR;

using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

using StreamMasterApplication.M3UFiles.Commands;

using StreamMasterDomain.Dto;

namespace StreamMasterApplication.M3UFiles.Queries;

public record GetM3UFilesNeedUpdating() : IRequest<IEnumerable<M3UFileDto>>;

internal class GetM3UFilesNeedUpdatingHandler : BaseMemoryRequestHandler, IRequestHandler<GetM3UFilesNeedUpdating, IEnumerable<M3UFileDto>>
{
    public GetM3UFilesNeedUpdatingHandler(ILogger<ProcessM3UFileRequestHandler> logger, IRepositoryWrapper repository, IMapper mapper, IPublisher publisher, ISender sender, IMemoryCache memoryCache)
        : base(logger, repository, mapper, publisher, sender, memoryCache) { }

    public async Task<IEnumerable<M3UFileDto>> Handle(GetM3UFilesNeedUpdating request, CancellationToken cancellationToken = default)
    {
        DateTime now = DateTime.Now;

        List<M3UFile> M3UFiles = new();
        IEnumerable<M3UFile> M3UFilesRepo = await Repository.M3UFile.GetAllM3UFilesAsync();
        IEnumerable<M3UFile> M3UFilesToUpdated = M3UFilesRepo.Where(a => a.AutoUpdate && !string.IsNullOrEmpty(a.Url) && a.HoursToUpdate > 0 && a.LastDownloaded.AddHours(a.HoursToUpdate) < now);
        M3UFiles.AddRange(M3UFilesToUpdated);
        foreach (M3UFile? M3UFile in M3UFilesRepo.Where(a => string.IsNullOrEmpty(a.Url)))
        {
            if (M3UFile.LastWrite() >= M3UFile.LastUpdated)
            {
                M3UFiles.Add(M3UFile);
            }
        }
        IEnumerable<M3UFileDto> ret = Mapper.Map<IEnumerable<M3UFileDto>>(M3UFiles);
        return ret;
    }
}