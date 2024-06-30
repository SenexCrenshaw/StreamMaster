using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StreamMaster.Infrastructure.EF.PGSQL.Migrations.Repository
{
    /// <inheritdoc />
    public partial class MovedOverwriteChannelNumbers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OverwriteChannelNumbers",
                table: "M3UFiles");

            migrationBuilder.DropColumn(
                name: "StartingChannelNumber",
                table: "M3UFiles");

            migrationBuilder.RenameColumn(
                name: "AutoSetChannelNumbers",
                table: "StreamGroups",
                newName: "IgnoreExistingChannelNumbers");

            migrationBuilder.AddColumn<int>(
                name: "StartingChannelNumber",
                table: "StreamGroups",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "StartingChannelNumber",
                table: "StreamGroups");

            migrationBuilder.RenameColumn(
                name: "IgnoreExistingChannelNumbers",
                table: "StreamGroups",
                newName: "AutoSetChannelNumbers");

            migrationBuilder.AddColumn<bool>(
                name: "OverwriteChannelNumbers",
                table: "M3UFiles",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "StartingChannelNumber",
                table: "M3UFiles",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }
    }
}
