using StreamMasterDomain.EPG;
using System.Web;

namespace StreamMasterApplication.Programmes.Queries;

public record GetProgrammes : IRequest<IEnumerable<Programme>>;

internal class GetProgrammesHandler(IMemoryCache memoryCache) : IRequestHandler<GetProgrammes, IEnumerable<Programme>>
{
    public Task<IEnumerable<Programme>> Handle(GetProgrammes request, CancellationToken cancellationToken)
    {
        IEnumerable<Programme> programmes = memoryCache.Programmes().ToList();
        if (programmes == null)
        {
            return Task.FromResult<IEnumerable<Programme>>(new List<Programme>());
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

        return Task.FromResult(programmes);
    }
}
