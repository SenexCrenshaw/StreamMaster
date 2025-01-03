CREATE OR REPLACE FUNCTION idx_ChannelGroup_indexes() RETURNS VOID AS $$
BEGIN
    CREATE INDEX IF NOT EXISTS idx_Name ON "ChannelGroup" ("Name");
    CREATE INDEX IF NOT EXISTS idx_Name_IsHidden ON "ChannelGroup" ("Name", "IsHidden");
END;
$$ LANGUAGE plpgsql;
