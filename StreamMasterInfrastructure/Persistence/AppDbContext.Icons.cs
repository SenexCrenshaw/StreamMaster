using AutoMapper.QueryableExtensions;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

using StreamMasterApplication.Icons;

using StreamMasterDomain.Common;
using StreamMasterDomain.Dto;
using StreamMasterDomain.Entities;
using StreamMasterDomain.Enums;

namespace StreamMasterInfrastructure.Persistence;

public partial class AppDbContext : IIconDB
{
    public DbSet<IconFile> Icons { get; set; }

    public async Task<List<IconFileDto>> GetIcons(CancellationToken cancellationToken)
    {
        if (!_memoryCache.TryGetValue(CacheKeys.ListIconFiles, out List<IconFileDto>? cacheValue))
        {
            var setting = FileUtil.GetSetting();

            List<IconFileDto> icons = await Icons.Where(a => a.FileExists)
              .AsNoTracking()
              .Where(a => a.FileExists && a.SMFileType == SMFileTypes.Icon)
              .ProjectTo<IconFileDto>(_mapper.ConfigurationProvider)
              .OrderBy(x => x.Name)
              .ToListAsync(cancellationToken).ConfigureAwait(false);

            IEnumerable<IconFileDto> tvlogos = _mapper.Map<IEnumerable<IconFileDto>>(_memoryCache.TvLogos());

            cacheValue = tvlogos.Concat(icons).ToList();

            var cacheEntryOptions = new MemoryCacheEntryOptions()
                .SetPriority(CacheItemPriority.NeverRemove);

            _memoryCache.Set(CacheKeys.ListIconFiles, cacheValue, cacheEntryOptions);
        }

        return cacheValue ?? new List<IconFileDto>();
    }
}
