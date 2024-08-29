using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StreamMaster.Infrastructure.EF.PGSQL.Migrations.Repository;

/// <inheritdoc />
public partial class AddCreateOrUpdateSmStreamsAndChannelsFunction : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        // Add the SQL to create the function
        migrationBuilder.Sql(@"
            CREATE OR REPLACE FUNCTION public.create_or_update_smstreams_and_channels(
                p_ids TEXT[],
                p_file_positions INTEGER[],
                p_channel_numbers INTEGER[],
                p_groups CITEXT[],
                p_epgids CITEXT[],
                p_logos CITEXT[],
                p_names CITEXT[],
                p_urls CITEXT[],
                p_station_ids CITEXT[],
                p_m3u_file_id INTEGER,
                p_m3u_file_name CITEXT,
                p_stream_group_id INTEGER,
                p_create_channels BOOLEAN
            )
            RETURNS TABLE(channel_id INTEGER)
            LANGUAGE plpgsql
            AS $$
            BEGIN
                FOR i IN array_lower(p_ids, 1)..array_upper(p_ids, 1)
                LOOP
                    -- Attempt to update an existing stream
                    UPDATE ""SMStreams""
                    SET
                        ""FilePosition"" = p_file_positions[i],
                        ""ChannelNumber"" = p_channel_numbers[i],
                        ""M3UFileId"" = p_m3u_file_id,
                        ""M3UFileName"" = p_m3u_file_name,
                        ""Group"" = p_groups[i],
                        ""EPGID"" = p_epgids[i],
                        ""Logo"" = p_logos[i],
                        ""Name"" = p_names[i],
                        ""Url"" = p_urls[i],
                        ""StationId"" = p_station_ids[i],
                        ""NeedsDelete"" = false -- Unmark this stream
                    WHERE ""Id"" = p_ids[i];

                    -- If no rows were updated, perform an insert
                    IF NOT FOUND THEN
                        INSERT INTO ""SMStreams"" (
                            ""Id"",
                            ""FilePosition"",
                            ""IsHidden"",
                            ""IsUserCreated"",
                            ""M3UFileId"",
                            ""ChannelNumber"",
                            ""M3UFileName"",
                            ""Group"",
                            ""EPGID"",
                            ""Logo"",
                            ""Name"",
                            ""Url"",
                            ""StationId"",
                            ""IsSystem"",
                            ""CUID"",
                            ""SMStreamType"",
                            ""NeedsDelete""
                        ) VALUES (
                            p_ids[i],
                            p_file_positions[i],
                            false,  -- IsHidden (constant value)
                            false,  -- IsUserCreated (constant value)
                            p_m3u_file_id,
                            p_channel_numbers[i],
                            p_m3u_file_name,
                            p_groups[i],
                            p_epgids[i],
                            p_logos[i],
                            p_names[i],
                            p_urls[i],
                            p_station_ids[i],
                            false,  -- IsSystem (constant value)
                            '',  -- Default or COALESCE value for CUID
                            0,  -- SMStreamType (constant value)
                            false  -- NeedsDelete (new stream, not marked for deletion)
                        );
                    END IF;

                    -- Optional: Create a channel if p_create_channels is true
                    IF p_create_channels THEN
                        -- Insert into SMChannels
                        INSERT INTO ""SMChannels"" (
                            ""CommandProfileName"",
                            ""IsHidden"",
                            ""BaseStreamID"",
                            ""M3UFileId"",
                            ""ChannelNumber"",
                            ""TimeShift"",
                            ""Group"",
                            ""EPGId"",
                            ""Logo"",
                            ""Name"",
                            ""StationId"",
                            ""GroupTitle"",
                            ""IsSystem"",
                            ""SMChannelType""
                        ) VALUES (
                            'Default',
                            false,  -- IsHidden (constant value)
                            p_ids[i],  -- Using the same Id as BaseStreamID
                            p_m3u_file_id,
                            p_channel_numbers[i],
                            0,  -- TimeShift (constant value)
                            p_groups[i],
                            p_epgids[i],
                            p_logos[i],
                            p_names[i],
                            p_station_ids[i],
                            '',  -- Default or COALESCE value for GroupTitle
                            false,  -- IsSystem (constant value)
                            0  -- SMChannelType (constant value)
                        ) RETURNING ""Id"" INTO channel_id;

                        -- Insert into StreamGroupSMChannelLink table if p_stream_group_id is not 0
                        IF p_stream_group_id != 0 THEN
                            INSERT INTO ""StreamGroupSMChannelLink"" (
                                ""SMChannelId"",
                                ""StreamGroupId"",
                                ""IsReadOnly"",
                                ""Rank""
                            ) VALUES (
                                channel_id,
                                p_stream_group_id,
                                false,  -- IsReadOnly (constant value)
                                0  -- Rank (constant value)
                            )
                            ON CONFLICT (""SMChannelId"", ""StreamGroupId"") DO NOTHING;
                        END IF;

                        -- Return the channel ID
                        RETURN NEXT;
                    END IF;
                END LOOP;
            END $$;
        ");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        // Drop the function if it exists
        migrationBuilder.Sql("DROP FUNCTION IF EXISTS public.create_or_update_smstreams_and_channels;");
    }
}
