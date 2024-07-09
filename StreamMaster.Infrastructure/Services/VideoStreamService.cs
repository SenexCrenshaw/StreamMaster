using AutoMapper;

using MediatR;

using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;

using StreamMaster.Domain.Dto;
using StreamMaster.Domain.Repository;

namespace StreamMaster.Infrastructure.Services;

public class VideoStreamService(IServiceProvider serviceProvider, IMapper mapper, IMemoryCache cache) : IVideoStreamService
{
    public void RemoveVideoStreamDto(int smChannelId)
    {
        string cacheKey = $"SMChannel-{smChannelId}";
        cache.Remove(cacheKey);
    }

    public async Task<SMStreamDto?> GetSMStreamDtoAsync(string smStreamId, CancellationToken cancellationToken)
    {
        // Cache key to uniquely identify the VideoStreamDto
        string cacheKey = $"SMStreamDto-{smStreamId}";

        // Try to get the VideoStreamDto from the cache
        if (!cache.TryGetValue(cacheKey, out SMStreamDto? smStreamDto))
        {
            using IServiceScope scope = serviceProvider.CreateScope();
            IMediator mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
            IRepositoryWrapper repositoryWrapper = scope.ServiceProvider.GetRequiredService<IRepositoryWrapper>();
            SMStream? smStream = await repositoryWrapper.SMStream.FirstOrDefaultAsync(a => a.Id == smStreamId);

            if (smStream != null)
            {
                smStreamDto = mapper.Map<SMStreamDto>(smStream);

                // Define cache options
                MemoryCacheEntryOptions cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromMinutes(5))
                    .SetAbsoluteExpiration(TimeSpan.FromHours(1));

                // Cache the VideoStreamDto
                cache.Set(cacheKey, smStreamDto, cacheEntryOptions);
            }
        }

        return smStreamDto;
    }
}
