using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StreamMasterInfrastructureEF.Migrations.Repository
{
    /// <inheritdoc />
    public partial class M3UIndexes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_VideoStream_User_Tvg_chno",
                table: "VideoStreams",
                column: "User_Tvg_chno");

            migrationBuilder.CreateIndex(
                name: "IX_StreamGroup_StreamGroupNumber",
                table: "StreamGroups",
                column: "StreamGroupNumber");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_VideoStream_User_Tvg_chno",
                table: "VideoStreams");

            migrationBuilder.DropIndex(
                name: "IX_StreamGroup_StreamGroupNumber",
                table: "StreamGroups");
        }
    }
}
