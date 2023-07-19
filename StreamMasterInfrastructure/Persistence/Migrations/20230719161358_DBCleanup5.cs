using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StreamMasterInfrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class DBCleanup5 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_VideoStreamsChildVideoStream_VideoStreams_ParentVideoStreamsId",
                table: "VideoStreamsChildVideoStream");

            migrationBuilder.DropPrimaryKey(
                name: "PK_VideoStreamsChildVideoStream",
                table: "VideoStreamsChildVideoStream");

            migrationBuilder.RenameTable(
                name: "VideoStreamsChildVideoStream",
                newName: "VideoStreamsChildVideoStreams");

            migrationBuilder.RenameIndex(
                name: "IX_VideoStreamsChildVideoStream_ParentVideoStreamsId",
                table: "VideoStreamsChildVideoStreams",
                newName: "IX_VideoStreamsChildVideoStreams_ParentVideoStreamsId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_VideoStreamsChildVideoStreams",
                table: "VideoStreamsChildVideoStreams",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_VideoStreamsChildVideoStreams_VideoStreams_ParentVideoStreamsId",
                table: "VideoStreamsChildVideoStreams",
                column: "ParentVideoStreamsId",
                principalTable: "VideoStreams",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_VideoStreamsChildVideoStreams_VideoStreams_ParentVideoStreamsId",
                table: "VideoStreamsChildVideoStreams");

            migrationBuilder.DropPrimaryKey(
                name: "PK_VideoStreamsChildVideoStreams",
                table: "VideoStreamsChildVideoStreams");

            migrationBuilder.RenameTable(
                name: "VideoStreamsChildVideoStreams",
                newName: "VideoStreamsChildVideoStream");

            migrationBuilder.RenameIndex(
                name: "IX_VideoStreamsChildVideoStreams_ParentVideoStreamsId",
                table: "VideoStreamsChildVideoStream",
                newName: "IX_VideoStreamsChildVideoStream_ParentVideoStreamsId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_VideoStreamsChildVideoStream",
                table: "VideoStreamsChildVideoStream",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_VideoStreamsChildVideoStream_VideoStreams_ParentVideoStreamsId",
                table: "VideoStreamsChildVideoStream",
                column: "ParentVideoStreamsId",
                principalTable: "VideoStreams",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
