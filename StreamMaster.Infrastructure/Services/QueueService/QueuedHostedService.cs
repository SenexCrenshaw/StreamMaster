using System.Collections.Concurrent;

using MediatR;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using StreamMaster.Application.Common.Models;
using StreamMaster.Application.Custom.Commands;
using StreamMaster.Application.EPGFiles.Commands;
using StreamMaster.Application.General.Commands;
using StreamMaster.Application.Logos.Commands;
using StreamMaster.Application.M3UFiles.Commands;
using StreamMaster.Application.SchedulesDirect.Commands;
using StreamMaster.Application.Services;
using StreamMaster.Application.StreamGroups.Commands;
using StreamMaster.Domain.Enums;

namespace StreamMaster.Infrastructure.Services.QueueService;

public sealed class QueuedHostedService(
    IBackgroundTaskQueue taskQueue,
    IServiceProvider serviceProvider,
    IMessageService messageService,
    ILogoService logoService,
    ILogger<QueuedHostedService> logger
) : BackgroundService
{
    private readonly ConcurrentDictionary<Guid, CancellationTokenSource> _taskCancellationTokens = new();
    private readonly SemaphoreSlim _semaphore = new(5); // Limit concurrency to 5 tasks

    public override async Task StopAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation(
            "{nameof(QueuedHostedService)} is stopping.", nameof(QueuedHostedService));

        foreach (CancellationTokenSource cts in _taskCancellationTokens.Values)
        {
            cts.Cancel();
        }

        await base.StopAsync(stoppingToken).ConfigureAwait(false);
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        return ProcessTaskQueueAsync(stoppingToken);
    }

    private async Task ProcessTaskQueueAsync(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            await _semaphore.WaitAsync(cancellationToken).ConfigureAwait(false);

            BackgroundTaskQueueConfig command;
            try
            {
                command = await taskQueue.DeQueueAsync(cancellationToken).ConfigureAwait(false);
            }
            catch (OperationCanceledException)
            {
                _semaphore.Release();
                break;
            }

            using CancellationTokenSource cancellationSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            _taskCancellationTokens.TryAdd(command.Id, cancellationSource);
            cancellationSource.CancelAfter(TimeSpan.FromMinutes(10)); // Set a timeout for each task

            try
            {
                logger.LogInformation("Starting {command}", command.Command);
                await messageService.SendInfo($"Starting task: {command.Command}").ConfigureAwait(false);

                using IServiceScope scope = serviceProvider.CreateScope();

                ISender _sender = scope.ServiceProvider.GetRequiredService<ISender>();

                await taskQueue.SetStart(command.Id).ConfigureAwait(false);

                switch (command.Command)
                {
                    case SMQueCommand.EPGRemovedExpiredKeys:
                        await _sender.Send(new EPGRemovedExpiredKeysRequest(), cancellationSource.Token).ConfigureAwait(false);
                        break;

                    case SMQueCommand.CacheChannelLogos:
                        await _sender.Send(new CacheSMChannelLogosRequest(), cancellationSource.Token).ConfigureAwait(false);
                        break;

                    case SMQueCommand.CacheStreamLogos:
                        await _sender.Send(new CacheSMStreamLogosRequest(), cancellationSource.Token).ConfigureAwait(false);
                        break;

                    case SMQueCommand.ScanForTvLogos:
                        await logoService.ScanForTvLogosAsync(cancellationSource.Token).ConfigureAwait(false);
                        break;

                    case SMQueCommand.CreateSTRMFiles:
                        await _sender.Send(new CreateSTRMFilesRequest(), cancellationSource.Token).ConfigureAwait(false);
                        break;

                    case SMQueCommand.ProcessEPGFile:
                        if (command.Entity is int entityId)
                        {
                            await _sender.Send(new ProcessEPGFileRequest(entityId), cancellationSource.Token).ConfigureAwait(false);
                        }
                        break;

                    case SMQueCommand.EPGSync:
                        await _sender.Send(new EPGSync(), cancellationSource.Token).ConfigureAwait(false);
                        break;

                    case SMQueCommand.ProcessM3UFile:
                        if (command.Entity is ProcessM3UFileRequest p)
                        {
                            await _sender.Send(p, cancellationSource.Token).ConfigureAwait(false);
                        }
                        break;

                    case SMQueCommand.ScanForCustomPlayLists:
                        await _sender.Send(new ScanForCustomRequest(), cancellationSource.Token).ConfigureAwait(false);
                        break;

                    case SMQueCommand.ProcessM3UFiles:
                        await _sender.Send(new ProcessM3UFilesRequest(), cancellationSource.Token).ConfigureAwait(false);
                        break;

                    case SMQueCommand.ScanDirectoryForM3UFiles:
                        await _sender.Send(new ScanDirectoryForM3UFilesRequest(), cancellationSource.Token).ConfigureAwait(false);
                        break;

                    case SMQueCommand.ScanDirectoryForEPGFiles:
                        await _sender.Send(new ScanDirectoryForEPGFilesRequest(), cancellationSource.Token).ConfigureAwait(false);
                        break;

                    case SMQueCommand.SetIsSystemReady:
                        if (command.Entity is bool isSystemReady)
                        {
                            await _sender.Send(new SetIsSystemReadyRequest(isSystemReady), cancellationSource.Token).ConfigureAwait(false);
                        }
                        break;

                    case SMQueCommand.SetTestTask:
                        if (command.Entity is int testTaskId)
                        {
                            await _sender.Send(new SetTestTaskRequest(testTaskId), cancellationSource.Token).ConfigureAwait(false);
                        }
                        break;

                    default:
                        logger.LogWarning("{command} not found", command.Command);
                        break;
                }
                await taskQueue.SetStop(command.Id).ConfigureAwait(false);
                await messageService.SendInfo($"Finished task: {command.Command}").ConfigureAwait(false);
                logger.LogInformation("Finished {command}", command.Command);
            }
            catch (OperationCanceledException)
            {
                logger.LogWarning("Task {command} was canceled", command.Command);
                await messageService.SendWarning($"Task canceled: {command.Command}").ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error occurred executing task work item. {command}", command.Command);
                await messageService.SendError($"Error executing task: {command.Command}, {ex.Message}").ConfigureAwait(false);
            }
            finally
            {
                _taskCancellationTokens.TryRemove(command.Id, out _);
                cancellationSource.Dispose();
                await taskQueue.SetStop(command.Id).ConfigureAwait(false);
                _semaphore.Release();
            }
        }
        logger.LogInformation("{nameof(QueuedHostedService)} is stopped.{Environment.NewLine}", nameof(QueuedHostedService), Environment.NewLine);
    }
}