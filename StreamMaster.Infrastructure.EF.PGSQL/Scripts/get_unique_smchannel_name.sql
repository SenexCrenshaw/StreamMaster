CREATE OR REPLACE FUNCTION get_unique_smchannel_name(sm_channel_name TEXT)
RETURNS TEXT AS $$
SELECT CASE
    WHEN COUNT(*) = 0 THEN sm_channel_name
    ELSE sm_channel_name || '.' || (
        COALESCE(
            MAX(
                SPLIT_PART("Name", '.', 2)::INT
            ), 0
        ) + 1
    )
END
FROM "SMChannels"
WHERE LOWER("Name") = LOWER(sm_channel_name)
   OR LOWER("Name") LIKE LOWER(sm_channel_name) || '.%';
$$ LANGUAGE sql;
