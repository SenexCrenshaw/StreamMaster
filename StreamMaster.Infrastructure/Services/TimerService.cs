using MediatR;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using StreamMaster.Application.EPGFiles.Commands;
using StreamMaster.Application.Interfaces;
using StreamMaster.Application.M3UFiles.Commands;
using StreamMaster.Application.Services;
using StreamMaster.Domain.API;
using StreamMaster.Domain.Common;
using StreamMaster.Domain.Configuration;
using StreamMaster.Domain.Dto;
using StreamMaster.Domain.Enums;
using StreamMaster.Domain.Helpers;

namespace StreamMaster.Infrastructure.Services
{
    public class TimerService : BackgroundService
    {
        private readonly IServiceProvider serviceProvider;
        private readonly IJobStatusService jobStatusService;
        private readonly ILogger<TimerService> logger;
        private readonly IOptionsMonitor<Setting> intSettings;
        private readonly IOptionsMonitor<SDSettings> intsdsettings;

        // Static fields to maintain application-wide state
        private static DateTime LastBackupTime = DateTime.UtcNow;

        public TimerService(
            IServiceProvider serviceProvider,
            IOptionsMonitor<Setting> intSettings,
            IOptionsMonitor<SDSettings> intsdsettings,
            IJobStatusService jobStatusService,
            ILogger<TimerService> logger)
        {
            this.serviceProvider = serviceProvider;
            this.intSettings = intSettings;
            this.intsdsettings = intsdsettings;
            this.jobStatusService = jobStatusService;
            this.logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using PeriodicTimer timer = new(TimeSpan.FromMinutes(1));
            while (await timer.WaitForNextTickAsync(stoppingToken))
            {
                await DoWorkAsync(stoppingToken);
            }
        }

        private async Task DoWorkAsync(CancellationToken cancellationToken)
        {
            using IServiceScope scope = serviceProvider.CreateScope();
            IMediator mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
            IBackgroundTaskQueue backgroundTask = scope.ServiceProvider.GetRequiredService<IBackgroundTaskQueue>();
            IEPGFileService epgFileService = scope.ServiceProvider.GetRequiredService<IEPGFileService>();
            IM3UFileService m3uFileService = scope.ServiceProvider.GetRequiredService<IM3UFileService>();

            DateTime now = DateTime.UtcNow;

            // Manage SDSync Job
            await ExecuteJobAsync(
                jobStatusService.GetJobManager(JobType.SDSync, EPGHelper.SchedulesDirectId),
                async () =>
                {
                    if (intsdsettings.CurrentValue.SDEnabled)
                    {
                        logger.LogInformation("SDSync started.");

                        await backgroundTask.EPGSync(cancellationToken).ConfigureAwait(false);

                        logger.LogInformation("SDSync completed.");
                    }
                },
                runInterval: TimeSpan.FromMinutes(15),
                successInterval: TimeSpan.FromMinutes(60),
                cancellationToken: cancellationToken);

            // Manage TimerEPG Job
            await ExecuteJobAsync(
                jobStatusService.GetJobManager(JobType.TimerEPG, 0),
                async () =>
                {
                    DataResponse<List<EPGFileDto>> epgFilesToUpdated = await epgFileService.GetEPGFilesNeedUpdatingAsync().ConfigureAwait(false);
                    if (epgFilesToUpdated.Data.Any())
                    {
                        logger.LogInformation("EPG Files to update count: {count}", epgFilesToUpdated.Data.Count);

                        foreach (EPGFileDto epg in epgFilesToUpdated.Data)
                        {
                            await mediator.Send(new RefreshEPGFileRequest(epg.Id), cancellationToken).ConfigureAwait(false);
                        }
                    }
                },
                cancellationToken: cancellationToken);

            // Manage TimerM3U Job
            await ExecuteJobAsync(
                jobStatusService.GetJobManager(JobType.TimerM3U, 0),
                async () =>
                {
                    DataResponse<List<M3UFileDto>> m3uFilesToUpdated = await m3uFileService.GetM3UFilesNeedUpdatingAsync().ConfigureAwait(false);
                    if (m3uFilesToUpdated.Data.Any())
                    {
                        logger.LogInformation("M3U Files to update count: {count}", m3uFilesToUpdated.Data.Count);

                        foreach (M3UFileDto m3uFile in m3uFilesToUpdated.Data)
                        {
                            await mediator.Send(new RefreshM3UFileRequest(m3uFile.Id), cancellationToken).ConfigureAwait(false);
                        }
                    }
                },
                cancellationToken: cancellationToken);

            // Manage Backup Job
            await ExecuteJobAsync(
                jobStatusService.GetJobManager(JobType.TimerBackup, 0),
                async () =>
                {
                    logger.LogInformation("Backup started.");

                    await FileUtil.Backup().ConfigureAwait(false);

                    logger.LogInformation("Backup completed.");
                    LastBackupTime = DateTime.UtcNow;
                },
                executeIf: () => intSettings.CurrentValue.BackupEnabled && LastBackupTime.AddHours(intSettings.CurrentValue.BackupInterval) <= now,
                cancellationToken: cancellationToken);
        }

        private async Task ExecuteJobAsync(
            JobStatusManager jobManager,
            Func<Task> jobFunc,
            TimeSpan? runInterval = null,
            TimeSpan? successInterval = null,
            Func<bool>? executeIf = null,
            Action? onSuccessful = null,
            CancellationToken cancellationToken = default)
        {
            if (jobManager.IsRunning)
            {
                return;
            }

            if (executeIf != null && !executeIf())
            {
                return;
            }

            if (runInterval.HasValue && (DateTime.UtcNow - jobManager.LastRun) < runInterval.Value)
            {
                return;
            }

            if (successInterval.HasValue && (DateTime.UtcNow - jobManager.LastSuccessful) < successInterval.Value)
            {
                return;
            }

            try
            {
                jobManager.Start();
                await jobFunc();
                jobManager.SetSuccessful();
                onSuccessful?.Invoke();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred during the job {JobType}", jobManager.JobType);
                jobManager.SetError();
            }
        }
    }
}