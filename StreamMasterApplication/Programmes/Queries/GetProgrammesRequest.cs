using StreamMasterApplication.Services;

using StreamMasterDomain.EPG;

using System.Web;

namespace StreamMasterApplication.Programmes.Queries;

public record GetProgrammesRequest : IRequest<List<Programme>>;

public class GetProgrammesRequestHandler(ILogger<GetProgrammesRequest> logger, ISDService sdService, IRepositoryWrapper repository, IMapper mapper, ISettingsService settingsService, IPublisher publisher, ISender sender, IHubContext<StreamMasterHub, IStreamMasterHub> hubContext, IMemoryCache memoryCache)
: BaseMediatorRequestHandler(logger, repository, mapper, settingsService, publisher, sender, hubContext, memoryCache), IRequestHandler<GetProgrammesRequest, List<Programme>>
{
    public async Task<List<Programme>> Handle(GetProgrammesRequest request, CancellationToken cancellationToken)
    {
        List<Programme> programmes = MemoryCache.Programmes();

        Setting setting = await GetSettingsAsync();
        if (setting.SDEnabled)
        {
            List<Programme> sdprogrammes = await sdService.GetProgrammes(cancellationToken).ConfigureAwait(false);
            programmes = programmes.Concat(sdprogrammes).OrderBy(a => a.Channel).ToList();
        }

        List<IconFileDto> icons = MemoryCache.Icons();

        foreach (Programme? prog in programmes.Where(a => a.Icon.Any()))
        {
            foreach (TvIcon progIcon in prog.Icon)
            {
                if (progIcon != null && !string.IsNullOrEmpty(progIcon.Src))
                {
                    IconFileDto? icon = icons.Find(a => a.SMFileType == SMFileTypes.ProgrammeIcon && a.Source == progIcon.Src);
                    if (icon == null)
                    {
                        continue;
                    }
                    progIcon.Src = $"/api/files/{(int)SMFileTypes.ProgrammeIcon}/{HttpUtility.UrlEncode(icon.Source)}";
                }
            }
        }

        return programmes;
    }
}
