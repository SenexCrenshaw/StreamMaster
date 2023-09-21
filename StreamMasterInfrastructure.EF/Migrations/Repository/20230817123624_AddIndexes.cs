using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StreamMasterInfrastructureEF.Migrations.Repository
{
    /// <inheritdoc />
    public partial class AddIndexes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "idx_User_Tvg_group",
                table: "VideoStreams",
                column: "User_Tvg_group");

            migrationBuilder.CreateIndex(
                name: "idx_User_Tvg_group_IsHidden",
                table: "VideoStreams",
                columns: new[] { "User_Tvg_group", "IsHidden" });

            migrationBuilder.CreateIndex(
                name: "idx_Name",
                table: "ChannelGroups",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "idx_Name_IsHidden",
                table: "ChannelGroups",
                columns: new[] { "Name", "IsHidden" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "idx_User_Tvg_group",
                table: "VideoStreams");

            migrationBuilder.DropIndex(
                name: "idx_User_Tvg_group_IsHidden",
                table: "VideoStreams");

            migrationBuilder.DropIndex(
                name: "idx_Name",
                table: "ChannelGroups");

            migrationBuilder.DropIndex(
                name: "idx_Name_IsHidden",
                table: "ChannelGroups");
        }
    }
}
