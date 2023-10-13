using StreamMasterDomain.Pagination;

namespace StreamMasterApplication.Programmes.Queries;
public record GetProgrammsSimpleQuery(ProgrammeParameters Parameters) : IRequest<List<ProgrammeNameDto>>;

internal class GetProgrammsSimpleQueryHandler : BaseMediatorRequestHandler, IRequestHandler<GetProgrammsSimpleQuery, List<ProgrammeNameDto>>
{

    public GetProgrammsSimpleQueryHandler(ILogger<GetProgrammsSimpleQuery> logger, IRepositoryWrapper repository, IMapper mapper, ISettingsService settingsService, IPublisher publisher, ISender sender, IHubContext<StreamMasterHub, IStreamMasterHub> hubContext, IMemoryCache memoryCache)
    : base(logger, repository, mapper, settingsService, publisher, sender, hubContext, memoryCache) { }

    public async Task<List<ProgrammeNameDto>> Handle(GetProgrammsSimpleQuery request, CancellationToken cancellationToken)
    {
        List<ProgrammeNameDto> ret = new();

        // Retrieve and filter Programmes
        IEnumerable<ProgrammeNameDto> filteredProgrammes = await Sender.Send(new GetProgrammeNamesDto(), cancellationToken).ConfigureAwait(false);

        if (filteredProgrammes.Any())
        {
            // Get distinct channel names in order and take the required amount
            List<string> distinctChannels = filteredProgrammes
                .OrderBy(a => a.DisplayName, StringComparer.OrdinalIgnoreCase)
                .Select(a => a.Channel)
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .Skip(request.Parameters.First)
                .Take(request.Parameters.Count)
                .ToList();

            foreach (string channel in distinctChannels)
            {
                ProgrammeNameDto? programme = filteredProgrammes.FirstOrDefault(a => a.Channel == channel);
                if (programme != null)
                {
                    //ProgrammeNameDto programmeDto = Mapper.Map<ProgrammeNameDto>(programme);
                    ret.Add(programme);
                }
            }

            return ret;
        }

        return new List<ProgrammeNameDto>();
    }
}