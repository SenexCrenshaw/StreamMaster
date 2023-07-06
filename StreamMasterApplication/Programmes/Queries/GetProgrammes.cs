using AutoMapper;

using MediatR;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

using StreamMasterDomain.Entities.EPG;

using System.Web;

namespace StreamMasterApplication.Programmes.Queries;

public record GetProgrammes : IRequest<IEnumerable<Programme>>;

internal class GetProgrammesHandler : IRequestHandler<GetProgrammes, IEnumerable<Programme>>
{
    private readonly IAppDbContext _context;
    private readonly IMapper _mapper;
    private readonly IMemoryCache _memoryCache;
    private readonly ISender _sender;

    public GetProgrammesHandler(
        IMapper mapper,
        ISender sender,
        IAppDbContext context,
       IMemoryCache memoryCache
    )
    {
        _sender = sender;
        _context = context;
        _mapper = mapper;
        _memoryCache = memoryCache;
    }

    public async Task<IEnumerable<Programme>> Handle(GetProgrammes request, CancellationToken cancellationToken)
    {
        IEnumerable<Programme> programmes = _memoryCache.Programmes().Where(a => a.StopDateTime > DateTime.Now.AddDays(-1)).ToList();
        if (programmes == null)
        {
            return new List<Programme>();
        }
        StreamMasterDomain.Dto.SettingDto setting = await _sender.Send(new GetSettings(), cancellationToken).ConfigureAwait(false);

        foreach (Programme? prog in programmes.Where(a => a.Icon.Any()))
        {
            foreach (TvIcon progIcon in prog.Icon)
            {
                if (progIcon != null && !string.IsNullOrEmpty(progIcon.Src))
                {
                    IconFile? icon = await _context.Icons.FirstOrDefaultAsync(a => a.SMFileType == SMFileTypes.ProgrammeIcon && a.Source == progIcon.Src, cancellationToken: cancellationToken).ConfigureAwait(false);
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