using AutoMapper;

using MediatR;

using Microsoft.Extensions.Caching.Memory;

using StreamMasterDomain.Dto;

namespace StreamMasterApplication.Icons.Queries;

public record GetIcons : IRequest<List<IconFileDto>>;

internal class GetIconsQueryHandler : IRequestHandler<GetIcons, List<IconFileDto>>
{
    private readonly IAppDbContext _context;
    private readonly IMapper _mapper;
    private readonly IMemoryCache _memoryCache;

    private readonly ISender _sender;

    public GetIconsQueryHandler(

        IMemoryCache memoryCache,
        IAppDbContext context,
        ISender sender,
        IMapper mapper
        )
    {
        _memoryCache = memoryCache;

        _sender = sender;
        _mapper = mapper;

        _context = context;
    }

    public async Task<List<IconFileDto>> Handle(GetIcons request, CancellationToken cancellationToken)
    {
        //if (!_memoryCache.TryGetValue(CacheKeys.ListIconFiles, out List<IconFileDto>? cacheValue))
        //{
        //    SettingDto setting = await _sender.Send(new GetSettings(), cancellationToken).ConfigureAwait(false);

        // List<IconFileDto> icons = await _context.Icons.Where(a =>
        // a.FileExists) .AsNoTracking() .Where(a => a.FileExists &&
        // a.SMFileType == SMFileTypes.Icon)
        // .ProjectTo<IconFileDto>(_mapper.ConfigurationProvider) .OrderBy(x =>
        // x.Name) .ToListAsync(cancellationToken).ConfigureAwait(false);

        // IEnumerable<IconFileDto> tvlogos = _mapper.Map<IEnumerable<IconFileDto>>(_memoryCache.TvLogos());

        // List<IconFileDto> allIcons = tvlogos.Concat(icons).ToList();

        // int count = 0;

        // Stopwatch sw = Stopwatch.StartNew(); ParallelOptions po = new() {
        // CancellationToken = cancellationToken, MaxDegreeOfParallelism =
        // Environment.ProcessorCount };

        // _ = Parallel.ForEach(allIcons, po, icon => { string IconSource =
        // Helpers.GetIPTVChannelIconSources(icon.Source, setting, "/",
        // allIcons); icon.Id = count++; icon.Source = IconSource; icon.Url =
        // IconSource; });

        // sw.Stop(); long el = sw.ElapsedMilliseconds;

        // cacheValue = allIcons;

        // var cacheEntryOptions = new MemoryCacheEntryOptions() .SetPriority(CacheItemPriority.NeverRemove);

        //    _memoryCache.Set(CacheKeys.ListIconFiles, cacheValue, cacheEntryOptions);
        //}

        //// var test = cacheValue.Where(a => a.Name.StartsWith("sexy"));

        return await _context.GetIcons(cancellationToken);
    }
}
