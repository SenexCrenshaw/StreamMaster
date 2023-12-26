using StreamMaster.Domain.Dto;
using StreamMaster.Domain.Repository;
using StreamMaster.Domain.Services;

namespace StreamMaster.Application.Programmes.Queries;

public record GetProgrammeNames : IRequest<List<string>>;

internal class GetProgrammeNamesHandler(ILogger<GetProgrammeNames> logger, IRepositoryWrapper repository, IMapper mapper, ISettingsService settingsService, IPublisher publisher, ISender sender, IHubContext<StreamMasterHub, IStreamMasterHub> hubContext, IMemoryCache memoryCache) : BaseMediatorRequestHandler(logger, repository, mapper, settingsService, publisher, sender, hubContext, memoryCache), IRequestHandler<GetProgrammeNames, List<string>>
{
    public async Task<List<string>> Handle(GetProgrammeNames request, CancellationToken cancellationToken)
    {
        IEnumerable<ProgrammeNameDto> programmes = await Sender.Send(new GetProgrammeNamesDto(), cancellationToken).ConfigureAwait(false);

        List<string> ret = programmes
            .Where(a => !string.IsNullOrEmpty(a.Channel))
            .OrderBy(a => a.DisplayName, StringComparer.OrdinalIgnoreCase)
            .Select(a => a.DisplayName).Distinct().ToList();

        return ret;
    }
}
