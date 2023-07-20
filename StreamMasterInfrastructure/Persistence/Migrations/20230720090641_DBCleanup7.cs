using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StreamMasterInfrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class DBCleanup7 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "FilePosition",
                table: "M3UFiles",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FilePosition",
                table: "M3UFiles");
        }
    }
}
