using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StreamMaster.Infrastructure.EF.PGSQL.Migrations.Repository
{
    /// <inheritdoc />
    public partial class UserChooseM3UKey : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "M3UKey",
                table: "M3UFiles",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "M3UKey",
                table: "M3UFiles");
        }
    }
}
