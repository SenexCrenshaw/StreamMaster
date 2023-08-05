using MediatR;

using Microsoft.Extensions.Caching.Memory;

using StreamMasterDomain.Repository.EPG;

using System.Web;

namespace StreamMasterApplication.Programmes.Queries;

public record GetProgramme(string Channel) : IRequest<IEnumerable<Programme>?>;

internal class GetProgrammeHandler : IRequestHandler<GetProgramme, IEnumerable<Programme>?>
{
    private readonly IMemoryCache _memoryCache;
    private readonly ISender _sender;

    public GetProgrammeHandler(
           ISender sender,
        IMemoryCache memoryCache
        )
    {
        _sender = sender;
        _memoryCache = memoryCache;
    }

    public async Task<IEnumerable<Programme>?> Handle(GetProgramme request, CancellationToken cancellationToken)
    {
        IEnumerable<Programme> programmes = _memoryCache.Programmes().Where(a => a.Channel.ToLower() == request.Channel.ToLower());
        if (programmes == null)
        {
            return null;
        }
        StreamMasterDomain.Dto.SettingDto setting = await _sender.Send(new GetSettings(), cancellationToken).ConfigureAwait(false);

        var icons = _memoryCache.Icons();

        foreach (Programme? prog in programmes.Where(a => a.Icon.Any()))
        {
            foreach (TvIcon progIcon in prog.Icon)
            {
                if (progIcon != null && !string.IsNullOrEmpty(progIcon.Src))
                {
                    var icon = icons.FirstOrDefault(a => a.SMFileType == SMFileTypes.ProgrammeIcon && a.Source == progIcon.Src);
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
