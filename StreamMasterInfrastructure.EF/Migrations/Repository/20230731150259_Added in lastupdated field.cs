using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StreamMasterInfrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Addedinlastupdatedfield : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "LastUpdated",
                table: "M3UFiles",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "LastUpdated",
                table: "EPGFiles",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LastUpdated",
                table: "M3UFiles");

            migrationBuilder.DropColumn(
                name: "LastUpdated",
                table: "EPGFiles");
        }
    }
}
