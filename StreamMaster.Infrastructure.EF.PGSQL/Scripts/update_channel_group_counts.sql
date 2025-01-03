CREATE OR REPLACE FUNCTION update_channel_group_counts()
RETURNS void AS $$
BEGIN
    UPDATE "ChannelGroups" AS cg
    SET 
        "TotalCount" = sq."TotalCount",
        "HiddenCount" = sq."HiddenCount",
        "ActiveCount" = sq."ActiveCount"
    FROM (
        SELECT 
            sms."Group" AS "ChannelGroupName",
            COUNT(sms."Id") AS "TotalCount",
            COUNT(CASE WHEN sms."IsHidden" THEN 1 END) AS "HiddenCount",
            COUNT(CASE WHEN NOT sms."IsHidden" THEN 1 END) AS "ActiveCount"
        FROM "SMStreams" sms
        GROUP BY sms."Group"
    ) AS sq
    WHERE cg."Name" = sq."ChannelGroupName";
END;
$$ LANGUAGE plpgsql;
