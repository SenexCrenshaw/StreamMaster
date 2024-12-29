CREATE OR REPLACE FUNCTION public.create_or_update_smstreams_and_channels(
    p_ids text[],
    p_file_positions integer[],
    p_channel_numbers integer[],
    p_groups citext[],
    p_epgids citext[],
    p_logos citext[],
    p_names citext[],
    p_urls citext[],
    p_station_ids citext[],
    p_channel_ids citext[],
    p_channel_names citext[],
    p_extifs text[],
    p_is_hidden boolean[],
    p_tvg_names citext[],
    p_m3u_file_id integer,
    p_m3u_file_name citext,
    p_stream_group_id integer,
    p_create_channels boolean
)
RETURNS TABLE(channel_id integer) 
LANGUAGE 'plpgsql'
AS $$
DECLARE
    channel_row RECORD;
BEGIN
    -- Loop through unwrapped arrays (UNNEST)
    FOR channel_row IN
        SELECT *
        FROM unnest(
            p_ids, p_file_positions, p_channel_numbers, p_groups, p_epgids, 
            p_logos, p_names, p_urls, p_station_ids, p_channel_ids, 
            p_channel_names, p_extifs, p_is_hidden, p_tvg_names
        ) AS p(
            id, file_position, channel_number, "group", epgid, logo, name, url, 
            station_id, channel_id, channel_name, extinf, is_hidden, tvg_name
        )
    LOOP
        -- Update existing streams
        UPDATE "SMStreams"
        SET
            "FilePosition" = channel_row.file_position,
            "IsHidden" = channel_row.is_hidden,
            "IsUserCreated" = false,
            "M3UFileId" = p_m3u_file_id,
            "ChannelNumber" = channel_row.channel_number,
            "M3UFileName" = p_m3u_file_name,
            "Group" = channel_row."group",
            "EPGID" = channel_row.epgid,
            "Logo" = channel_row.logo,
            "Name" = channel_row.name,
            "Url" = channel_row.url,
            "StationId" = channel_row.station_id,
            "ChannelId" = channel_row.channel_id,
            "ChannelName" = channel_row.channel_name,
            "ExtInf" = channel_row.extinf,
            "TVGName" = channel_row.tvg_name,
            "IsSystem" = false,
            "CUID" = '',
            "SMStreamType" = 0,
            "NeedsDelete" = false
        WHERE "Id" = channel_row.id;

        -- Insert new streams if no rows were updated
        IF NOT FOUND THEN
            INSERT INTO "SMStreams" (
                "Id", "FilePosition", "IsHidden", "IsUserCreated", "M3UFileId",
                "ChannelNumber", "M3UFileName", "Group", "EPGID", "Logo",
                "Name", "Url", "StationId", "ChannelId", "ChannelName",
                "ExtInf", "TVGName", "IsSystem", "CUID", "SMStreamType", "NeedsDelete"
            )
            VALUES (
                channel_row.id, channel_row.file_position, channel_row.is_hidden, 
                false, p_m3u_file_id, channel_row.channel_number, p_m3u_file_name,
                channel_row."group", channel_row.epgid, channel_row.logo,
                channel_row.name, channel_row.url, channel_row.station_id, 
                channel_row.channel_id, channel_row.channel_name, channel_row.extinf,
                channel_row.tvg_name, false, '', 0, false
            );
        END IF;

        -- Optionally create channels
        IF p_create_channels THEN
            INSERT INTO "SMChannels" (
                "CommandProfileName", "BaseStreamID", "M3UFileId", "ChannelNumber",
                "TimeShift", "Group", "EPGId", "Logo", "Name",
                "StationId", "ChannelId", "ChannelName", "TVGName",
                "IsHidden", "GroupTitle", "IsSystem", "SMChannelType"
            )
            VALUES (
                'Default', channel_row.id, p_m3u_file_id, channel_row.channel_number,
                0, channel_row."group", channel_row.epgid, channel_row.logo, channel_row.name,
                channel_row.station_id, channel_row.channel_id, channel_row.channel_name, channel_row.tvg_name,
                channel_row.is_hidden, '', false, 0
            )
            ON CONFLICT ("BaseStreamID") DO NOTHING
            RETURNING "Id" INTO channel_id;

            -- Link the channel to the stream
            INSERT INTO "SMChannelStreamLinks" (
                "SMChannelId", "SMStreamId", "Rank"
            )
            VALUES (channel_id, channel_row.id, 0)
            ON CONFLICT ("SMChannelId", "SMStreamId") DO NOTHING;

            -- Add to stream group if applicable
            IF p_stream_group_id != 0 THEN
                INSERT INTO "StreamGroupSMChannelLink" (
                    "SMChannelId", "StreamGroupId", "IsReadOnly", "Rank"
                )
                VALUES (channel_id, p_stream_group_id, false, 0)
                ON CONFLICT ("SMChannelId", "StreamGroupId") DO NOTHING;
            END IF;

            RETURN NEXT;
        END IF;
    END LOOP;
END;
$$;
