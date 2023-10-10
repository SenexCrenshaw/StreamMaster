using StreamMasterApplication.Services;

using StreamMasterDomain.EPG;

using System.Web;

namespace StreamMasterApplication.Programmes.Queries;

public record GetProgrammes : IRequest<List<Programme>>;

internal class GetProgrammesHandler(IMemoryCache memoryCache, ISDService sdService) : IRequestHandler<GetProgrammes, List<Programme>>
{
    public async Task<List<Programme>> Handle(GetProgrammes request, CancellationToken cancellationToken)
    {
        List<Programme> cacheValues = memoryCache.Programmess();
        List<Programme> sdprogrammes = await sdService.GetProgrammes(cancellationToken).ConfigureAwait(false);
        List<Programme> programmes = cacheValues.Concat(sdprogrammes).OrderBy(a => a.Channel).ToList();

        if (programmes == null)
        {
            return new();
        }

        List<IconFileDto> icons = memoryCache.Icons();

        foreach (Programme? prog in programmes.Where(a => a.Icon.Any()))
        {
            foreach (TvIcon progIcon in prog.Icon)
            {
                if (progIcon != null && !string.IsNullOrEmpty(progIcon.Src))
                {
                    IconFileDto? icon = icons.FirstOrDefault(a => a.SMFileType == SMFileTypes.ProgrammeIcon && a.Source == progIcon.Src);
                    if (icon == null)
                    {
                        continue;
                    }
                    string IconSource = $"/api/files/{(int)SMFileTypes.ProgrammeIcon}/{HttpUtility.UrlEncode(icon.Source)}";
                    progIcon.Src = IconSource;
                }
            }
        }

        return programmes;
    }
}
