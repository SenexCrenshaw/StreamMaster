using Microsoft.EntityFrameworkCore.Migrations;

namespace StreamMaster.Infrastructure.EF.PGSQL.Extenstions;

public static class MigrationBuilderExtensions
{
    public static void DropTableIfExists(this MigrationBuilder migrationBuilder, string tableName)
    {
        try
        {
            // PostgreSQL supports DROP TABLE IF EXISTS directly
            string sql = $"DROP TABLE IF EXISTS \"{tableName}\" CASCADE;";
            migrationBuilder.Sql(sql);
        }
        catch (Exception ex)
        {
            //logger.LogError(ex, "Error dropping table {TableName}.", tableName);
            throw; // Ensure the migration stops if there's an error
        }
    }
}
