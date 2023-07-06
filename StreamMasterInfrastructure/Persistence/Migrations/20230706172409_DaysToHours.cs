using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StreamMasterInfrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class DaysToHours : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "DaysToUpdate",
                table: "M3UFiles",
                newName: "HoursToUpdate");

            migrationBuilder.RenameColumn(
                name: "DaysToUpdate",
                table: "EPGFiles",
                newName: "HoursToUpdate");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "HoursToUpdate",
                table: "M3UFiles",
                newName: "DaysToUpdate");

            migrationBuilder.RenameColumn(
                name: "HoursToUpdate",
                table: "EPGFiles",
                newName: "DaysToUpdate");
        }
    }
}
