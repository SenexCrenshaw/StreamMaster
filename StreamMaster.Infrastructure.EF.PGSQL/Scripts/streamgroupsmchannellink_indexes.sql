CREATE OR REPLACE FUNCTION idx_StreamGroupSMChannelLink_indexes() RETURNS VOID AS $$
BEGIN
    CREATE UNIQUE INDEX IF NOT EXISTS idx_streamgroupsmchannellink_smchannelid_streamgroupid ON "StreamGroupSMChannelLink" ("SMChannelId", "StreamGroupId");
    CREATE INDEX IF NOT EXISTS IX_StreamGroupSMChannelLink_SMChannelId ON "StreamGroupSMChannelLink" ("SMChannelId");
END;
$$ LANGUAGE plpgsql;
