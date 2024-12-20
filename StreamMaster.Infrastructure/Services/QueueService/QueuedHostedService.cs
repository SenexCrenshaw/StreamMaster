﻿using MediatR;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using StreamMaster.Application.Common.Models;
using StreamMaster.Application.Custom.Commands;
using StreamMaster.Application.EPGFiles.Commands;
using StreamMaster.Application.General.Commands;
using StreamMaster.Application.Logos.Commands;
using StreamMaster.Application.Logos.CommandsOld;
using StreamMaster.Application.M3UFiles.Commands;
using StreamMaster.Application.SchedulesDirect.Commands;
using StreamMaster.Application.Services;
using StreamMaster.Domain.Enums;

namespace StreamMaster.Infrastructure.Services.QueueService;

public sealed class QueuedHostedService(
    IBackgroundTaskQueue taskQueue,
    IServiceProvider serviceProvider,
    IMessageService messageService,
    ILogger<QueuedHostedService> logger
) : BackgroundService
{
    public override async Task StopAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation(
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
            BackgroundTaskQueueConfig command = await taskQueue.DeQueueAsync(cancellationToken).ConfigureAwait(false);

            try
            {
                logger.LogInformation("Starting {command}", command.Command);
                await messageService.SendInfo($"Starting task: {command.Command}");

                using IServiceScope scope = serviceProvider.CreateScope();

                ISender _sender = scope.ServiceProvider.GetRequiredService<ISender>();


                await taskQueue.SetStart(command.Id).ConfigureAwait(false);

                switch (command.Command)
                {
                    case SMQueCommand.BuildLogoCaches:
                        await _sender.Send(new BuildLogoCachesRequest(), cancellationToken).ConfigureAwait(false);
                        break;

                    case SMQueCommand.BuildLogosCacheFromStreams:
                        await _sender.Send(new BuildLogosCacheFromStreamsRequest(), cancellationToken).ConfigureAwait(false);
                        break;

                    case SMQueCommand.BuildProgLogosCacheFromEPGs:
                        _ = await _sender.Send(new BuildProgLogosCacheFromEPGsRequest(), cancellationToken).ConfigureAwait(false);
                        break;

                    case SMQueCommand.BuildLogosCacheFromVideoStreams:
                        _ = await _sender.Send(new BuildLogosCacheFromStreamsRequest(), cancellationToken).ConfigureAwait(false);
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
                        _ = await _sender.Send(new EPGSync(), cancellationToken).ConfigureAwait(false);
                        break;

                    case SMQueCommand.ProcessM3UFile:

                        if (command.Entity is not null && command.Entity.GetType() == typeof(ProcessM3UFileRequest))
                        {
                            ProcessM3UFileRequest? p = command.Entity as ProcessM3UFileRequest;
                            _ = await _sender.Send(p, cancellationToken).ConfigureAwait(false);
                        }
                        break;

                    case SMQueCommand.ScanForCustomPlayLists:
                        await _sender.Send(new ScanForCustomRequest(), cancellationToken).ConfigureAwait(false);
                        break;

                    case SMQueCommand.ProcessM3UFiles:

                        await _sender.Send(new ProcessM3UFilesRequest(), cancellationToken).ConfigureAwait(false);

                        break;

                    case SMQueCommand.ScanDirectoryForLogoFiles:
                        _ = await _sender.Send(new ScanDirectoryForLogoFilesRequest(), cancellationToken).ConfigureAwait(false);
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
                    case SMQueCommand.SetTestTask:
                        if (command.Entity is not null && command.Entity.GetType() == typeof(int))
                        {
                            await _sender.Send(new SetTestTaskRequest((int)command.Entity)).ConfigureAwait(false);
                        }
                        break;
                    default:
                        logger.LogWarning("{command} not found", command.Command);
                        break;
                }
                await taskQueue.SetStop(command.Id).ConfigureAwait(false);
                await messageService.SendInfo($"Finished task: {command.Command}");
                logger.LogInformation("Finished {command}", command.Command);
            }
            catch (OperationCanceledException)
            {
                // Prevent throwing if stoppingToken was signaled
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error occurred executing task work item. {command}", command.Command);
                await messageService.SendError($"Error executing task: {command.Command}, {ex.Message}");

                await taskQueue.SetStop(command.Id).ConfigureAwait(false);

            }
        }
        logger.LogInformation("{nameof(QueuedHostedService)} is stopped.{Environment.NewLine}", nameof(QueuedHostedService), Environment.NewLine);
    }
}