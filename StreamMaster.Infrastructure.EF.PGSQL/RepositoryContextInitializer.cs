using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace StreamMaster.Infrastructure.EF.PGSQL;

public class RepositoryContextInitializer(ILogger<RepositoryContextInitializer> logger, RepositoryContext context)
{
    public async Task InitialiseAsync()
    {
        try
        {
            await context.Database.MigrateAsync().ConfigureAwait(false);
            if (!context.StreamGroups.Any(a => a.Name == "ALL"))
            {
                context.Add(new StreamGroup { Name = "ALL", IsReadOnly = true });
                await context.SaveChangesAsync().ConfigureAwait(false);
            }

            if (!context.ChannelGroups.Any(a => a.Name == "(None)"))
            {
                context.Add(new ChannelGroup { Name = "(None)", IsReadOnly = true });
                await context.SaveChangesAsync().ConfigureAwait(false);
            }
            //_context.Database.ExecuteSqlRaw("PRAGMA journal_mode = 'delete';");

        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while initialising the database.");
            throw;
        }
    }

    public void TrySeed()
    {
        FileUtil.SetupDirectories();
    }

    public void MigrateData()
    {

    }
}