CREATE OR REPLACE FUNCTION idx_SMChannel_indexes() RETURNS VOID AS $$
BEGIN
    CREATE UNIQUE INDEX IF NOT EXISTS idx_smchannels_id ON "SMChannel" ("Id");
    CREATE INDEX IF NOT EXISTS idx_smchannels_group ON "SMChannel" ("Group");
    CREATE INDEX IF NOT EXISTS idx_smchannels_basestreamid ON "SMChannel" ("BaseStreamID");
    CREATE INDEX IF NOT EXISTS idx_SMChannelName ON "SMChannel" ("Name");
    CREATE INDEX IF NOT EXISTS idx_smchannels_channelnumber_id ON "SMChannel" ("ChannelNumber", "Id");
END;
$$ LANGUAGE plpgsql;
