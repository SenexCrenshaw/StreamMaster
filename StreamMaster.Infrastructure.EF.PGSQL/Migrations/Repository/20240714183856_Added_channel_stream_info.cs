using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StreamMaster.Infrastructure.EF.PGSQL.Migrations.Repository
{
    /// <inheritdoc />
    public partial class Added_channel_stream_info : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AutoSetChannelNumbers",
                table: "StreamGroups");

            migrationBuilder.DropColumn(
                name: "IgnoreExistingChannelNumbers",
                table: "StreamGroups");

            migrationBuilder.DropColumn(
                name: "StartingChannelNumber",
                table: "StreamGroups");

            migrationBuilder.AddColumn<int>(
                name: "M3UFileId",
                table: "SMChannels",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "StreamID",
                table: "SMChannels",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "SyncChannels",
                table: "M3UFiles",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "M3UFileId",
                table: "SMChannels");

            migrationBuilder.DropColumn(
                name: "StreamID",
                table: "SMChannels");

            migrationBuilder.DropColumn(
                name: "SyncChannels",
                table: "M3UFiles");

            migrationBuilder.AddColumn<bool>(
                name: "AutoSetChannelNumbers",
                table: "StreamGroups",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IgnoreExistingChannelNumbers",
                table: "StreamGroups",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "StartingChannelNumber",
                table: "StreamGroups",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }
    }
}
