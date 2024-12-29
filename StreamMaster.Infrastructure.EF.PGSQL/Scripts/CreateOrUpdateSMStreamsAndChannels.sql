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
) RETURNS TABLE(channel_id integer)
LANGUAGE 'plpgsql'
AS $BODY$
DECLARE
    channel_exists BOOLEAN;
    stream_exists BOOLEAN;
BEGIN
    FOR i IN array_lower(p_ids, 1)..array_upper(p_ids, 1)
    LOOP
        -- Ensure the SMStream exists
        SELECT EXISTS (
            SELECT 1 FROM "SMStreams" WHERE "Id" = p_ids[i]
        ) INTO stream_exists;

        -- If the stream does not exist, insert it
        IF NOT stream_exists THEN
            INSERT INTO "SMStreams" (
                "Id", "FilePosition", "IsHidden", "IsUserCreated", "M3UFileId",
                "ChannelNumber", "M3UFileName", "Group", "EPGID", "Logo", "Name",
                "Url", "StationId", "ChannelId", "ChannelName", "ExtInf", "TVGName",
                "IsSystem", "CUID", "SMStreamType", "NeedsDelete"
            ) VALUES (
                p_ids[i], p_file_positions[i], p_is_hidden[i], false, p_m3u_file_id,
                p_channel_numbers[i], p_m3u_file_name, p_groups[i], p_epgids[i], p_logos[i],
                p_names[i], p_urls[i], p_station_ids[i], p_channel_ids[i], p_channel_names[i],
                p_extifs[i], p_tvg_names[i], false, '', 0, false
            );
        END IF;

        -- Check if at least one channel exists for the BaseStreamID
        SELECT EXISTS (
            SELECT 1 FROM "SMChannels" WHERE "BaseStreamID" = p_ids[i]
        ) INTO channel_exists;

        -- If no channel exists, create one
        IF NOT channel_exists THEN
            INSERT INTO "SMChannels" (
                "CommandProfileName", "BaseStreamID", "M3UFileId", "ChannelNumber",
                "TimeShift", "Group", "EPGId", "Logo", "Name", "StationId",
                "ChannelId", "ChannelName", "TVGName", "IsHidden", "GroupTitle",
                "IsSystem", "SMChannelType"
            ) VALUES (
                'Default', p_ids[i], p_m3u_file_id, p_channel_numbers[i], 0,
                p_groups[i], p_epgids[i], p_logos[i], p_names[i],
                p_station_ids[i], p_channel_ids[i], p_channel_names[i],
                p_tvg_names[i], p_is_hidden[i], '', false, 0
            ) RETURNING "Id" INTO channel_id;

            -- Link the new channel to the stream
            INSERT INTO "SMChannelStreamLinks" (
                "SMChannelId", "SMStreamId", "Rank"
            ) VALUES (
                channel_id, p_ids[i], 0
            ) ON CONFLICT ("SMChannelId", "SMStreamId") DO NOTHING;

            -- Link the channel to the stream group, if applicable
            IF p_stream_group_id != 0 THEN
                INSERT INTO "StreamGroupSMChannelLink" (
                    "SMChannelId", "StreamGroupId", "IsReadOnly", "Rank"
                ) VALUES (
                    channel_id, p_stream_group_id, false, 0
                ) ON CONFLICT ("SMChannelId", "StreamGroupId") DO NOTHING;
            END IF;

            RETURN NEXT;
        END IF;
    END LOOP;
END;
$BODY$;
