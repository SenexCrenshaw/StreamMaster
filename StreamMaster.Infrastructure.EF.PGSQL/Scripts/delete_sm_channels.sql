CREATE OR REPLACE FUNCTION delete_sm_channels(channel_ids INTEGER[])
RETURNS VOID AS $$
BEGIN
    -- Delete links from SMChannelStreamLink
    DELETE FROM public."SMChannelStreamLinks"
    WHERE "SMChannelId" = ANY(channel_ids);

    -- Delete channels from SMChannel
    DELETE FROM public."SMChannels"
    WHERE "Id" = ANY(channel_ids);
END;
$$ LANGUAGE plpgsql;
