using Microsoft.EntityFrameworkCore;

using StreamMaster.Domain.Configuration;
using StreamMaster.Domain.Helpers;
using StreamMaster.Infrastructure.EF.Base;
using StreamMaster.SchedulesDirect.Domain.Models;

namespace StreamMaster.Infrastructure.EF.PGSQL
{
    public partial class PGSQLRepositoryContext(DbContextOptions<PGSQLRepositoryContext> options) : BaseRepositoryContext(options)
    {

        public static string DbConnectionString => $"Host={BuildInfo.DBHost};Database={BuildInfo.DBName};Username={BuildInfo.DBUser};Password={BuildInfo.DBPassword}";

        new public bool IsEntityTracked<TEntity>(TEntity entity) where TEntity : class
        {
            return ChangeTracker.Entries<TEntity>().Any(e => e.Entity == entity);
        }

        public async Task MigrateData(List<MxfService>? allServices = null)
        {
            string? currentMigration = Database.GetAppliedMigrations().LastOrDefault();
            if (currentMigration == null)
            {
                return;
            }

            //if (currentMigration == "20240229201207_ConvertToSMChannels")
            //{
            //if (!SystemKeyValues.Any(a => a.Key == "MigrateData_20240229201207_ConvertToSMChannels"))
            //{
            //    MigrateData_20240229201207_ConvertToSMChannels();
            //    SystemKeyValues.Add(new SystemKeyValue { Key = "MigrateData_20240229201207_ConvertToSMChannels", Value = "1" });
            //    await SaveChangesAsync().ConfigureAwait(false);
            //}

            //}
        }

        protected void MigrateData_20240229201207_ConvertToSMChannels()
        {
            Console.WriteLine("Migrating videostream custom information to SMChannels");
            SMChannels.RemoveRange(SMChannels);
            SMStreams.RemoveRange(SMStreams);
            SMChannelStreamLinks.RemoveRange(SMChannelStreamLinks);
            StreamGroupSMChannels.RemoveRange(StreamGroupSMChannels);
            SaveChanges();

            int counter = 0;
            //List<VideoStream> videoStreams = [.. VideoStreams];
            //foreach (VideoStream videoStream in videoStreams)
            //{
            //    //SMChannel channel = new()
            //    //{
            //    //    ChannelNumber = smChannelDto.ChannelNumber,
            //    //    Group = videoStream.User_Tvg_group,
            //    //    EPGId = smChannelDto.EPGId,
            //    //    Logo = smChannelDto.Logo,
            //    //    Name = smChannelDto.Name,
            //    //    StationId = videoStream.StationId,
            //    //    TimeShift = videoStream.TimeShift,
            //    //    VideoStreamHandler = videoStream.VideoStreamHandler,
            //    //    IsHidden = videoStream.IsHidden,
            //    //    GroupTitle = videoStream.GroupTitle,
            //    //    StreamingProxyType = videoStream.StreamingProxyType,
            //    //    SMStreamId = videoStream.Id
            //    //};
            //    //SMChannels.Add(channel);

            //    SMStream smStream = new()
            //    {
            //        Id = videoStream.Id,
            //        FilePosition = videoStream.FilePosition,
            //        IsHidden = videoStream.IsHidden,
            //        IsUserCreated = videoStream.IsUserCreated,
            //        M3UFileId = videoStream.M3UFileId,
            //        M3UFileName = videoStream.M3UFileName,
            //        ChannelNumber = videoStream.Tvg_chno,
            //        ShortSMChannelId = videoStream.ShortSMChannelId,
            //        StationId = videoStream.StationId,
            //        Group = videoStream.Tvg_group,
            //        EPGId = videoStream.Tvg_ID,
            //        Logo = videoStream.Tvg_logo,
            //        Name = videoStream.Tvg_name,
            //        Url = videoStream.Url
            //    };
            //    SMStreams.Add(smStream);

            //    counter++;
            //    if (counter >= 500) // Check if 500 entities have been processed
            //    {
            //        SaveChanges();
            //        counter = 0; // Reset the counter after saving
            //    }
            //}

            //if (counter > 0) // Check if there are any remaining entities to save after the loop
            //{
            //    SaveChanges(); // Save the remaining entities
            //    counter = 0;
            //}

            //List<StreamGroupVideoStream> streamGroupVideoStreams = [.. StreamGroupVideoStreams];
            //foreach (StreamGroupVideoStream streamGroupVideoStream in streamGroupVideoStreams)
            //{
            //    int channelId = SMChannels.First(a => a.VideoStreamId == streamGroupVideoStream.ChildVideoStreamId).Id;
            //    StreamGroupSMChannelLink streamGroupSMChannel = new()
            //    {
            //        ShortSMChannelId = channelId,
            //        StreamGroupId = streamGroupVideoStream.StreamGroupId,
            //        Rank = streamGroupVideoStream.Rank,
            //        IsReadOnly = streamGroupVideoStream.IsReadOnly
            //    };
            //    StreamGroupSMChannels.Add(streamGroupSMChannel);
            //}
            //SaveChanges();

            //List<VideoStreamLink> videoStreamLinks = VideoStreamLinks.ToList();
            //foreach (VideoStreamLink videoStreamLink in videoStreamLinks)
            //{
            //    int channelId = SMChannels.First(a => a.VideoStreamId == videoStreamLink.ParentVideoStreamId).Id;
            //    SMChannelStreamLink smChannelStreamLink = new()
            //    {
            //        ShortSMChannelId = channelId,
            //        SMStreamId = videoStreamLink.ChildVideoStreamId,
            //        Rank = videoStreamLink.Rank
            //    };
            //    SMChannelStreamLinks.Add(smChannelStreamLink);
            //}
            SaveChanges();
        }


        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            DirectoryHelper.CreateApplicationDirectories();
            options.UseNpgsql(DbConnectionString,
                o =>
                {
                    o.UseNodaTime();
                    o.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
                }
                );

        }

        protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
        {
        }

    }
}