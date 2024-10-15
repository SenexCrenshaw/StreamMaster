using Microsoft.EntityFrameworkCore;

using StreamMaster.Domain.Configuration;
using StreamMaster.Domain.Helpers;
using StreamMaster.Infrastructure.EF.Base;

using System.Text;

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
            StringBuilder sqlBuilder = new();

            sqlBuilder.AppendLine("CREATE OR REPLACE FUNCTION public.create_or_update_smstreams_and_channels(");
            sqlBuilder.AppendLine("    p_ids TEXT[],");
            sqlBuilder.AppendLine("    p_file_positions INTEGER[],");
            sqlBuilder.AppendLine("    p_channel_numbers INTEGER[],");
            sqlBuilder.AppendLine("    p_groups CITEXT[],");
            sqlBuilder.AppendLine("    p_epgids CITEXT[],");
            sqlBuilder.AppendLine("    p_logos CITEXT[],");
            sqlBuilder.AppendLine("    p_names CITEXT[],");
            sqlBuilder.AppendLine("    p_urls CITEXT[],");
            sqlBuilder.AppendLine("    p_station_ids CITEXT[],");
            sqlBuilder.AppendLine("    p_channel_ids CITEXT[],");
            sqlBuilder.AppendLine("    p_channel_names CITEXT[],");
            sqlBuilder.AppendLine("    p_is_hidden BOOLEAN[],");
            sqlBuilder.AppendLine("    p_tvg_names CITEXT[],");  // New parameter for TVGName column
            sqlBuilder.AppendLine("    p_m3u_file_id INTEGER,");
            sqlBuilder.AppendLine("    p_m3u_file_name CITEXT,");
            sqlBuilder.AppendLine("    p_stream_group_id INTEGER,");
            sqlBuilder.AppendLine("    p_create_channels BOOLEAN");
            sqlBuilder.AppendLine(")");
            sqlBuilder.AppendLine("RETURNS TABLE(channel_id INTEGER)");
            sqlBuilder.AppendLine("LANGUAGE plpgsql");
            sqlBuilder.AppendLine("AS $$");
            sqlBuilder.AppendLine("DECLARE");
            sqlBuilder.AppendLine("    channel_exists BOOLEAN;");
            sqlBuilder.AppendLine("BEGIN");
            sqlBuilder.AppendLine("    FOR i IN array_lower(p_ids, 1)..array_upper(p_ids, 1)");
            sqlBuilder.AppendLine("    LOOP");
            sqlBuilder.AppendLine("        -- Attempt to update an existing stream");
            sqlBuilder.AppendLine("        UPDATE \"SMStreams\"");
            sqlBuilder.AppendLine("        SET");
            sqlBuilder.AppendLine("            \"FilePosition\" = p_file_positions[i],");
            sqlBuilder.AppendLine("            \"ChannelNumber\" = p_channel_numbers[i],");
            sqlBuilder.AppendLine("            \"M3UFileId\" = p_m3u_file_id,");
            sqlBuilder.AppendLine("            \"M3UFileName\" = p_m3u_file_name,");
            sqlBuilder.AppendLine("            \"Group\" = p_groups[i],");
            sqlBuilder.AppendLine("            \"EPGID\" = p_epgids[i],");
            sqlBuilder.AppendLine("            \"Logo\" = p_logos[i],");
            sqlBuilder.AppendLine("            \"Name\" = p_names[i],");
            sqlBuilder.AppendLine("            \"StationId\" = p_station_ids[i],");
            sqlBuilder.AppendLine("            \"ChannelId\" = p_channel_ids[i],");
            sqlBuilder.AppendLine("            \"ChannelName\" = p_channel_names[i],");
            sqlBuilder.AppendLine("            \"IsHidden\" = p_is_hidden[i],");
            sqlBuilder.AppendLine("            \"TVGName\" = p_tvg_names[i],");  // Set TVGName column
            sqlBuilder.AppendLine("            \"NeedsDelete\" = false -- Unmark this stream");
            sqlBuilder.AppendLine("        WHERE \"Id\" = p_ids[i];");
            sqlBuilder.AppendLine();
            sqlBuilder.AppendLine("        -- If no rows were updated, perform an insert");
            sqlBuilder.AppendLine("        IF NOT FOUND THEN");
            sqlBuilder.AppendLine("            INSERT INTO \"SMStreams\" (");
            sqlBuilder.AppendLine("                \"Id\",");
            sqlBuilder.AppendLine("                \"FilePosition\",");
            sqlBuilder.AppendLine("                \"IsHidden\",");
            sqlBuilder.AppendLine("                \"IsUserCreated\",");
            sqlBuilder.AppendLine("                \"M3UFileId\",");
            sqlBuilder.AppendLine("                \"ChannelNumber\",");
            sqlBuilder.AppendLine("                \"M3UFileName\",");
            sqlBuilder.AppendLine("                \"Group\",");
            sqlBuilder.AppendLine("                \"EPGID\",");
            sqlBuilder.AppendLine("                \"Logo\",");
            sqlBuilder.AppendLine("                \"Name\",");
            sqlBuilder.AppendLine("                \"Url\",");
            sqlBuilder.AppendLine("                \"StationId\",");
            sqlBuilder.AppendLine("                \"ChannelId\",");
            sqlBuilder.AppendLine("                \"ChannelName\",");
            sqlBuilder.AppendLine("                \"TVGName\",");  // Insert TVGName value
            sqlBuilder.AppendLine("                \"IsSystem\",");
            sqlBuilder.AppendLine("                \"CUID\",");
            sqlBuilder.AppendLine("                \"SMStreamType\",");
            sqlBuilder.AppendLine("                \"NeedsDelete\"");
            sqlBuilder.AppendLine("            ) VALUES (");
            sqlBuilder.AppendLine("                p_ids[i],");
            sqlBuilder.AppendLine("                p_file_positions[i],");
            sqlBuilder.AppendLine("                p_is_hidden[i],");
            sqlBuilder.AppendLine("                false,");  // IsUserCreated
            sqlBuilder.AppendLine("                p_m3u_file_id,");
            sqlBuilder.AppendLine("                p_channel_numbers[i],");
            sqlBuilder.AppendLine("                p_m3u_file_name,");
            sqlBuilder.AppendLine("                p_groups[i],");
            sqlBuilder.AppendLine("                p_epgids[i],");
            sqlBuilder.AppendLine("                p_logos[i],");
            sqlBuilder.AppendLine("                p_names[i],");
            sqlBuilder.AppendLine("                p_urls[i],");
            sqlBuilder.AppendLine("                p_station_ids[i],");
            sqlBuilder.AppendLine("                p_channel_ids[i],");
            sqlBuilder.AppendLine("                p_channel_names[i],");
            sqlBuilder.AppendLine("                p_tvg_names[i],");  // Insert TVGName value
            sqlBuilder.AppendLine("                false,");  // IsSystem
            sqlBuilder.AppendLine("                '',");  // CUID
            sqlBuilder.AppendLine("                0,");  // SMStreamType
            sqlBuilder.AppendLine("                false");  // NeedsDelete
            sqlBuilder.AppendLine("            );");
            sqlBuilder.AppendLine("        END IF;");
            sqlBuilder.AppendLine();

            // Optional: Handle SMChannels creation with TVGName
            sqlBuilder.AppendLine("        -- Optional: Create a channel if p_create_channels is true and the channel does not already exist");
            sqlBuilder.AppendLine("        IF p_create_channels THEN");
            sqlBuilder.AppendLine("            SELECT EXISTS (");
            sqlBuilder.AppendLine("                SELECT 1 FROM \"SMChannels\" WHERE \"BaseStreamID\" = p_ids[i]");
            sqlBuilder.AppendLine("            ) INTO channel_exists;");
            sqlBuilder.AppendLine();
            sqlBuilder.AppendLine("            IF NOT channel_exists THEN");
            sqlBuilder.AppendLine("                INSERT INTO \"SMChannels\" (");
            sqlBuilder.AppendLine("                    \"CommandProfileName\",");
            sqlBuilder.AppendLine("                    \"IsHidden\",");
            sqlBuilder.AppendLine("                    \"BaseStreamID\",");
            sqlBuilder.AppendLine("                    \"M3UFileId\",");
            sqlBuilder.AppendLine("                    \"ChannelNumber\",");
            sqlBuilder.AppendLine("                    \"TimeShift\",");
            sqlBuilder.AppendLine("                    \"Group\",");
            sqlBuilder.AppendLine("                    \"EPGId\",");
            sqlBuilder.AppendLine("                    \"Logo\",");
            sqlBuilder.AppendLine("                    \"Name\",");
            sqlBuilder.AppendLine("                    \"StationId\",");
            sqlBuilder.AppendLine("                    \"ChannelId\",");
            sqlBuilder.AppendLine("                    \"ChannelName\",");
            sqlBuilder.AppendLine("                    \"TVGName\",");  // Insert TVGName value for SMChannels
            sqlBuilder.AppendLine("                    \"GroupTitle\",");
            sqlBuilder.AppendLine("                    \"IsSystem\",");
            sqlBuilder.AppendLine("                    \"SMChannelType\"");
            sqlBuilder.AppendLine("                ) VALUES (");
            sqlBuilder.AppendLine("                    'Default',");
            sqlBuilder.AppendLine("                    p_is_hidden[i],");
            sqlBuilder.AppendLine("                    p_ids[i],");
            sqlBuilder.AppendLine("                    p_m3u_file_id,");
            sqlBuilder.AppendLine("                    p_channel_numbers[i],");
            sqlBuilder.AppendLine("                    0,");  // TimeShift
            sqlBuilder.AppendLine("                    p_groups[i],");
            sqlBuilder.AppendLine("                    p_epgids[i],");
            sqlBuilder.AppendLine("                    p_logos[i],");
            sqlBuilder.AppendLine("                    p_names[i],");
            sqlBuilder.AppendLine("                    p_station_ids[i],");
            sqlBuilder.AppendLine("                    p_channel_ids[i],");
            sqlBuilder.AppendLine("                    p_channel_names[i],");
            sqlBuilder.AppendLine("                    p_tvg_names[i],");  // Insert TVGName value for SMChannels
            sqlBuilder.AppendLine("                    '',");  // GroupTitle
            sqlBuilder.AppendLine("                    false,");  // IsSystem
            sqlBuilder.AppendLine("                    0");  // SMChannelType
            sqlBuilder.AppendLine("                ) RETURNING \"Id\" INTO channel_id;");
            sqlBuilder.AppendLine();
            sqlBuilder.AppendLine("                INSERT INTO \"SMChannelStreamLinks\" (");
            sqlBuilder.AppendLine("                    \"SMChannelId\",");
            sqlBuilder.AppendLine("                    \"SMStreamId\",");
            sqlBuilder.AppendLine("                    \"Rank\"");
            sqlBuilder.AppendLine("                ) VALUES (");
            sqlBuilder.AppendLine("                    channel_id,");
            sqlBuilder.AppendLine("                    p_ids[i],");
            sqlBuilder.AppendLine("                    0");
            sqlBuilder.AppendLine("                ) ON CONFLICT (\"SMChannelId\", \"SMStreamId\") DO NOTHING;");
            sqlBuilder.AppendLine();
            sqlBuilder.AppendLine("                IF p_stream_group_id != 0 THEN");
            sqlBuilder.AppendLine("                    INSERT INTO \"StreamGroupSMChannelLink\" (");
            sqlBuilder.AppendLine("                        \"SMChannelId\",");
            sqlBuilder.AppendLine("                        \"StreamGroupId\",");
            sqlBuilder.AppendLine("                        \"IsReadOnly\",");
            sqlBuilder.AppendLine("                        \"Rank\"");
            sqlBuilder.AppendLine("                    ) VALUES (");
            sqlBuilder.AppendLine("                        channel_id,");
            sqlBuilder.AppendLine("                        p_stream_group_id,");
            sqlBuilder.AppendLine("                        false,");
            sqlBuilder.AppendLine("                        0");
            sqlBuilder.AppendLine("                    ) ON CONFLICT (\"SMChannelId\", \"StreamGroupId\") DO NOTHING;");
            sqlBuilder.AppendLine("                END IF;");
            sqlBuilder.AppendLine();
            sqlBuilder.AppendLine("                RETURN NEXT;");
            sqlBuilder.AppendLine("            END IF;");
            sqlBuilder.AppendLine("        END IF;");
            sqlBuilder.AppendLine("    END LOOP;");
            sqlBuilder.AppendLine("END $$;");

            string sql = sqlBuilder.ToString();
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