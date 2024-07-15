using Microsoft.EntityFrameworkCore;

using StreamMaster.Domain.Helpers;
using StreamMaster.Infrastructure.EF.PGSQL.Logging;

namespace StreamMaster.Infrastructure.EF;

public class LogDbContextInitialiser(ILogger<LogDbContextInitialiser> logger, LogDbContext context)
{
    public async Task InitialiseAsync()
    {
        try
        {
            await context.Database.MigrateAsync().ConfigureAwait(false);
            context.RemoveRange(context.LogEntries);
            await context.SaveChangesAsync().ConfigureAwait(false);

        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while initializing the database.");
            throw;
        }
    }

    public void TrySeed()
    {
        DirectoryHelper.CreateApplicationDirectories();
    }
}
