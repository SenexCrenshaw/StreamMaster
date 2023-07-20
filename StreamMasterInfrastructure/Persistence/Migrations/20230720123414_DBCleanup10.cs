using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StreamMasterInfrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class DBCleanup10 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StreamGroupChannelGroups_ChannelGroups_ChannelGroupId",
                table: "StreamGroupChannelGroups");

            migrationBuilder.RenameColumn(
                name: "Rank",
                table: "StreamGroupVideoStreams",
                newName: "IsReadOnly");

            migrationBuilder.AddForeignKey(
                name: "FK_StreamGroupChannelGroups_ChannelGroups_ChannelGroupId",
                table: "StreamGroupChannelGroups",
                column: "ChannelGroupId",
                principalTable: "ChannelGroups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StreamGroupChannelGroups_ChannelGroups_ChannelGroupId",
                table: "StreamGroupChannelGroups");

            migrationBuilder.RenameColumn(
                name: "IsReadOnly",
                table: "StreamGroupVideoStreams",
                newName: "Rank");

            migrationBuilder.AddForeignKey(
                name: "FK_StreamGroupChannelGroups_ChannelGroups_ChannelGroupId",
                table: "StreamGroupChannelGroups",
                column: "ChannelGroupId",
                principalTable: "ChannelGroups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
