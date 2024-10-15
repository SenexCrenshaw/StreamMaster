using Microsoft.EntityFrameworkCore;

using StreamMaster.Domain.Configuration;
using StreamMaster.Domain.Helpers;
using StreamMaster.Infrastructure.EF.Base;

namespace StreamMaster.Infrastructure.EF.PGSQL
{
    public partial class PGSQLRepositoryContext(DbContextOptions<PGSQLRepositoryContext> options) : BaseRepositoryContext(options)
    {
        public static string DbConnectionString => $"Host={BuildInfo.DBHost};Database={BuildInfo.DBName};Username={BuildInfo.DBUser};Password={BuildInfo.DBPassword}";

        public new bool IsEntityTracked<TEntity>(TEntity entity) where TEntity : class
        {
            return ChangeTracker.Entries<TEntity>().Any(e => e.Entity == entity);
        }

        public new void MigrateData()
        {
            CheckFunctions();

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
            return;
        }
        private void CheckFunctions()
        {
            const string sql = "CREATE OR REPLACE FUNCTION public.create_or_update_smstreams_and_channels(\r\n"
                  + "    p_ids TEXT[],\r\n"
                  + "    p_file_positions INTEGER[],\r\n"
                  + "    p_channel_numbers INTEGER[],\r\n"
                  + "    p_groups CITEXT[],\r\n"
                  + "    p_epgids CITEXT[],\r\n"
                  + "    p_logos CITEXT[],\r\n"
                  + "    p_names CITEXT[],\r\n"
                  + "    p_urls CITEXT[],\r\n"
                  + "    p_station_ids CITEXT[],\r\n"
                  + "    p_channel_ids CITEXT[],\r\n"
                  + "    p_channel_names CITEXT[],\r\n"
                  + "    p_is_hidden BOOLEAN[],\r\n"  // New parameter for IsHidden column
                  + "    p_m3u_file_id INTEGER,\r\n"
                  + "    p_m3u_file_name CITEXT,\r\n"
                  + "    p_stream_group_id INTEGER,\r\n"
                  + "    p_create_channels BOOLEAN\r\n"
                  + ")\r\n"
                  + "RETURNS TABLE(channel_id INTEGER)\r\n"
                  + "LANGUAGE plpgsql\r\n"
                  + "AS $$\r\n"
                  + "DECLARE\r\n"
                  + "    channel_exists BOOLEAN;\r\n"
                  + "BEGIN\r\n"
                  + "    FOR i IN array_lower(p_ids, 1)..array_upper(p_ids, 1)\r\n"
                  + "    LOOP\r\n"
                  + "        -- Attempt to update an existing stream\r\n"
                  + "        UPDATE \"SMStreams\"\r\n"
                  + "        SET\r\n"
                  + "            \"FilePosition\" = p_file_positions[i],\r\n"
                  + "            \"ChannelNumber\" = p_channel_numbers[i],\r\n"
                  + "            \"M3UFileId\" = p_m3u_file_id,\r\n"
                  + "            \"M3UFileName\" = p_m3u_file_name,\r\n"
                  + "            \"Group\" = p_groups[i],\r\n"
                  + "            \"EPGID\" = p_epgids[i],\r\n"
                  + "            \"Logo\" = p_logos[i],\r\n"
                  + "            \"Name\" = p_names[i],\r\n"
                  + "            \"StationId\" = p_station_ids[i],\r\n"
                  + "            \"ChannelId\" = p_channel_ids[i],\r\n"
                  + "            \"ChannelName\" = p_channel_names[i],\r\n"
                  + "            \"IsHidden\" = p_is_hidden[i],\r\n"  // Set IsHidden column
                  + "            \"NeedsDelete\" = false -- Unmark this stream\r\n"
                  + "        WHERE \"Id\" = p_ids[i];\r\n"
                  + "\r\n"
                  + "        -- If no rows were updated, perform an insert\r\n"
                  + "        IF NOT FOUND THEN\r\n"
                  + "            INSERT INTO \"SMStreams\" (\r\n"
                  + "                \"Id\",\r\n"
                  + "                \"FilePosition\",\r\n"
                  + "                \"IsHidden\",\r\n"  // Insert IsHidden value
                  + "                \"IsUserCreated\",\r\n"
                  + "                \"M3UFileId\",\r\n"
                  + "                \"ChannelNumber\",\r\n"
                  + "                \"M3UFileName\",\r\n"
                  + "                \"Group\",\r\n"
                  + "                \"EPGID\",\r\n"
                  + "                \"Logo\",\r\n"
                  + "                \"Name\",\r\n"
                  + "                \"Url\",\r\n"
                  + "                \"StationId\",\r\n"
                  + "                \"ChannelId\",\r\n"
                  + "                \"ChannelName\",\r\n"
                  + "                \"IsSystem\",\r\n"
                  + "                \"CUID\",\r\n"
                  + "                \"SMStreamType\",\r\n"
                  + "                \"NeedsDelete\"\r\n"
                  + "            ) VALUES (\r\n"
                  + "                p_ids[i],\r\n"
                  + "                p_file_positions[i],\r\n"
                  + "                p_is_hidden[i],  -- Insert IsHidden value\r\n"
                  + "                false,  -- IsUserCreated (constant value)\r\n"
                  + "                p_m3u_file_id,\r\n"
                  + "                p_channel_numbers[i],\r\n"
                  + "                p_m3u_file_name,\r\n"
                  + "                p_groups[i],\r\n"
                  + "                p_epgids[i],\r\n"
                  + "                p_logos[i],\r\n"
                  + "                p_names[i],\r\n"
                  + "                p_urls[i],\r\n"
                  + "                p_station_ids[i],\r\n"
                  + "                p_channel_ids[i],\r\n"
                  + "                p_channel_names[i],\r\n"
                  + "                false,  -- IsSystem (constant value)\r\n"
                  + "                '',  -- Default or COALESCE value for CUID\r\n"
                  + "                0,  -- SMStreamType (constant value)\r\n"
                  + "                false  -- NeedsDelete (new stream, not marked for deletion)\r\n"
                  + "            );\r\n"
                  + "        END IF;\r\n"
                  + "\r\n"
                  + "        -- Optional: Create a channel if p_create_channels is true and the channel does not already exist\r\n"
                  + "        IF p_create_channels THEN\r\n"
                  + "            -- Check if the channel with the given BaseStreamID already exists\r\n"
                  + "            SELECT EXISTS (\r\n"
                  + "                SELECT 1 FROM \"SMChannels\" WHERE \"BaseStreamID\" = p_ids[i]\r\n"
                  + "            ) INTO channel_exists;\r\n"
                  + "\r\n"
                  + "            -- If the channel does not exist, create it\r\n"
                  + "            IF NOT channel_exists THEN\r\n"
                  + "                -- Insert into \"SMChannels\"\r\n"
                  + "                INSERT INTO \"SMChannels\" (\r\n"
                  + "                    \"CommandProfileName\",\r\n"
                  + "                    \"IsHidden\",\r\n"
                  + "                    \"BaseStreamID\",\r\n"
                  + "                    \"M3UFileId\",\r\n"
                  + "                    \"ChannelNumber\",\r\n"
                  + "                    \"TimeShift\",\r\n"
                  + "                    \"Group\",\r\n"
                  + "                    \"EPGId\",\r\n"
                  + "                    \"Logo\",\r\n"
                  + "                    \"Name\",\r\n"
                  + "                    \"StationId\",\r\n"
                  + "                    \"ChannelId\",\r\n"
                  + "                    \"ChannelName\",\r\n"
                  + "                    \"GroupTitle\",\r\n"
                  + "                    \"IsSystem\",\r\n"
                  + "                    \"SMChannelType\"\r\n"
                  + "                ) VALUES (\r\n"
                  + "                    'Default',\r\n"
                  + "                    false,  -- IsHidden (constant value)\r\n"
                  + "                    p_ids[i],  -- Using the same Id as BaseStreamID\r\n"
                  + "                    p_m3u_file_id,\r\n"
                  + "                    p_channel_numbers[i],\r\n"
                  + "                    0,  -- TimeShift (constant value)\r\n"
                  + "                    p_groups[i],\r\n"
                  + "                    p_epgids[i],\r\n"
                  + "                    p_logos[i],\r\n"
                  + "                    p_names[i],\r\n"
                  + "                    p_station_ids[i],\r\n"
                  + "                    p_channel_ids[i],\r\n"
                  + "                    p_channel_names[i],\r\n"
                  + "                    '',  -- Default or COALESCE value for GroupTitle\r\n"
                  + "                    false,  -- IsSystem (constant value)\r\n"
                  + "                    0  -- SMChannelType (constant value)\r\n"
                  + "                ) RETURNING \"Id\" INTO channel_id;\r\n"
                  + "\r\n"
                  + "                -- Insert into \"SMChannelStreamLinks\"\r\n"
                  + "                INSERT INTO \"SMChannelStreamLinks\" (\r\n"
                  + "                    \"SMChannelId\",\r\n"
                  + "                    \"SMStreamId\",\r\n"
                  + "                    \"Rank\"\r\n"
                  + "                ) VALUES (\r\n"
                  + "                    channel_id,\r\n"
                  + "                    p_ids[i],                   \r\n"
                  + "                    0  -- Rank (constant value)\r\n"
                  + "                )\r\n"
                  + "                ON CONFLICT (\"SMChannelId\", \"SMStreamId\") DO NOTHING;\r\n"
                  + "\r\n"
                  + "                -- Insert into StreamGroupSMChannelLink table if p_stream_group_id is not 0\r\n"
                  + "                IF p_stream_group_id != 0 THEN\r\n"
                  + "                    INSERT INTO \"StreamGroupSMChannelLink\" (\r\n"
                  + "                        \"SMChannelId\",\r\n"
                  + "                        \"StreamGroupId\",\r\n"
                  + "                        \"IsReadOnly\",\r\n"
                  + "                        \"Rank\"\r\n"
                  + "                    ) VALUES (\r\n"
                  + "                        channel_id,\r\n"
                  + "                        p_stream_group_id,\r\n"
                  + "                        false,  -- IsReadOnly (constant value)\r\n"
                  + "                        0  -- Rank (constant value)\r\n"
                  + "                    )\r\n"
                  + "                    ON CONFLICT (\"SMChannelId\", \"StreamGroupId\") DO NOTHING;\r\n"
                  + "                END IF;\r\n"
                  + "\r\n"
                  + "                -- Return the channel ID\r\n"
                  + "                RETURN NEXT;\r\n"
                  + "            END IF;\r\n"
                  + "        END IF;\r\n"
                  + "    END LOOP;\r\n"
                  + "END $$;\r\n";

            Database.ExecuteSqlRaw(sql);

        }

        //public static readonly ILoggerFactory MyLoggerFactory = LoggerFactory.Create(builder =>
        //{
        //    builder
        //        .AddConsole()
        //        .AddFilter((category, level) =>
        //            category == DbLoggerCategory.Database.Command.Name &&
        //            level == LogLevel.Information)
        //        .AddDebug();
        //});

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
            Setting? setting = SettingsHelper.GetSetting<Setting>(BuildInfo.SettingsFile);
            if (setting?.EnableDBDebug == true)
            {
                options.EnableSensitiveDataLogging(true);
            }
        }

        protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
        {
        }
    }
}