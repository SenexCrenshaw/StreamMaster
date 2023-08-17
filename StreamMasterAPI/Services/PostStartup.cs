using AutoMapper;

using MediatR;

using Microsoft.Extensions.Caching.Memory;

using StreamMasterApplication.Services;

using StreamMasterDomain.Cache;
using StreamMasterDomain.Common;
using StreamMasterDomain.Dto;

namespace StreamMasterAPI.Services;

public class PostStartup : BackgroundService
{
    private readonly ILogger _logger;
    private readonly ISender _sender;
    private readonly IBackgroundTaskQueue _taskQueue;
    private readonly IMemoryCache _memoryCache;
    private readonly IMapper _mapper;
    public PostStartup(
        ILogger<PostStartup> logger,
        ISender sender,
        IMapper mapper,
        IMemoryCache memoryCache,
        IBackgroundTaskQueue taskQueue
        )
    {
        (_logger, _taskQueue, _sender, _memoryCache, _mapper) = (logger, taskQueue, sender, memoryCache, mapper);
    }

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        if (cancellationToken.IsCancellationRequested)
        {
            return;
        }

        _logger.LogInformation(
        $"{nameof(PostStartup)} is running.");

        //await _hubContext.Clients.All.SystemStatusUpdate(new StreamMasterApplication.Settings.Queries.SystemStatus { IsSystemReady = false }).ConfigureAwait(false);

        // await _taskQueue.ScanDirectoryForIconFiles(cancellationToken).ConfigureAwait(false);

        //_sender.Send(new StreamMasterApplication.Settings.Queries.SystemStatus { IsSystemReady = false }, cancellationToken);

        //await _taskQueue.ReadDirectoryLogosRequest(cancellationToken).ConfigureAwait(false);

        if (await IconHelper.ReadDirectoryLogos(_memoryCache, cancellationToken).ConfigureAwait(false))
        {
            List<IconFileDto> cacheValue = _mapper.Map<List<IconFileDto>>(_memoryCache.TvLogos());
            _memoryCache.Set(cacheValue);
        }

        await _taskQueue.ScanDirectoryForEPGFiles(cancellationToken).ConfigureAwait(false);

        await _taskQueue.ScanDirectoryForM3UFiles(cancellationToken).ConfigureAwait(false);

        //await _taskQueue.ProcessM3UFiles(cancellationToken).ConfigureAwait(false);

        //await _taskQueue.UpdateChannelGroupCounts(cancellationToken).ConfigureAwait(false);

        await _taskQueue.BuildIconCaches(cancellationToken).ConfigureAwait(false);

        while (!_taskQueue.IsCurrent())
        {
            await Task.Delay(250, cancellationToken).ConfigureAwait(false);
        }

        await _taskQueue.SetIsSystemReady(true, cancellationToken).ConfigureAwait(false);

        //await _taskQueue.CacheAllIcons(cancellationToken).ConfigureAwait(false);
    }
}
