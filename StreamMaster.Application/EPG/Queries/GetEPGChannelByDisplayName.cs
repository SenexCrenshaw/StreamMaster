using StreamMaster.Domain.Dto;
using StreamMaster.Domain.Repository;
using StreamMaster.Domain.Services;

using StreamMaster.Application.Programmes.Queries;

namespace StreamMaster.Application.EPG.Queries;

public record GetEPGChannelByDisplayName(string DisplayName) : IRequest<ProgrammeNameDto?>;

internal class GetEPGChannelByDisplayNameHandler(ILogger<GetEPGChannelByDisplayName> logger, IRepositoryWrapper repository, IMapper mapper, ISettingsService settingsService, IPublisher publisher, ISender sender, IHubContext<StreamMasterHub, IStreamMasterHub> hubContext, IMemoryCache memoryCache) : BaseMediatorRequestHandler(logger, repository, mapper, settingsService, publisher, sender, hubContext, memoryCache), IRequestHandler<GetEPGChannelByDisplayName, ProgrammeNameDto?>
{
    public async Task<ProgrammeNameDto?> Handle(GetEPGChannelByDisplayName request, CancellationToken cancellationToken = default)
    {
        IEnumerable<ProgrammeNameDto> programmeNames = await Sender.Send(new GetProgrammeNamesDto(), cancellationToken).ConfigureAwait(false);
        ProgrammeNameDto? pn = programmeNames.FirstOrDefault(a => a.DisplayName == request.DisplayName);
        if (pn == null)
        {
            pn = programmeNames.FirstOrDefault(a => a.ChannelName == request.DisplayName);
            if (pn == null)
            {
                return programmeNames.FirstOrDefault(a => a.Channel == request.DisplayName); ;
            }
        }
        return pn;
    }
}
