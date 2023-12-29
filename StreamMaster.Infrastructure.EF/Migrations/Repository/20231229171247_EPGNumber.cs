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
            migrationBuilder.RenameColumn(
                name: "EPGRank",
                table: "EPGFiles",
                newName: "EPGNumber");

            migrationBuilder.Sql("UPDATE EPGFiles SET EPGNumber = Id where EPGNumber='0'");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "EPGNumber",
                table: "EPGFiles",
                newName: "EPGRank");
        }
    }
}
