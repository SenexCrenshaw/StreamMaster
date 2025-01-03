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
    p_create_channels boolean,
    p_return_results boolean -- New parameter
) RETURNS TABLE(channel_id integer)
LANGUAGE 'plpgsql'
AS $BODY$
BEGIN
    -- Step 1: Upsert Streams in batch
    INSERT INTO "SMStreams" (
        "Id", "FilePosition", "IsHidden", "IsUserCreated", "M3UFileId",
        "ChannelNumber", "M3UFileName", "Group", "EPGID", "Logo", "Name",
        "Url", "StationId", "ChannelId", "ChannelName", "ExtInf", "TVGName",
        "IsSystem", "CUID", "SMStreamType", "NeedsDelete"
    )
    SELECT 
        p_ids[i], p_file_positions[i], p_is_hidden[i], false, p_m3u_file_id,
        p_channel_numbers[i], p_m3u_file_name, p_groups[i], p_epgids[i], p_logos[i],
        p_names[i], p_urls[i], p_station_ids[i], p_channel_ids[i], p_channel_names[i],
        p_extifs[i], p_tvg_names[i], false, '', 0, false
    FROM generate_subscripts(p_ids, 1) i
    ON CONFLICT ("Id") DO UPDATE
    SET
        "FilePosition" = EXCLUDED."FilePosition",
        "IsHidden" = EXCLUDED."IsHidden",
        "ChannelNumber" = EXCLUDED."ChannelNumber",
        "Group" = EXCLUDED."Group",
        "EPGID" = EXCLUDED."EPGID",
        "Logo" = EXCLUDED."Logo",
        "Name" = EXCLUDED."Name",
        "Url" = EXCLUDED."Url",
        "StationId" = EXCLUDED."StationId",
        "ChannelId" = EXCLUDED."ChannelId",
        "ChannelName" = EXCLUDED."ChannelName",
        "TVGName" = EXCLUDED."TVGName",
        "ExtInf" = EXCLUDED."ExtInf";

    -- Step 2: Conditionally Create Channels
    IF p_create_channels THEN
        -- Step 2.1: Use a temporary table to store new channel IDs
        CREATE TEMP TABLE temp_channel_ids ("Id" integer, "BaseStreamID" text) ON COMMIT DROP;

        -- Use a WITH clause to select and insert into the temporary table
        WITH inserted_channels AS (
            INSERT INTO "SMChannels" (
                "CommandProfileName", "BaseStreamID", "M3UFileId", "ChannelNumber", "TimeShift", 
                "Group", "EPGId", "Logo", "Name", "StationId", "ChannelId", "ChannelName", 
                "TVGName", "IsHidden", "GroupTitle", "IsSystem", "SMChannelType"
            )
            SELECT DISTINCT ON (s."Id")
                'Default', s."Id", p_m3u_file_id, p_channel_numbers[i], 0,
                p_groups[i], p_epgids[i], p_logos[i], p_names[i],
                p_station_ids[i], p_channel_ids[i], p_channel_names[i],
                p_tvg_names[i], p_is_hidden[i], '', false, 0
            FROM generate_subscripts(p_ids, 1) i
            INNER JOIN "SMStreams" s ON s."Id" = p_ids[i]
            LEFT JOIN "SMChannels" c ON c."BaseStreamID" = s."Id"
            WHERE c."Id" IS NULL
            RETURNING "Id", "BaseStreamID"
        )
        INSERT INTO temp_channel_ids ("Id", "BaseStreamID")
        SELECT "Id", "BaseStreamID" FROM inserted_channels;

        -- Step 3: Insert into SMChannelStreamLinks for newly created channels
        INSERT INTO public."SMChannelStreamLinks"(
            "SMChannelId", "SMStreamId", "Rank"
        )
        SELECT t."Id", t."BaseStreamID", 0
        FROM temp_channel_ids t;

        -- Step 4: Batch Link to StreamGroupSMChannelLink
        IF p_stream_group_id != 0 THEN
            INSERT INTO "StreamGroupSMChannelLink" (
                "SMChannelId", "StreamGroupId", "IsReadOnly", "Rank"
            )
            SELECT "Id", p_stream_group_id, false, 0
            FROM temp_channel_ids
            ON CONFLICT ("SMChannelId", "StreamGroupId") DO NOTHING;
        END IF;

        -- Return results if required
        IF p_return_results THEN
            RETURN QUERY SELECT "Id" FROM temp_channel_ids;
        END IF;
    END IF;

    RETURN; -- If p_return_results is FALSE, return an empty result
END;
$BODY$;
