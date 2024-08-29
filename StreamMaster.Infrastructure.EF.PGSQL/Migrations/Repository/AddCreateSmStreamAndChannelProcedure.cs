using Microsoft.EntityFrameworkCore.Migrations;

namespace StreamMaster.Infrastructure.EF.PGSQL.Migrations.Repository;

public partial class AddCreateSmStreamAndChannelProcedure : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql(@"
            CREATE OR REPLACE PROCEDURE create_smstream_and_channel(
                p_id TEXT,
                p_client_user_agent CITEXT,
                p_file_position INT,
                p_is_hidden BOOLEAN,
                p_is_user_created BOOLEAN,
                p_m3u_file_id INT,
                p_channel_number INT,
                p_m3u_file_name CITEXT,
                p_group CITEXT,
                p_epgid CITEXT,
                p_logo CITEXT,
                p_name CITEXT,
                p_url CITEXT,
                p_station_id CITEXT,
                p_is_system BOOLEAN,
                p_cuid CITEXT,
                p_smstream_type INT,
                p_create_channel BOOLEAN
            )
            LANGUAGE plpgsql
            AS $$
            BEGIN
                -- Insert into SMStreams table
                INSERT INTO ""SMStreams"" (
                    ""Id"",
                    ""ClientUserAgent"",
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
                    ""SMStreamType""
                ) VALUES (
                    p_id,
                    p_client_user_agent,
                    p_file_position,
                    p_is_hidden,
                    p_is_user_created,
                    p_m3u_file_id,
                    p_channel_number,
                    p_m3u_file_name,
                    p_group,
                    p_epgid,
                    p_logo,
                    p_name,
                    p_url,
                    p_station_id,
                    p_is_system,
                    p_cuid,
                    p_smstream_type
                );

                -- Create a channel if p_create_channel is true
                IF p_create_channel THEN
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
                        ""ClientUserAgent"",
                        ""StationId"",
                        ""GroupTitle"",
                        ""IsSystem"",
                        ""SMChannelType""
                    ) VALUES (
                        'Default',
                        p_is_hidden,
                        p_id,
                        p_m3u_file_id,
                        p_channel_number,
                        0,
                        p_group,
                        p_epgid,
                        p_logo,
                        p_name,
                        p_client_user_agent,
                        p_station_id,
                        '',
                        p_is_system,
                        p_smstream_type
                    );
                END IF;
            END;
            $$;
        ");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql("DROP PROCEDURE IF EXISTS create_smstream_and_channel;");
    }
}
