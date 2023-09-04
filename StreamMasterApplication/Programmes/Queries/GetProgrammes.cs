using MediatR;

using Microsoft.Extensions.Caching.Memory;

using StreamMasterDomain.Cache;
using StreamMasterDomain.Repository.EPG;

using System.Web;

namespace StreamMasterApplication.Programmes.Queries;

public record GetProgrammes : IRequest<IEnumerable<Programme>>;

internal class GetProgrammesHandler : IRequestHandler<GetProgrammes, IEnumerable<Programme>>
{
    private readonly IMemoryCache _memoryCache;

    public GetProgrammesHandler(

       IMemoryCache memoryCache
    )
    {
        _memoryCache = memoryCache;
    }

    public async Task<IEnumerable<Programme>> Handle(GetProgrammes request, CancellationToken cancellationToken)
    {
        IEnumerable<Programme> programmes = _memoryCache.Programmes().ToList();
        if (programmes == null)
        {
            return new List<Programme>();
        }

        Setting setting = FileUtil.GetSetting();
        List<StreamMasterDomain.Dto.IconFileDto> icons = _memoryCache.Icons();

        foreach (Programme? prog in programmes.Where(a => a.Icon.Any()))
        {
            foreach (TvIcon progIcon in prog.Icon)
            {
                if (progIcon != null && !string.IsNullOrEmpty(progIcon.Src))
                {
                    StreamMasterDomain.Dto.IconFileDto? icon = icons.FirstOrDefault(a => a.SMFileType == SMFileTypes.ProgrammeIcon && a.Source == progIcon.Src);
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
