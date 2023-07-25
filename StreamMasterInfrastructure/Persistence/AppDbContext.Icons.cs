using Microsoft.Extensions.Caching.Memory;

using StreamMasterApplication.Icons;

using StreamMasterDomain.Dto;
using StreamMasterDomain.Enums;

namespace StreamMasterInfrastructure.Persistence;

public partial class AppDbContext : IIconDB
{
    //public DbSet<IconFile> Icons { get; set; }

    public async Task<List<IconFileDto>> GetIcons(CancellationToken cancellationToken)
    {
        if (!_memoryCache.TryGetValue(CacheKeys.ListIconFiles, out List<IconFileDto>? cacheValue))
        {
            _memoryCache.Icons().ToList();
            //IEnumerable<IconFileDto> tvlogos = _mapper.Map<IEnumerable<IconFileDto>>(_memoryCache.TvLogos());
            //cacheValue = tvlogos.Concat(icons).ToList();
            //return new List<IconFileDto>();
            //var setting = FileUtil.GetSetting();

            //List<IconFileDto> icons = await Icons.Where(a => a.FileExists)
            //  .AsNoTracking()
            //  .Where(a => a.FileExists && a.SMFileType == SMFileTypes.Icon)
            //  .ProjectTo<IconFileDto>(_mapper.ConfigurationProvider)
            //  .OrderBy(x => x.Name)
            //  .ToListAsync(cancellationToken).ConfigureAwait(false);

            cacheValue = _mapper.Map<List<IconFileDto>>(_memoryCache.TvLogos());

            //cacheValue = tvlogos.Concat(icons).ToList();

            var cacheEntryOptions = new MemoryCacheEntryOptions()
                .SetPriority(CacheItemPriority.NeverRemove);

            _memoryCache.Set(CacheKeys.ListIconFiles, cacheValue, cacheEntryOptions);
        }

        return cacheValue ?? new List<IconFileDto>();
    }
}
