using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StreamMaster.Infrastructure.EF.PGSQL.Migrations.Repository
{
    /// <inheritdoc />
    public partial class SG : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StreamGroupSMChannels_SMChannels_SMChannelId",
                table: "StreamGroupSMChannels");

            migrationBuilder.DropForeignKey(
                name: "FK_StreamGroupSMChannels_StreamGroups_StreamGroupId",
                table: "StreamGroupSMChannels");

            migrationBuilder.DropPrimaryKey(
                name: "PK_StreamGroupSMChannels",
                table: "StreamGroupSMChannels");

            migrationBuilder.RenameTable(
                name: "StreamGroupSMChannels",
                newName: "StreamGroupSMChannelLink");

            migrationBuilder.RenameIndex(
                name: "IX_StreamGroupSMChannels_SMChannelId",
                table: "StreamGroupSMChannelLink",
                newName: "IX_StreamGroupSMChannelLink_SMChannelId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_StreamGroupSMChannelLink",
                table: "StreamGroupSMChannelLink",
                columns: new[] { "StreamGroupId", "SMChannelId" });

            migrationBuilder.AddForeignKey(
                name: "FK_StreamGroupSMChannelLink_SMChannels_SMChannelId",
                table: "StreamGroupSMChannelLink",
                column: "SMChannelId",
                principalTable: "SMChannels",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_StreamGroupSMChannelLink_StreamGroups_StreamGroupId",
                table: "StreamGroupSMChannelLink",
                column: "StreamGroupId",
                principalTable: "StreamGroups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StreamGroupSMChannelLink_SMChannels_SMChannelId",
                table: "StreamGroupSMChannelLink");

            migrationBuilder.DropForeignKey(
                name: "FK_StreamGroupSMChannelLink_StreamGroups_StreamGroupId",
                table: "StreamGroupSMChannelLink");

            migrationBuilder.DropPrimaryKey(
                name: "PK_StreamGroupSMChannelLink",
                table: "StreamGroupSMChannelLink");

            migrationBuilder.RenameTable(
                name: "StreamGroupSMChannelLink",
                newName: "StreamGroupSMChannels");

            migrationBuilder.RenameIndex(
                name: "IX_StreamGroupSMChannelLink_SMChannelId",
                table: "StreamGroupSMChannels",
                newName: "IX_StreamGroupSMChannels_SMChannelId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_StreamGroupSMChannels",
                table: "StreamGroupSMChannels",
                columns: new[] { "StreamGroupId", "SMChannelId" });

            migrationBuilder.AddForeignKey(
                name: "FK_StreamGroupSMChannels_SMChannels_SMChannelId",
                table: "StreamGroupSMChannels",
                column: "SMChannelId",
                principalTable: "SMChannels",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_StreamGroupSMChannels_StreamGroups_StreamGroupId",
                table: "StreamGroupSMChannels",
                column: "StreamGroupId",
                principalTable: "StreamGroups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
