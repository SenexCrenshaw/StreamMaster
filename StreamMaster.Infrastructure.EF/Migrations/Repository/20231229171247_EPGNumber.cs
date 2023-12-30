using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StreamMaster.Infrastructure.EF.Migrations.Repository
{
    /// <inheritdoc />
    public partial class EPGNumber : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            try
            {
                var dropColumnScript = @"
        IF EXISTS(SELECT * FROM INFORMATION_SCHEMA.COLUMNS 
                  WHERE TABLE_NAME = 'EPGFiles' AND COLUMN_NAME = 'EPGRank')
        BEGIN
            ALTER TABLE EPGFiles DROP COLUMN ""EPGRank"";
        END";
                migrationBuilder.Sql(dropColumnScript);

                var addColumnScript = @"
        IF NOT EXISTS(SELECT * FROM INFORMATION_SCHEMA.COLUMNS 
                  WHERE TABLE_NAME = 'EPGFiles' AND COLUMN_NAME = 'EPGNumber')
        BEGIN
            ALTER TABLE EPGFiles ADD COLUMN ""EPGNumber"" INTEGER NOT NULL DEFAULT 0;
        END";
                migrationBuilder.Sql(addColumnScript);

                //migrationBuilder.AddColumn<int>(
                //    name: "EPGNumber",
                //    table: "EPGFiles",
                //    type: "INTEGER",
                //    nullable: false,
                //    defaultValue: 0
                //);
                //migrationBuilder.RenameColumn(
                //    name: "EPGRank",
                //    table: "EPGFiles",
                //    newName: "EPGNumber");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            migrationBuilder.Sql("UPDATE EPGFiles SET EPGNumber = Id where EPGNumber='0'");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            try
            {
                migrationBuilder.RenameColumn(
                name: "EPGNumber",
                table: "EPGFiles",
                newName: "EPGRank");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }

}
