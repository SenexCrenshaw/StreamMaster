using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StreamMaster.Infrastructure.EF.PGSQL.Migrations.Repository
{
    /// <inheritdoc />
    public partial class Changed_SMChannelLinks : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SMChannelChannelLinks_SMChannels_ChildSMChannelId",
                table: "SMChannelChannelLinks");

            migrationBuilder.RenameColumn(
                name: "ChildSMChannelId",
                table: "SMChannelChannelLinks",
                newName: "SMChannelId");

            migrationBuilder.RenameIndex(
                name: "IX_SMChannelChannelLinks_ChildSMChannelId",
                table: "SMChannelChannelLinks",
                newName: "IX_SMChannelChannelLinks_SMChannelId");

            migrationBuilder.AddForeignKey(
                name: "FK_SMChannelChannelLinks_SMChannels_SMChannelId",
                table: "SMChannelChannelLinks",
                column: "SMChannelId",
                principalTable: "SMChannels",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SMChannelChannelLinks_SMChannels_SMChannelId",
                table: "SMChannelChannelLinks");

            migrationBuilder.RenameColumn(
                name: "SMChannelId",
                table: "SMChannelChannelLinks",
                newName: "ChildSMChannelId");

            migrationBuilder.RenameIndex(
                name: "IX_SMChannelChannelLinks_SMChannelId",
                table: "SMChannelChannelLinks",
                newName: "IX_SMChannelChannelLinks_ChildSMChannelId");

            migrationBuilder.AddForeignKey(
                name: "FK_SMChannelChannelLinks_SMChannels_ChildSMChannelId",
                table: "SMChannelChannelLinks",
                column: "ChildSMChannelId",
                principalTable: "SMChannels",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
