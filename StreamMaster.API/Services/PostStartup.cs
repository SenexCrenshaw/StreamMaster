using AutoMapper;

using Microsoft.Extensions.Caching.Memory;

using StreamMaster.Application.Services;
using StreamMaster.Domain.Common;
using StreamMaster.Infrastructure.EF;
using StreamMaster.SchedulesDirect.Domain.Interfaces;

namespace StreamMasterAPI.Services;

public class PostStartup : BackgroundService
{
    private readonly ILogger _logger;

    private readonly IBackgroundTaskQueue _taskQueue;
    private readonly IMemoryCache _memoryCache;
    private readonly IMapper _mapper;
    private readonly IServiceProvider serviceProvider;
    public PostStartup(
        ILogger<PostStartup> logger,
        IServiceProvider serviceProvider,
        IMapper mapper,
        IMemoryCache memoryCache,
        IBackgroundTaskQueue taskQueue
        )
    {
        (_logger, _taskQueue, _memoryCache, _mapper, this.serviceProvider) = (logger, taskQueue, memoryCache, mapper, serviceProvider);
    }

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        if (cancellationToken.IsCancellationRequested)
        {
            return;
        }

        _logger.LogInformation($"Stream Master is startting.");

        await _taskQueue.EPGSync(cancellationToken).ConfigureAwait(false);


        if (await IconHelper.ReadDirectoryLogos(_memoryCache, cancellationToken).ConfigureAwait(false))
        {
            //List<IconFileDto> cacheValue = _mapper.Map<List<IconFileDto>>(_memoryCache.TvLogos());
            //_memoryCache.SetCache(cacheValue);
        }

        await _taskQueue.ScanDirectoryForEPGFiles(cancellationToken).ConfigureAwait(false);

        await _taskQueue.ScanDirectoryForM3UFiles(cancellationToken).ConfigureAwait(false);

        await _taskQueue.UpdateChannelGroupCounts(cancellationToken).ConfigureAwait(false);

        await _taskQueue.BuildIconCaches(cancellationToken).ConfigureAwait(false);


        while (_taskQueue.HasJobs())
        {
            await Task.Delay(250, cancellationToken).ConfigureAwait(false);
        }


        await _taskQueue.SetIsSystemReady(true, cancellationToken).ConfigureAwait(false);

        using IServiceScope scope = serviceProvider.CreateScope();
        RepositoryContext repositoryContext = scope.ServiceProvider.GetRequiredService<RepositoryContext>();
        ISchedulesDirectDataService schedulesDirectService = scope.ServiceProvider.GetRequiredService<ISchedulesDirectDataService>();
        await repositoryContext.MigrateData(schedulesDirectService.AllServices);
    }
}
