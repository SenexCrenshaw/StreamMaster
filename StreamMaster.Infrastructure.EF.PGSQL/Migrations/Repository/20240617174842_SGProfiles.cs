using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StreamMaster.Infrastructure.EF.PGSQL.Migrations.Repository
{
    /// <inheritdoc />
    public partial class SGProfiles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FFMPEGProfileId",
                table: "StreamGroups");

            migrationBuilder.AddColumn<string[]>(
                name: "StreamGroupProfiles",
                table: "StreamGroups",
                type: "text[]",
                nullable: false,
                defaultValue: new string[0]);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "StreamGroupProfiles",
                table: "StreamGroups");

            migrationBuilder.AddColumn<string>(
                name: "FFMPEGProfileId",
                table: "StreamGroups",
                type: "text",
                nullable: false,
                defaultValue: "");
        }
    }
}
