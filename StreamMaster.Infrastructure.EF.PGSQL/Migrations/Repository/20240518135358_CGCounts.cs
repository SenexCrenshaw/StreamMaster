using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StreamMaster.Infrastructure.EF.PGSQL.Migrations.Repository
{
    /// <inheritdoc />
    public partial class CGCounts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StreamGroupVideoStreams_StreamGroups_StreamGroupId",
                table: "StreamGroupVideoStreams");

            migrationBuilder.DropIndex(
                name: "IX_StreamGroupVideoStreams_StreamGroupId",
                table: "StreamGroupVideoStreams");

            migrationBuilder.AddColumn<int>(
                name: "ActiveCount",
                table: "ChannelGroups",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "HiddenCount",
                table: "ChannelGroups",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TotalCount",
                table: "ChannelGroups",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ActiveCount",
                table: "ChannelGroups");

            migrationBuilder.DropColumn(
                name: "HiddenCount",
                table: "ChannelGroups");

            migrationBuilder.DropColumn(
                name: "TotalCount",
                table: "ChannelGroups");

            migrationBuilder.CreateIndex(
                name: "IX_StreamGroupVideoStreams_StreamGroupId",
                table: "StreamGroupVideoStreams",
                column: "StreamGroupId");

            migrationBuilder.AddForeignKey(
                name: "FK_StreamGroupVideoStreams_StreamGroups_StreamGroupId",
                table: "StreamGroupVideoStreams",
                column: "StreamGroupId",
                principalTable: "StreamGroups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
