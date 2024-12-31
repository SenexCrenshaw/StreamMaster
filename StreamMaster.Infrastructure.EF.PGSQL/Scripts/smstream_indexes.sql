CREATE OR REPLACE FUNCTION idx_SMStream_indexes() RETURNS VOID AS $$
BEGIN
    CREATE INDEX IF NOT EXISTS idx_smstreams_name ON "SMStream" ("Name");
    CREATE UNIQUE INDEX IF NOT EXISTS idx_smstreams_id ON "SMStream" ("Id");
    CREATE INDEX IF NOT EXISTS idx_SMStreamName ON "SMStream" ("Name");
    CREATE INDEX IF NOT EXISTS idx_smstreams_group ON "SMStream" ("Group");
    CREATE INDEX IF NOT EXISTS idx_smstreams_group_ishidden ON "SMStream" ("Group", "IsHidden");
    CREATE INDEX IF NOT EXISTS idx_smstreams_m3ufileid ON "SMStream" ("M3UFileId");
    CREATE INDEX IF NOT EXISTS idx_smstreams_needsdelete_m3ufileid ON "SMStream" ("NeedsDelete", "M3UFileId");
END;
$$ LANGUAGE plpgsql;
