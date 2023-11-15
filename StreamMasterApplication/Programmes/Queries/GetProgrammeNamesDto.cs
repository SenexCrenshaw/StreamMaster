using StreamMaster.SchedulesDirectAPI.Domain.EPG;

namespace StreamMasterApplication.Programmes.Queries;

public record GetProgrammeNamesDto : IRequest<IEnumerable<ProgrammeNameDto>>;

internal class GetProgrammeNamesDtoHandler(ILogger<GetProgrammeNamesDto> logger, IRepositoryWrapper repository, IMapper mapper, ISettingsService settingsService, IPublisher publisher, ISender sender, IHubContext<StreamMasterHub, IStreamMasterHub> hubContext, IMemoryCache memoryCache) : BaseMediatorRequestHandler(logger, repository, mapper, settingsService, publisher, sender, hubContext, memoryCache), IRequestHandler<GetProgrammeNamesDto, IEnumerable<ProgrammeNameDto>>
{
    public async Task<IEnumerable<ProgrammeNameDto>> Handle(GetProgrammeNamesDto request, CancellationToken cancellationToken)
    {
        List<Programme> programmes = await Sender.Send(new GetProgrammesRequest(), cancellationToken).ConfigureAwait(false);

        if (programmes.Any())
        {
            IEnumerable<ProgrammeNameDto> progs = programmes.GroupBy(a => a.Channel).Select(group => group.First()).Select(a => new ProgrammeNameDto
            {
                Channel = a.Channel,
                ChannelName = a.ChannelName,
                DisplayName = a.DisplayName
            });

            List<ProgrammeNameDto> ret = progs.OrderBy(a => a.DisplayName).ToList();

            ret.Insert(0, new ProgrammeNameDto
            {
                Channel = "Dummy",
                ChannelName = "Dummy",
                DisplayName = "Dummy"
            });

            return ret;
        }

        return new List<ProgrammeNameDto>();
    }
}
