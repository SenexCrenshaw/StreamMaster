using MediatR;

using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using StreamMaster.Application.Common.Interfaces;
using StreamMaster.Application.EPGFiles.Commands;
using StreamMaster.Application.EPGFiles.Queries;
using StreamMaster.Application.Hubs;
using StreamMaster.Application.M3UFiles.Commands;
using StreamMaster.Application.M3UFiles.Queries;
using StreamMaster.Application.SchedulesDirect.Commands;
using StreamMaster.Application.Services;
using StreamMaster.Application.Settings.Queries;
using StreamMaster.Domain.Cache;
using StreamMaster.Domain.Common;
using StreamMaster.Domain.Dto;
using StreamMaster.Domain.Repository;
using StreamMaster.Domain.Services;
using StreamMaster.SchedulesDirect.Domain.Interfaces;

namespace StreamMaster.Infrastructure.Services;

public class TimerService(IServiceProvider serviceProvider, IMemoryCache memoryCache, IJobStatusService jobStatusService, ILogger<TimerService> logger) : IHostedService, IDisposable
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

        _timer = new Timer(async state => await DoWorkAsync(state, cancellationToken), null, TimeSpan.Zero, TimeSpan.FromSeconds(1));

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        //_logger.LogInformation("Timer Service is stopping.");

        _ = (_timer?.Change(Timeout.Infinite, 0));

        return Task.CompletedTask;
    }

    private async Task DoWorkAsync(object? _, CancellationToken cancellationToken)
    {
        if (isActive)
        {
            return;
        }

        lock (Lock)
        {
            if (isActive)
            {
                return;
            }
            isActive = true;
        }



        using IServiceScope scope = serviceProvider.CreateScope();
        IMediator mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        IHubContext<StreamMasterHub, IStreamMasterHub> hubContext = scope.ServiceProvider.GetRequiredService<IHubContext<StreamMasterHub, IStreamMasterHub>>();
        IBackgroundTaskQueue backgroundTask = scope.ServiceProvider.GetRequiredService<IBackgroundTaskQueue>();

        await hubContext.Clients.All.TaskQueueStatusUpdate(await backgroundTask.GetQueueStatus()).ConfigureAwait(false);

        SDSystemStatus status = new() { IsSystemReady = BuildInfo.SetIsSystemReady };

        lock (Lock)
        {
            if (!status.IsSystemReady)
            {
                isActive = false;
                return;
            }
            isActive = true;
        }

        IRepositoryWrapper repository = scope.ServiceProvider.GetRequiredService<IRepositoryWrapper>();
        ISchedulesDirect schedulesDirect = scope.ServiceProvider.GetRequiredService<ISchedulesDirect>();


        Setting setting = memoryCache.GetSetting();
        DateTime now = DateTime.Now;

        JobStatus jobStatus = jobStatusService.GetSyncJobStatus();
        if (!jobStatus.IsRunning)
        {
            jobStatus.SetIsRunning(true);

            if (jobStatus.ForceNextRun || (now - jobStatus.LastRun).TotalMinutes > 15 || (now - jobStatus.LastSuccessful).TotalMinutes > 60)
            {
                if (jobStatus.ForceNextRun)
                {
                    jobStatusService.ClearSyncForce();
                }

                if (setting.SDSettings.SDEnabled)
                {
                    logger.LogInformation("EPGSync started. {status}", jobStatusService.GetSyncJobStatus());

                    _ = await mediator.Send(new EPGSync(), cancellationToken).ConfigureAwait(false);
                    await hubContext.Clients.All.EPGFilesRefresh().ConfigureAwait(false);

                    logger.LogInformation("EPGSync completed. {status}", jobStatusService.GetSyncJobStatus());
                }
            }
        }

        if (!jobStatusService.GetEPGJobStatus().IsRunning)
        {
            IEnumerable<EPGFileDto> epgFilesToUpdated = await mediator.Send(new GetEPGFilesNeedUpdating(), cancellationToken).ConfigureAwait(false);
            if (epgFilesToUpdated.Any())
            {
                logger.LogInformation("EPG Files to update count: {epgFiles.Count()}", epgFilesToUpdated.Count());

                foreach (EPGFileDto epg in epgFilesToUpdated)
                {
                    _ = await mediator.Send(new RefreshEPGFileRequest(epg.Id), cancellationToken).ConfigureAwait(false);
                }
            }
        }

        if (!jobStatusService.GetM3UJobStatus().IsRunning)
        {
            IEnumerable<M3UFileDto> m3uFilesToUpdated = await mediator.Send(new GetM3UFilesNeedUpdating(), cancellationToken).ConfigureAwait(false);
            if (m3uFilesToUpdated.Any())
            {
                logger.LogInformation("M3U Files to update count: {m3uFiles.Count()}", m3uFilesToUpdated.Count());

                foreach (M3UFileDto? m3uFile in m3uFilesToUpdated)
                {
                    await mediator.Send(new RefreshM3UFileRequest(m3uFile.Id), cancellationToken).ConfigureAwait(false);
                }
            }
        }

        lock (Lock)
        {
            isActive = false;
        }
    }
}