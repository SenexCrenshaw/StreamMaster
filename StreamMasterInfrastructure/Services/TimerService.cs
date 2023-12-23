using MediatR;

using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using StreamMaster.SchedulesDirectAPI.Domain.Interfaces;

using StreamMasterApplication.EPGFiles.Commands;
using StreamMasterApplication.EPGFiles.Queries;
using StreamMasterApplication.M3UFiles.Commands;
using StreamMasterApplication.M3UFiles.Queries;
using StreamMasterApplication.SchedulesDirectAPI.Commands;
using StreamMasterApplication.Settings.Queries;

using StreamMasterDomain.Cache;
using StreamMasterDomain.Dto;
using StreamMasterDomain.Repository;

namespace StreamMasterInfrastructure.Services;

public class TimerService(IServiceProvider serviceProvider, IMemoryCache memoryCache, ILogger<TimerService> logger) : IHostedService, IDisposable
{
    private readonly object Lock = new();

    private Timer? _timer;
    private bool isActive = false;

    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        //_logger.LogInformation("Timer Service running.");

        _timer = new Timer(async state => await DoWorkAsync(state, cancellationToken), null, TimeSpan.Zero, TimeSpan.FromSeconds(5));

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        //_logger.LogInformation("Timer Service is stopping.");

        _ = (_timer?.Change(Timeout.Infinite, 0));

        return Task.CompletedTask;
    }

    private async Task DoWorkAsync(object? state, CancellationToken cancellationToken)
    {
        lock (Lock)
        {
            if (isActive)
            {
                return;
            }
            isActive = true;
        }

        SDSystemStatus status = new() { IsSystemReady = memoryCache.IsSystemReady() };

        if (!status.IsSystemReady)
        {
            lock (Lock)
            {
                isActive = false;
            }
            return;
        }

        using IServiceScope scope = serviceProvider.CreateScope();
        IMediator mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

        IRepositoryWrapper repository = scope.ServiceProvider.GetRequiredService<IRepositoryWrapper>();
        ISchedulesDirect schedulesDirect = scope.ServiceProvider.GetRequiredService<ISchedulesDirect>();

        StreamMasterDomain.Common.Setting setting = memoryCache.GetSetting();
        DateTime now = DateTime.Now;

        //if (setting.SDSettings.SDEnabled)
        //{
        JobStatus jobStatus = memoryCache.GetSyncJobStatus();
        if (!jobStatus.IsRunning)
        {
            if (jobStatus.ForceNextRun || (now - jobStatus.LastRun).TotalMinutes > 15)
            {
                if (jobStatus.ForceNextRun || jobStatus.IsErrored || (now - jobStatus.LastSuccessful).TotalMinutes > 60)
                {
                    logger.LogInformation("EPGSync started. {status}", memoryCache.GetSyncJobStatus());
                    await mediator.Send(new EPGSync(), cancellationToken).ConfigureAwait(false);
                    if (jobStatus.Extra)
                    {
                        foreach (EPGFileDto epg in await repository.EPGFile.GetEPGFiles())
                        {
                            await mediator.Send(new RefreshEPGFileRequest(epg.Id), cancellationToken).ConfigureAwait(false);
                        }
                        jobStatus.Extra = false;
                    }
                    logger.LogInformation("EPGSync completed. {status}", memoryCache.GetSyncJobStatus());
                }
            }
        }
        //}

        IEnumerable<EPGFileDto> epgFilesToUpdated = await mediator.Send(new GetEPGFilesNeedUpdating(), cancellationToken).ConfigureAwait(false);
        IEnumerable<M3UFileDto> m3uFilesToUpdated = await mediator.Send(new GetM3UFilesNeedUpdating(), cancellationToken).ConfigureAwait(false);

        if (epgFilesToUpdated.Any())
        {
            logger.LogInformation("EPG Files to update count: {epgFiles.Count()}", epgFilesToUpdated.Count());
            //foreach (EPGFileDto? epgFile in epgFilesToUpdated)
            //{
            //    _ = await mediator.Send(new RefreshEPGFileRequest(epgFile.Id), cancellationToken).ConfigureAwait(false);
            //}
            schedulesDirect.ResetEPGCache();
            memoryCache.SetSyncForceNextRun(Extra: true);
        }

        if (m3uFilesToUpdated.Any())
        {
            logger.LogInformation("M3U Files to update count: {m3uFiles.Count()}", m3uFilesToUpdated.Count());

            foreach (M3UFileDto? m3uFile in m3uFilesToUpdated)
            {
                _ = await mediator.Send(new RefreshM3UFileRequest(m3uFile.Id), cancellationToken).ConfigureAwait(false);
            }
        }

        lock (Lock)
        {
            isActive = false;
        }
    }
}