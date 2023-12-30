using Microsoft.Data.Sqlite;
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
                migrationBuilder.DropColumn(
     name: "EPGRank",
     table: "EPGFiles"
 );
            }
            catch (SqliteException ex)
            {
                Console.WriteLine(ex.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            try
            {

                migrationBuilder.AddColumn<int>(
                    name: "EPGNumber",
                    table: "EPGFiles",
                    type: "INTEGER",
                    nullable: false,
                    defaultValue: 0
                );

            }
            catch (SqliteException ex)
            {
                Console.WriteLine(ex.Message);
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
