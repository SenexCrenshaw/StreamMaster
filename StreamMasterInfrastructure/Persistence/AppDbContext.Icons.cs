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
            cacheValue = _mapper.Map<List<IconFileDto>>(_memoryCache.TvLogos());


            var cacheEntryOptions = new MemoryCacheEntryOptions()
                .SetPriority(CacheItemPriority.NeverRemove);

            _memoryCache.Set(CacheKeys.ListIconFiles, cacheValue, cacheEntryOptions);
        }

        return cacheValue ?? new List<IconFileDto>();
    }
}
