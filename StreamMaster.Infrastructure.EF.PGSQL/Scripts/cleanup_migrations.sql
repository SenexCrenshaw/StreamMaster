DO $$
BEGIN
    DELETE FROM public."__EFMigrationsHistory"
    WHERE "MigrationId" > '20241216201039_SGSTRM';
END;
$$;