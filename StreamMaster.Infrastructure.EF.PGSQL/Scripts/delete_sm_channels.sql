CREATE OR REPLACE FUNCTION delete_sm_channels(channel_ids INTEGER[])
RETURNS INTEGER[] AS $$
DECLARE
    deleted_ids INTEGER[];
BEGIN
    -- Delete links from SMChannelStreamLink
    DELETE FROM "SMChannelStreamLink"
    WHERE "SMChannelId" = ANY(channel_ids);

    -- Delete channels from SMChannel
    DELETE FROM "SMChannel"
    WHERE "Id" = ANY(channel_ids)
    RETURNING "Id" INTO deleted_ids;

    -- Return the list of deleted IDs
    RETURN deleted_ids;
END;
$$ LANGUAGE plpgsql;
