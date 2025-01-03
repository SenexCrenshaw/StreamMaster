CREATE OR REPLACE FUNCTION idx_EPGFile_indexes() RETURNS VOID AS $$
BEGIN
    CREATE INDEX IF NOT EXISTS idx_epgfiles_url ON "EPGFile" ("Url");
END;
$$ LANGUAGE plpgsql;
