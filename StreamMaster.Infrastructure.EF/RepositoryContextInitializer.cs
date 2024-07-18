using Microsoft.EntityFrameworkCore;

using StreamMaster.Domain.Configuration;
using StreamMaster.Domain.Helpers;
using StreamMaster.Infrastructure.EF.PGSQL;

namespace StreamMaster.Infrastructure.EF;

public class RepositoryContextInitializer(ILogger<RepositoryContextInitializer> logger, PGSQLRepositoryContext context)
{
    public async Task InitializeAsync(Setting settings)
    {
        try
        {
            await context.Database.MigrateAsync().ConfigureAwait(false);

            if (!context.StreamGroups.Any(a => a.Name == "ALL"))
            {
                var sg = new StreamGroup { Id = 0, Name = "ALL", IsReadOnly = true, IsSystem = true, DeviceID = settings.DeviceID };
                context.Add(sg);
                var profile = new StreamGroupProfile
                {
                    Name = "Default",
                    OutputProfileName = "Default",
                    VideoProfileName = "Default"
                };

                context.StreamGroupProfiles.Add(profile);
                sg.StreamGroupProfiles.Add(profile);

                await context.SaveChangesAsync().ConfigureAwait(false);
            }

            if (!context.ChannelGroups.Any(a => a.Name == "Dummy"))
            {
                context.Add(new ChannelGroup { Name = "Dummy", IsReadOnly = true, IsSystem = true });
                await context.SaveChangesAsync().ConfigureAwait(false);
            }

            if (!context.ChannelGroups.Any(a => a.Name == "CustomPlayList"))
            {
                context.Add(new ChannelGroup { Name = "CustomPlayList", IsReadOnly = true, IsSystem = true });
                await context.SaveChangesAsync().ConfigureAwait(false);
            }
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

    public void MigrateData()
    {
    }
}
