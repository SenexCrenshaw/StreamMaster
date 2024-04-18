using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StreamMaster.Infrastructure.EF.PGSQL.Migrations.Repository
{
    /// <inheritdoc />
    public partial class SGSMChannels : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_StreamGroupSMChannels",
                table: "StreamGroupSMChannels");

            migrationBuilder.AddPrimaryKey(
                name: "PK_StreamGroupSMChannels",
                table: "StreamGroupSMChannels",
                columns: new[] { "StreamGroupId", "SMChannelId" });

            migrationBuilder.CreateIndex(
                name: "IX_StreamGroupSMChannels_SMChannelId",
                table: "StreamGroupSMChannels",
                column: "SMChannelId");

            migrationBuilder.AddForeignKey(
                name: "FK_StreamGroupSMChannels_StreamGroups_StreamGroupId",
                table: "StreamGroupSMChannels",
                column: "StreamGroupId",
                principalTable: "StreamGroups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StreamGroupSMChannels_StreamGroups_StreamGroupId",
                table: "StreamGroupSMChannels");

            migrationBuilder.DropPrimaryKey(
                name: "PK_StreamGroupSMChannels",
                table: "StreamGroupSMChannels");

            migrationBuilder.DropIndex(
                name: "IX_StreamGroupSMChannels_SMChannelId",
                table: "StreamGroupSMChannels");

            migrationBuilder.AddPrimaryKey(
                name: "PK_StreamGroupSMChannels",
                table: "StreamGroupSMChannels",
                columns: new[] { "SMChannelId", "StreamGroupId" });
        }
    }
}
