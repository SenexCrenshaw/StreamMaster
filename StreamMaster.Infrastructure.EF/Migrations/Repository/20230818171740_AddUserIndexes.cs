using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StreamMaster.Infrastructure.EF.Migrations.Repository
{
    /// <inheritdoc />
    public partial class AddUserIndexes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_VideoStream_User_Tvg_name",
                table: "VideoStreams",
                column: "User_Tvg_name");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_VideoStream_User_Tvg_name",
                table: "VideoStreams");
        }
    }
}
