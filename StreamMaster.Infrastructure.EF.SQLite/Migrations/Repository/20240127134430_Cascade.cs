using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StreamMaster.Infrastructure.EF.Migrations.Repository
{
    /// <inheritdoc />
    public partial class Cascade : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StreamGroupChannelGroups_StreamGroups_StreamGroupId",
                table: "StreamGroupChannelGroups");

            migrationBuilder.DropForeignKey(
                name: "FK_StreamGroupVideoStreams_VideoStreams_ChildVideoStreamId",
                table: "StreamGroupVideoStreams");

            migrationBuilder.DropForeignKey(
                name: "FK_VideoStreamLinks_VideoStreams_ParentVideoStreamId",
                table: "VideoStreamLinks");

            migrationBuilder.AddForeignKey(
                name: "FK_StreamGroupChannelGroups_StreamGroups_StreamGroupId",
                table: "StreamGroupChannelGroups",
                column: "StreamGroupId",
                principalTable: "StreamGroups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_StreamGroupVideoStreams_VideoStreams_ChildVideoStreamId",
                table: "StreamGroupVideoStreams",
                column: "ChildVideoStreamId",
                principalTable: "VideoStreams",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_VideoStreamLinks_VideoStreams_ParentVideoStreamId",
                table: "VideoStreamLinks",
                column: "ParentVideoStreamId",
                principalTable: "VideoStreams",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StreamGroupChannelGroups_StreamGroups_StreamGroupId",
                table: "StreamGroupChannelGroups");

            migrationBuilder.DropForeignKey(
                name: "FK_StreamGroupVideoStreams_VideoStreams_ChildVideoStreamId",
                table: "StreamGroupVideoStreams");

            migrationBuilder.DropForeignKey(
                name: "FK_VideoStreamLinks_VideoStreams_ParentVideoStreamId",
                table: "VideoStreamLinks");

            migrationBuilder.AddForeignKey(
                name: "FK_StreamGroupChannelGroups_StreamGroups_StreamGroupId",
                table: "StreamGroupChannelGroups",
                column: "StreamGroupId",
                principalTable: "StreamGroups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_StreamGroupVideoStreams_VideoStreams_ChildVideoStreamId",
                table: "StreamGroupVideoStreams",
                column: "ChildVideoStreamId",
                principalTable: "VideoStreams",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_VideoStreamLinks_VideoStreams_ParentVideoStreamId",
                table: "VideoStreamLinks",
                column: "ParentVideoStreamId",
                principalTable: "VideoStreams",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
