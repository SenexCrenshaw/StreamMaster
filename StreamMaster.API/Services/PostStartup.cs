using StreamMaster.Application.Services;
using StreamMaster.Infrastructure.EF.PGSQL;

namespace StreamMaster.API.Services;

public class PostStartup(ILogger<PostStartup> logger, IServiceProvider serviceProvider, IBackgroundTaskQueue taskQueue) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        if (cancellationToken.IsCancellationRequested)
        {
            return;
        }

        logger.LogInformation("Stream Master is starting.");

        //DirectoryHelper.EmptyDirectory(BuildInfo.HLSOutputFolder);

        using IServiceScope scope = serviceProvider.CreateScope();
        PGSQLRepositoryContext repositoryContext = scope.ServiceProvider.GetRequiredService<PGSQLRepositoryContext>();
        IDataRefreshService dataRefreshService = scope.ServiceProvider.GetRequiredService<IDataRefreshService>();

        IStreamGroupService StreamGroupService = scope.ServiceProvider.GetRequiredService<IStreamGroupService>();
        await StreamGroupService.GetDefaultSGIdAsync();

        repositoryContext.MigrateData();

        await taskQueue.EPGSync(cancellationToken).ConfigureAwait(false);

        await taskQueue.ScanForTvLogos(cancellationToken).ConfigureAwait(false);

        await taskQueue.ScanDirectoryForEPGFiles(cancellationToken).ConfigureAwait(false);

        await taskQueue.ScanForCustomPlayLists(cancellationToken).ConfigureAwait(false);

        await taskQueue.ScanDirectoryForM3UFiles(cancellationToken).ConfigureAwait(false);

        //await taskQueue.UpdateChannelGroupCounts(cancellationToken).ConfigureAwait(false);

        await taskQueue.CacheChannelLogos(cancellationToken).ConfigureAwait(false);

        await taskQueue.CacheStreamLogos(cancellationToken).ConfigureAwait(false);

        while (taskQueue.HasJobs())
        {
            await Task.Delay(250, cancellationToken).ConfigureAwait(false);
        }

        //await dataRefreshService.RefreshAll();

        await taskQueue.SetIsSystemReady(true, cancellationToken).ConfigureAwait(false);
    }
}
