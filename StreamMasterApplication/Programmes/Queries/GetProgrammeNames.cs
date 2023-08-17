using AutoMapper;

using MediatR;

using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

using StreamMasterDomain.Cache;
using StreamMasterDomain.Dto;
using StreamMasterDomain.Repository.EPG;

namespace StreamMasterApplication.Programmes.Queries;

public record GetProgrammeNames : IRequest<List<ProgrammeNameDto>>;

internal class GetProgrammeNamesHandler : BaseMemoryRequestHandler, IRequestHandler<GetProgrammeNames, List<ProgrammeNameDto>>
{

    public GetProgrammeNamesHandler(ILogger<GetProgrammeNamesHandler> logger, IRepositoryWrapper repository, IMapper mapper, IPublisher publisher, ISender sender, IMemoryCache memoryCache)
        : base(logger, repository, mapper, publisher, sender, memoryCache) { }

    public Task<List<ProgrammeNameDto>> Handle(GetProgrammeNames request, CancellationToken cancellationToken)
    {
        List<ProgrammeNameDto> ret = new();

        List<Programme> programmes = MemoryCache.Programmes().Where(a => !string.IsNullOrEmpty(a.Channel) && a.StopDateTime > DateTime.Now.AddDays(-1)).ToList();
        if (programmes.Any())
        {
            List<string> names = programmes.Select(a => a.Channel).Distinct().Order().ToList();
            foreach (string? name in names)
            {
                Programme? programme = programmes.FirstOrDefault(a => a.Channel == name);
                if (programme != null)
                {
                    ProgrammeNameDto programmeDto = Mapper.Map<ProgrammeNameDto>(programme);
                    ret.Add(programmeDto);
                }
            }

            return Task.FromResult(ret);
        }

        return Task.FromResult(new List<ProgrammeNameDto>());
    }
}
