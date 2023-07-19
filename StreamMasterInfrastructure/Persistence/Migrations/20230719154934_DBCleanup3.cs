using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StreamMasterInfrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class DBCleanup3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_VideoStreamChildVideoStream_VideoStreams_ParentRelationshipsId",
                table: "VideoStreamChildVideoStream");

            migrationBuilder.RenameColumn(
                name: "ParentRelationshipsId",
                table: "VideoStreamChildVideoStream",
                newName: "ParentVideoStreamsId");

            migrationBuilder.RenameIndex(
                name: "IX_VideoStreamChildVideoStream_ParentRelationshipsId",
                table: "VideoStreamChildVideoStream",
                newName: "IX_VideoStreamChildVideoStream_ParentVideoStreamsId");

            migrationBuilder.AddForeignKey(
                name: "FK_VideoStreamChildVideoStream_VideoStreams_ParentVideoStreamsId",
                table: "VideoStreamChildVideoStream",
                column: "ParentVideoStreamsId",
                principalTable: "VideoStreams",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_VideoStreamChildVideoStream_VideoStreams_ParentVideoStreamsId",
                table: "VideoStreamChildVideoStream");

            migrationBuilder.RenameColumn(
                name: "ParentVideoStreamsId",
                table: "VideoStreamChildVideoStream",
                newName: "ParentRelationshipsId");

            migrationBuilder.RenameIndex(
                name: "IX_VideoStreamChildVideoStream_ParentVideoStreamsId",
                table: "VideoStreamChildVideoStream",
                newName: "IX_VideoStreamChildVideoStream_ParentRelationshipsId");

            migrationBuilder.AddForeignKey(
                name: "FK_VideoStreamChildVideoStream_VideoStreams_ParentRelationshipsId",
                table: "VideoStreamChildVideoStream",
                column: "ParentRelationshipsId",
                principalTable: "VideoStreams",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
