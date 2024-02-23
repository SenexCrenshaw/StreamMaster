using StreamMaster.Application.Services;
using StreamMaster.Domain.Helpers;
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

        logger.LogInformation($"Stream Master is starting.");

        DirectoryHelper.EmptyDirectory(BuildInfo.HLSOutputFolder, logger);

        using IServiceScope scope = serviceProvider.CreateScope();
        PGSQLRepositoryContext repositoryContext = scope.ServiceProvider.GetRequiredService<PGSQLRepositoryContext>();

        //ISchedulesDirectDataService schedulesDirectService = scope.ServiceProvider.GetRequiredService<ISchedulesDirectDataService>();
        await repositoryContext.MigrateData();

        await taskQueue.EPGSync(cancellationToken).ConfigureAwait(false);

        await taskQueue.ReadDirectoryLogos(cancellationToken).ConfigureAwait(false);

        await taskQueue.ScanDirectoryForEPGFiles(cancellationToken).ConfigureAwait(false);

        await taskQueue.ScanDirectoryForM3UFiles(cancellationToken).ConfigureAwait(false);

        await taskQueue.UpdateChannelGroupCounts(cancellationToken).ConfigureAwait(false);

        await taskQueue.BuildIconCaches(cancellationToken).ConfigureAwait(false);


        while (taskQueue.HasJobs())
        {
            await Task.Delay(250, cancellationToken).ConfigureAwait(false);
        }


        await taskQueue.SetIsSystemReady(true, cancellationToken).ConfigureAwait(false);


    }
}
