CREATE OR REPLACE FUNCTION idx_StreamGroup_indexes() RETURNS VOID AS $$
BEGIN
    CREATE INDEX IF NOT EXISTS idx_streamgroups_name_id ON "StreamGroup" ("Name", "Id");
END;
$$ LANGUAGE plpgsql;
