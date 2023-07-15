using AutoMapper.QueryableExtensions;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

using StreamMasterApplication.General;
using StreamMasterApplication.Icons;

using StreamMasterDomain.Common;
using StreamMasterDomain.Dto;
using StreamMasterDomain.Entities;
using StreamMasterDomain.Enums;

using System.Diagnostics;

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

            List<IconFileDto> allIcons = tvlogos.Concat(icons).ToList();

            int count = 0;

            Stopwatch sw = Stopwatch.StartNew();
            ParallelOptions po = new()
            {
                CancellationToken = cancellationToken,
                MaxDegreeOfParallelism = Environment.ProcessorCount
            };

            _ = Parallel.ForEach(allIcons, po, icon =>
            {
                string IconSource = Helpers.GetIPTVChannelIconSources(icon.Source, setting, "/", allIcons);
                icon.Id = count++;
                icon.Source = IconSource;
                icon.Url = IconSource;
            });

            sw.Stop();
            long el = sw.ElapsedMilliseconds;

            cacheValue = allIcons;

            var cacheEntryOptions = new MemoryCacheEntryOptions()
                .SetPriority(CacheItemPriority.NeverRemove);

            _memoryCache.Set(CacheKeys.ListIconFiles, cacheValue, cacheEntryOptions);
        }

        // var test = cacheValue.Where(a => a.Name.StartsWith("sexy"));

        return cacheValue ?? new List<IconFileDto>();
    }
}
