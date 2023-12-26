using StreamMaster.Domain.Dto;
using StreamMaster.Domain.Repository;
using StreamMaster.Domain.Services;

using StreamMaster.Application.Programmes.Queries;

namespace StreamMaster.Application.EPG.Queries;

public record GetEPGNameTvgName(string User_Tvg_Name) : IRequest<string?>;

internal class GetEPGNameTvgNameHandler : BaseMediatorRequestHandler, IRequestHandler<GetEPGNameTvgName, string?>
{

    public GetEPGNameTvgNameHandler(ILogger<GetEPGNameTvgName> logger, IRepositoryWrapper repository, IMapper mapper, ISettingsService settingsService, IPublisher publisher, ISender sender, IHubContext<StreamMasterHub, IStreamMasterHub> hubContext, IMemoryCache memoryCache)
: base(logger, repository, mapper, settingsService, publisher, sender, hubContext, memoryCache) { }
    public async Task<string?> Handle(GetEPGNameTvgName request, CancellationToken cancellationToken = default)
    {
        IEnumerable<ProgrammeNameDto> programmeNames = await Sender.Send(new GetProgrammeNamesDto(), cancellationToken).ConfigureAwait(false);

        ProgrammeNameDto? pn = programmeNames.FirstOrDefault(a => a.DisplayName == request.User_Tvg_Name);
        if (pn == null)
        {
            pn = programmeNames.FirstOrDefault(a => a.ChannelName == request.User_Tvg_Name);
            if (pn == null)
            {
                return null;
            }
        }
        return request.User_Tvg_Name;
    }
}
