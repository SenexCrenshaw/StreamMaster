
using System.Web;

namespace StreamMasterApplication.Programmes.Queries;

public record GetProgrammesRequest : IRequest<List<EPGProgramme>>;

public class GetProgrammesRequestHandler(ILogger<GetProgrammesRequest> logger, IRepositoryWrapper repository, IMapper mapper, ISettingsService settingsService, IPublisher publisher, ISender sender, IHubContext<StreamMasterHub, IStreamMasterHub> hubContext, IMemoryCache memoryCache)
: BaseMediatorRequestHandler(logger, repository, mapper, settingsService, publisher, sender, hubContext, memoryCache), IRequestHandler<GetProgrammesRequest, List<EPGProgramme>>
{
    public async Task<List<EPGProgramme>> Handle(GetProgrammesRequest request, CancellationToken cancellationToken)
    {
        List<EPGProgramme> programmes = MemoryCache.Programmes();

        Setting setting = await GetSettingsAsync();
        //if (setting.SDEnabled)
        //{
        //    List<Programme> sdprogrammes = schedulesDirect.GetProgrammes();
        //    programmes = programmes.Concat(sdprogrammes).OrderBy(a => a.Channel).ToList();
        //}

        //List<IconFileDto> icons = MemoryCache.Icons();

        //foreach (Programme? prog in programmes.Where(a => a.Icon.Any()))
        //{
        //    foreach (TvIcon progIcon in prog.Icon)
        //    {
        //        if (progIcon != null && !string.IsNullOrEmpty(progIcon.Src))
        //        {
        //            IconFileDto? icon = icons.Find(a => a.SMFileType == SMFileTypes.ProgrammeIcon && a.Source == progIcon.Src);
        //            if (icon == null)
        //            {
        //                continue;
        //            }
        //            progIcon.Src = $"/api/files/{(int)SMFileTypes.ProgrammeIcon}/{HttpUtility.UrlEncode(icon.Source)}";
        //        }
        //    }
        //}

        return programmes;
    }
}