using StreamMasterDomain.Pagination;
using StreamMasterDomain.Repository.EPG;

namespace StreamMasterApplication.Programmes.Queries;
public record GetProgrammsSimpleQuery(ProgrammeParameters Parameters) : IRequest<List<ProgrammeNameDto>>;

internal class GetProgrammsSimpleQueryHandler : BaseMemoryRequestHandler, IRequestHandler<GetProgrammsSimpleQuery, List<ProgrammeNameDto>>
{

    public GetProgrammsSimpleQueryHandler(ILogger<GetProgrammsSimpleQuery> logger, IRepositoryWrapper repository, IMapper mapper,ISettingsService settingsService, IPublisher publisher, ISender sender, IHubContext<StreamMasterHub, IStreamMasterHub> hubContext, IMemoryCache memoryCache)
    : base(logger, repository, mapper,settingsService, publisher, sender, hubContext, memoryCache) { }

    public async Task<List<ProgrammeNameDto>> Handle(GetProgrammsSimpleQuery request, CancellationToken cancellationToken)
    {
        List<ProgrammeNameDto> ret = new();

        // Retrieve and filter Programmes
        List<Programme> filteredProgrammes = MemoryCache.Programmes()
            .Where(a => !string.IsNullOrEmpty(a.Channel))
            .ToList();

        if (filteredProgrammes.Any())
        {
            // Get distinct channel names in order and take the required amount
            List<string> distinctChannels = filteredProgrammes
                .Select(a => a.Channel)
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .OrderBy(a => a, StringComparer.OrdinalIgnoreCase)
                .Skip(request.Parameters.First)
                .Take(request.Parameters.Count)
                .ToList();

            foreach (string channel in distinctChannels)
            {
                Programme? programme = filteredProgrammes.FirstOrDefault(a => a.Channel == channel);
                if (programme != null)
                {
                    ProgrammeNameDto programmeDto;
                    programmeDto = Mapper.Map<ProgrammeNameDto>(programme);
                    ret.Add(programmeDto);
                }
            }

            return ret;
        }

        return new List<ProgrammeNameDto>();
    }
}