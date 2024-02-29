using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StreamMaster.Infrastructure.EF.PGSQL.Migrations.Repository
{
    /// <inheritdoc />
    public partial class TimeShiftUpdates : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(name: "TimeShift", table: "VideoStreams");
            migrationBuilder.AddColumn<int>(name: "TimeShift", table: "VideoStreams", type: "integer", nullable: false, defaultValue: 0);

            migrationBuilder.DropColumn(name: "TimeShift", table: "EPGFiles");
            migrationBuilder.AddColumn<int>(name: "TimeShift", table: "EPGFiles", type: "integer", nullable: false, defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(name: "TimeShift", table: "VideoStreams");
            migrationBuilder.AddColumn<string>(name: "TimeShift", table: "VideoStreams", type: "integer", nullable: false, defaultValue: "0");

            migrationBuilder.DropColumn(name: "TimeShift", table: "EPGFiles");
            migrationBuilder.AddColumn<string>(name: "TimeShift", table: "EPGFiles", type: "integer", nullable: false, defaultValue: "0");

        }
    }
}
