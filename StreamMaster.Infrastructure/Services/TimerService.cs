using MediatR;

using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using StreamMaster.Application.Common.Interfaces;
using StreamMaster.Application.EPGFiles.Commands;
using StreamMaster.Application.EPGFiles.Queries;
using StreamMaster.Application.Hubs;
using StreamMaster.Application.M3UFiles.CommandsOrig;
using StreamMaster.Application.M3UFiles.Queries;
using StreamMaster.Application.SchedulesDirect.Commands;
using StreamMaster.Application.Services;
using StreamMaster.Application.Settings.Queries;
using StreamMaster.Domain.Common;
using StreamMaster.Domain.Configuration;
using StreamMaster.Domain.Dto;
using StreamMaster.Domain.Enums;
using StreamMaster.Domain.Helpers;
using StreamMaster.Domain.Repository;
using StreamMaster.Domain.Services;
using StreamMaster.SchedulesDirect.Domain.Interfaces;

namespace StreamMaster.Infrastructure.Services;

public class TimerService(IServiceProvider serviceProvider, IOptionsMonitor<Setting> intsettings, IOptionsMonitor<SDSettings> intsdsettings, IJobStatusService jobStatusService, ILogger<TimerService> logger) : IHostedService, IDisposable
{
    private readonly object Lock = new();
    private readonly Setting settings = intsettings.CurrentValue;
    private readonly SDSettings sdsettings = intsdsettings.CurrentValue;
    private Timer? _timer;
    private bool isActive = false;
    private static DateTime LastBackupTime = DateTime.UtcNow;
    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {

        _timer = new Timer(async state => await DoWorkAsync(state, cancellationToken), null, TimeSpan.Zero, TimeSpan.FromSeconds(5));

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
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



        DateTime now = DateTime.Now;

        JobStatusManager jobManager = jobStatusService.GetJobManager(JobType.SDSync, EPGHelper.SchedulesDirectId);
        if (!jobManager.IsRunning)
        {
            try
            {
                jobManager.Start();

                if (jobManager.ForceNextRun || (now - jobManager.LastRun).TotalMinutes > 15 || (now - jobManager.LastSuccessful).TotalMinutes > 60)
                {
                    if (jobManager.ForceNextRun)
                    {
                        jobManager.ClearForce();
                    }

                    if (sdsettings.SDEnabled)
                    {
                        logger.LogInformation("SDSync started. {status}", jobManager.Status);

                        _ = await mediator.Send(new EPGSync(), cancellationToken).ConfigureAwait(false);
                        await hubContext.Clients.All.EPGFilesRefresh().ConfigureAwait(false);

                        logger.LogInformation("SDSync completed. {status}", jobManager.Status);
                    }
                }
                jobManager.SetSuccessful();
            }
            catch
            {
                jobManager.SetError();
            }
        }

        jobManager = jobStatusService.GetJobManager(JobType.TimerEPG, 0);
        if (!jobManager.IsRunning)
        {
            try
            {
                jobManager.Start();
                IEnumerable<EPGFileDto> epgFilesToUpdated = await mediator.Send(new GetEPGFilesNeedUpdating(), cancellationToken).ConfigureAwait(false);
                if (epgFilesToUpdated.Any())
                {
                    logger.LogInformation("EPG Files to update count: {epgFiles.Count()}", epgFilesToUpdated.Count());

                    foreach (EPGFileDto epg in epgFilesToUpdated)
                    {
                        _ = await mediator.Send(new RefreshEPGFileRequest(epg.Id), cancellationToken).ConfigureAwait(false);
                    }
                }
                jobManager.SetSuccessful();
            }
            catch
            {
                jobManager.SetError();
            }
        }

        jobManager = jobStatusService.GetJobManager(JobType.TimerM3U, 0);
        if (!jobManager.IsRunning)
        {
            try
            {
                jobManager.Start();
                IEnumerable<M3UFileDto> m3uFilesToUpdated = await mediator.Send(new GetM3UFilesNeedUpdating(), cancellationToken).ConfigureAwait(false);
                if (m3uFilesToUpdated.Any())
                {
                    logger.LogInformation("M3U Files to update count: {m3uFiles.Count()}", m3uFilesToUpdated.Count());

                    foreach (M3UFileDto? m3uFile in m3uFilesToUpdated)
                    {
                        await mediator.Send(new RefreshM3UFileRequest(m3uFile.Id), cancellationToken).ConfigureAwait(false);
                    }
                }
                jobManager.SetSuccessful();
            }
            catch
            {
                jobManager.SetError();
            }
        }

        jobManager = jobStatusService.GetJobManager(JobType.TimerBackup, 0);
        if (settings.BackupEnabled && !jobManager.IsRunning && LastBackupTime.AddHours(settings.BackupInterval) <= DateTime.UtcNow)
        {
            try
            {
                jobManager.Start();

                logger.LogInformation("Backup started. {status}", jobManager.Status);

                await FileUtil.Backup().ConfigureAwait(false);

                logger.LogInformation("Backup completed. {status}", jobManager.Status);
                LastBackupTime = DateTime.UtcNow;

                jobManager.SetSuccessful();
            }
            catch
            {
                jobManager.SetError();
            }
        }

        lock (Lock)
        {
            isActive = false;
        }
    }
}