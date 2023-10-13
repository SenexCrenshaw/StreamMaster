using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StreamMasterInfrastructureEF.Migrations.Repository
{
    /// <inheritdoc />
    public partial class AddTimeShift : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TimeShift",
                table: "VideoStreams",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TimeShift",
                table: "VideoStreams");
        }
    }
}
