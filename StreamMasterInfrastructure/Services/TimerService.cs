using MediatR;

using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using StreamMasterApplication.EPGFiles.Commands;
using StreamMasterApplication.EPGFiles.Queries;
using StreamMasterApplication.M3UFiles.Commands;
using StreamMasterApplication.M3UFiles.Queries;
using StreamMasterApplication.Settings.Queries;

using StreamMasterDomain.Cache;
using StreamMasterDomain.Repository;

namespace StreamMasterInfrastructure.Services;

public class TimerService : IHostedService, IDisposable
{
    private readonly ILogger<TimerService> _logger;
    private readonly IMemoryCache _memoryCache;
    private readonly IServiceProvider _serviceProvider;
    private readonly object Lock = new();

    private Timer? _timer;
    private bool isActive = false;

    public TimerService(
        IServiceProvider serviceProvider,
        IMemoryCache memoryCache,
        ILogger<TimerService> logger
       )
    {
        _memoryCache = memoryCache;
        _logger = logger;
        _serviceProvider = serviceProvider;
    }

    public void Dispose()
    {
        _timer?.Dispose();
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        //_logger.LogInformation("Timer Service running.");

        _timer = new Timer(async state => await DoWorkAsync(state, cancellationToken), null, TimeSpan.Zero, TimeSpan.FromSeconds(600));

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

        SystemStatus status = new() { IsSystemReady = _memoryCache.IsSystemReady() };

        if (!status.IsSystemReady)
        {
            lock (Lock)
            {
                isActive = false;
            }
            return;
        }

        using IServiceScope scope = _serviceProvider.CreateScope();
        IMediator mediator = scope.ServiceProvider.GetRequiredService<IMediator>();


        IRepositoryWrapper repository = scope.ServiceProvider.GetRequiredService<IRepositoryWrapper>();

        //_logger.LogInformation("Timer Service is working.");

        DateTime now = DateTime.Now;

        IEnumerable<StreamMasterDomain.Dto.EPGFileDto> epgFilesToUpdated = await mediator.Send(new GetEPGFilesNeedUpdating(), cancellationToken).ConfigureAwait(false);
        IEnumerable<StreamMasterDomain.Dto.M3UFileDto> m3uFilesToUpdated = await mediator.Send(new GetM3UFilesNeedUpdating(), cancellationToken).ConfigureAwait(false);

        if (epgFilesToUpdated.Any())
        {
            _logger.LogInformation("EPG Files to update count: {epgFiles.Count()}", epgFilesToUpdated.Count());
            foreach (StreamMasterDomain.Dto.EPGFileDto? epgFile in epgFilesToUpdated)
            {
                _ = await mediator.Send(new RefreshEPGFileRequest(epgFile.Id), cancellationToken).ConfigureAwait(false);
            }
        }

        if (m3uFilesToUpdated.Any())
        {
            _logger.LogInformation("M3U Files to update count: {m3uFiles.Count()}", m3uFilesToUpdated.Count());

            foreach (StreamMasterDomain.Dto.M3UFileDto? m3uFile in m3uFilesToUpdated)
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