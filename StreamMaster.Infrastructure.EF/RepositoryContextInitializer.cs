using Microsoft.EntityFrameworkCore;

using StreamMaster.Domain.Helpers;
using StreamMaster.Infrastructure.EF.PGSQL;

namespace StreamMaster.Infrastructure.EF;

public class RepositoryContextInitializer(ILogger<RepositoryContextInitializer> logger, PGSQLRepositoryContext context)
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

        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while initialising the database.");
            throw;
        }
    }

    public void TrySeed()
    {
        DirectoryHelper.CreateApplicationDirectories();
    }

    public void MigrateData()
    {
        CheckShortIDs();
    }

    private void CheckShortIDs()
    {
        List<VideoStream> videos = [.. context.VideoStreams.Where(a => a.ShortId == UniqueHexGenerator.ShortIdEmpty)];
        if (videos.Count == 0)
        {
            //Console.WriteLine("No shortids need fixing", videos.Count);
            return;
        }

        Console.WriteLine($"Fixing {videos.Count} empty shortids");

        HashSet<string> ids = [.. context.VideoStreams.Select(a => a.ShortId)];

        foreach (VideoStream? video in videos)
        {
            video.ShortId = UniqueHexGenerator.GenerateUniqueHex(ids);
        }

        context.SaveChanges();
        Console.WriteLine($"Done fixing empty shortids");
    }
}

