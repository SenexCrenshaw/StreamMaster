﻿using MediatR;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using StreamMasterApplication.ChannelGroups.Commands;
using StreamMasterApplication.Common.Models;
using StreamMasterApplication.EPGFiles.Commands;
using StreamMasterApplication.General.Commands;
using StreamMasterApplication.Icons.Commands;
using StreamMasterApplication.M3UFiles.Commands;
using StreamMasterApplication.SchedulesDirectAPI.Commands;
using StreamMasterApplication.Services;

using StreamMasterDomain.Enums;

namespace StreamMasterInfrastructure.Services.QueueService;

public sealed class QueuedHostedService : BackgroundService
{
    private readonly ILogger<QueuedHostedService> _logger;

    private readonly IServiceProvider _serviceProvider;

    private readonly IBackgroundTaskQueue _taskQueue;

    public QueuedHostedService(
        IBackgroundTaskQueue taskQueue,
        IServiceProvider serviceProvider,
        ILogger<QueuedHostedService> logger

        )
    {
        _taskQueue = taskQueue;
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    public override async Task StopAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation(
            "{nameof(QueuedHostedService)} is stopping.", nameof(QueuedHostedService));

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
            BackgroundTaskQueueConfig command = await _taskQueue.DeQueueAsync(cancellationToken).ConfigureAwait(false);

            try
            {
                _logger.LogInformation("Starting {command}", command.Command);
                using IServiceScope scope = _serviceProvider.CreateScope();

                ISender _sender = scope.ServiceProvider.GetRequiredService<ISender>();

                await _taskQueue.SetStart(command.Id).ConfigureAwait(false);

                switch (command.Command)
                {
                    case SMQueCommand.BuildIconCaches:
                        await _sender.Send(new BuildIconCachesRequest(), cancellationToken).ConfigureAwait(false);
                        break;

                    case SMQueCommand.BuildProgIconsCacheFromEPGs:
                        _ = await _sender.Send(new BuildProgIconsCacheFromEPGsRequest(), cancellationToken).ConfigureAwait(false);
                        break;

                    case SMQueCommand.BuildIconsCacheFromVideoStreams:
                        _ = await _sender.Send(new BuildIconsCacheFromVideoStreamRequest(), cancellationToken).ConfigureAwait(false);
                        break;

                    case SMQueCommand.ReadDirectoryLogosRequest:

                        await _sender.Send(new ReadDirectoryLogosRequest(), cancellationToken).ConfigureAwait(false);
                        break;

                    case SMQueCommand.ProcessEPGFile:
                        if (command.Entity is not null && command.Entity.GetType() == typeof(int))
                        {
                            _ = await _sender.Send(new ProcessEPGFileRequest((int)command.Entity), cancellationToken).ConfigureAwait(false);
                        }
                        break;
                    case SMQueCommand.EPGSync:
                        await _sender.Send(new EPGSync(), cancellationToken).ConfigureAwait(false);
                        break;

                    case SMQueCommand.UpdateChannelGroupCounts:
                        await _sender.Send(new UpdateChannelGroupCountsByIdsRequest(), cancellationToken).ConfigureAwait(false);
                        break;

                    case SMQueCommand.ProcessM3UFile:

                        if (command.Entity is not null && command.Entity.GetType() == typeof(int))
                        {
                            _ = await _sender.Send(new ProcessM3UFileRequest((int)command.Entity), cancellationToken).ConfigureAwait(false);
                        }
                        break;

                    case SMQueCommand.ProcessM3UFiles:

                        await _sender.Send(new ProcessM3UFilesRequest(), cancellationToken).ConfigureAwait(false);

                        break;

                    case SMQueCommand.ScanDirectoryForIconFiles:
                        _ = await _sender.Send(new ScanDirectoryForIconFilesRequest(), cancellationToken).ConfigureAwait(false);
                        break;

                    case SMQueCommand.ScanDirectoryForM3UFiles:
                        _ = await _sender.Send(new ScanDirectoryForM3UFilesRequest(), cancellationToken).ConfigureAwait(false);
                        break;

                    case SMQueCommand.ScanDirectoryForEPGFiles:
                        _ = await _sender.Send(new ScanDirectoryForEPGFilesRequest(), cancellationToken).ConfigureAwait(false);
                        break;

                    case SMQueCommand.SetIsSystemReady:
                        if (command.Entity is not null && command.Entity.GetType() == typeof(bool))
                        {
                            await _sender.Send(new SetIsSystemReadyRequest((bool)command.Entity), cancellationToken).ConfigureAwait(false);
                        }
                        break;

                    default:
                        _logger.LogWarning("{command} not found", command.Command);
                        break;
                }
                await _taskQueue.SetStop(command.Id).ConfigureAwait(false);
                _logger.LogInformation("Finished {command}", command.Command);
            }
            catch (OperationCanceledException)
            {
                // Prevent throwing if stoppingToken was signaled
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred executing task work item. {command}", command.Command);
                await _taskQueue.SetStop(command.Id).ConfigureAwait(false);
            }
        }
        _logger.LogInformation("{nameof(QueuedHostedService)} is stopped.{Environment.NewLine}", nameof(QueuedHostedService), Environment.NewLine);
    }
}