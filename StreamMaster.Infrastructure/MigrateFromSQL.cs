using StreamMaster.Domain.Common;
using StreamMaster.Infrastructure.EF.PGSQL;
using StreamMaster.Infrastructure.EF.SQLite;

namespace StreamMaster.Infrastructure
{
    public static class MigrateFromSQLite
    {
        public static bool MigrateFromSQLiteDatabaseToPostgres(PGSQLRepositoryContext repositoryContext, SQLiteRepositoryContext sQLiteRepositoryContext)
        {
            try
            {
                string sqliteDB = Path.Join(BuildInfo.AppDataFolder, "StreamMaster.db");
                if (File.Exists(sqliteDB))
                {
                    Console.WriteLine("Migrating from SQLite to Postgres");

                    List<EPGFile> epgFiles = sQLiteRepositoryContext.EPGFiles.ToList();
                    if (epgFiles.Count > 0)
                    {
                        Console.WriteLine($"Migrating {epgFiles.Count} EPG files");
                        repositoryContext.EPGFiles.RemoveRange(repositoryContext.EPGFiles);
                        repositoryContext.EPGFiles.AddRange(epgFiles);
                        foreach (EPGFile epgFile in epgFiles)
                        {
                            epgFile.LastUpdated = DateTime.SpecifyKind(epgFile.LastUpdated.ToUniversalTime(), DateTimeKind.Utc);
                            epgFile.LastDownloadAttempt = DateTime.SpecifyKind(epgFile.LastDownloadAttempt.ToUniversalTime(), DateTimeKind.Utc);
                            epgFile.LastDownloaded = DateTime.SpecifyKind(epgFile.LastDownloaded.ToUniversalTime(), DateTimeKind.Utc);

                        }
                        repositoryContext.SaveChanges();
                        Console.WriteLine("Migrated EPG files");
                    }

                    List<M3UFile> m3UFiles = sQLiteRepositoryContext.M3UFiles.ToList();
                    if (m3UFiles.Count > 0)
                    {
                        Console.WriteLine($"Migrating {m3UFiles.Count} M3U files");
                        repositoryContext.M3UFiles.RemoveRange(repositoryContext.M3UFiles);
                        repositoryContext.M3UFiles.AddRange(m3UFiles);
                        foreach (M3UFile m3uFile in m3UFiles)
                        {
                            m3uFile.LastUpdated = DateTime.SpecifyKind(m3uFile.LastUpdated.ToUniversalTime(), DateTimeKind.Utc);
                            m3uFile.LastDownloadAttempt = DateTime.SpecifyKind(m3uFile.LastDownloadAttempt.ToUniversalTime(), DateTimeKind.Utc);
                            m3uFile.LastDownloaded = DateTime.SpecifyKind(m3uFile.LastDownloaded.ToUniversalTime(), DateTimeKind.Utc);

                        }
                        repositoryContext.SaveChanges();
                        Console.WriteLine("Migrated M3U files");
                    }


                    List<ChannelGroup> channelGroups = sQLiteRepositoryContext.ChannelGroups.ToList();
                    if (channelGroups.Count > 0)
                    {
                        Console.WriteLine($"Migrating {channelGroups.Count} channel groups");
                        repositoryContext.ChannelGroups.RemoveRange(repositoryContext.ChannelGroups);
                        repositoryContext.ChannelGroups.AddRange(channelGroups);
                        repositoryContext.SaveChanges();
                        Console.WriteLine("Migrated channel groups");
                    }

                    List<VideoStream> videoStreams = sQLiteRepositoryContext.VideoStreams.ToList();
                    if (videoStreams.Count > 0)
                    {
                        Console.WriteLine($"Migrating {videoStreams.Count} video streams");
                        repositoryContext.VideoStreams.RemoveRange(repositoryContext.VideoStreams);
                        repositoryContext.VideoStreams.AddRange(videoStreams);
                        repositoryContext.SaveChanges();
                        Console.WriteLine("Migrated video streams");
                    }

                    List<StreamGroup> streamGroups = sQLiteRepositoryContext.StreamGroups.ToList();
                    if (streamGroups.Count > 0)
                    {
                        Console.WriteLine($"Migrating {streamGroups.Count} stream groups");
                        repositoryContext.StreamGroups.RemoveRange(repositoryContext.StreamGroups);
                        repositoryContext.StreamGroups.AddRange(streamGroups);
                        repositoryContext.SaveChanges();
                        Console.WriteLine("Migrated stream groups");
                    }

                    List<VideoStreamLink> videoStreamLinks = sQLiteRepositoryContext.VideoStreamLinks.ToList();
                    if (videoStreamLinks.Count > 0)
                    {
                        Console.WriteLine($"Migrating {videoStreamLinks.Count} video stream links");
                        repositoryContext.VideoStreamLinks.RemoveRange(repositoryContext.VideoStreamLinks);
                        repositoryContext.VideoStreamLinks.AddRange(videoStreamLinks);
                        repositoryContext.SaveChanges();
                        Console.WriteLine("Migrated video stream links");
                    }

                    List<StreamGroupChannelGroup> streamGroupChannelGroups = sQLiteRepositoryContext.StreamGroupChannelGroups.ToList();
                    if (streamGroupChannelGroups.Count > 0)
                    {
                        Console.WriteLine($"Migrating {streamGroupChannelGroups.Count} stream group channel groups");
                        repositoryContext.StreamGroupChannelGroups.RemoveRange(repositoryContext.StreamGroupChannelGroups);
                        repositoryContext.StreamGroupChannelGroups.AddRange(streamGroupChannelGroups);
                        repositoryContext.SaveChanges();
                        Console.WriteLine("Migrated stream group channel groups");
                    }

                    List<StreamGroupVideoStream> streamGroupVideoStreams = sQLiteRepositoryContext.StreamGroupVideoStreams.ToList();
                    if (streamGroupVideoStreams.Count > 0)
                    {
                        Console.WriteLine($"Migrating {streamGroupVideoStreams.Count} stream group video streams");
                        repositoryContext.StreamGroupVideoStreams.RemoveRange(repositoryContext.StreamGroupVideoStreams);
                        repositoryContext.StreamGroupVideoStreams.AddRange(streamGroupVideoStreams);
                        repositoryContext.SaveChanges();
                        Console.WriteLine("Migrated stream group video streams");
                    }

                    Console.WriteLine("Migration from SQLite to Postgres complete");
                    return true;
                }
            }
            catch
            {

            }
            return false;
        }

    }
}
