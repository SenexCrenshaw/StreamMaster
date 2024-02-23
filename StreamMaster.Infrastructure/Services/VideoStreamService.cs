using MediatR;

using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;

using StreamMaster.Application.VideoStreams.Queries;
using StreamMaster.Domain.Dto;
using StreamMaster.Domain.Services;

namespace StreamMaster.Infrastructure.Services;

public class VideoStreamService(IServiceProvider serviceProvider, IMemoryCache cache, ISender sender) : IVideoStreamService
{
    public void RemoveVideoStreamDto(string videoStreamId)
    {
        string cacheKey = $"VideoStreamDto-{videoStreamId}";
        cache.Remove(cacheKey);
    }

    public async Task<VideoStreamDto?> GetVideoStreamDtoAsync(string videoStreamId, CancellationToken cancellationToken)
    {
        // Cache key to uniquely identify the VideoStreamDto
        string cacheKey = $"VideoStreamDto-{videoStreamId}";

        // Try to get the VideoStreamDto from the cache
        if (!cache.TryGetValue(cacheKey, out VideoStreamDto? videoStreamDto))
        {
            using IServiceScope scope = serviceProvider.CreateScope();
            IMediator mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
            // If not in cache, fetch from the database
            videoStreamDto = await mediator.Send(new GetVideoStream(videoStreamId), cancellationToken);

            if (videoStreamDto != null)
            {
                // Define cache options
                MemoryCacheEntryOptions cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromMinutes(5))
                    .SetAbsoluteExpiration(TimeSpan.FromHours(1));

                // Cache the VideoStreamDto
                cache.Set(cacheKey, videoStreamDto, cacheEntryOptions);
            }
        }

        return videoStreamDto;
    }
}

