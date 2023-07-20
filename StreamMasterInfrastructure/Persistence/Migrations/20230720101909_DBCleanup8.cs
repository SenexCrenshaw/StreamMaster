using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StreamMasterInfrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class DBCleanup8 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StreamGroupChannelGroup_ChannelGroups_ChannelGroupId",
                table: "StreamGroupChannelGroup");

            migrationBuilder.DropForeignKey(
                name: "FK_StreamGroupChannelGroup_StreamGroups_StreamGroupId",
                table: "StreamGroupChannelGroup");

            migrationBuilder.DropForeignKey(
                name: "FK_StreamGroupVideoStream_StreamGroups_StreamGroupId",
                table: "StreamGroupVideoStream");

            migrationBuilder.DropForeignKey(
                name: "FK_StreamGroupVideoStream_VideoStreams_VideoStreamId",
                table: "StreamGroupVideoStream");

            migrationBuilder.DropForeignKey(
                name: "FK_VideoStreamLinks_VideoStreams_ChildVideoStreamId",
                table: "VideoStreamLinks");

            migrationBuilder.DropPrimaryKey(
                name: "PK_StreamGroupVideoStream",
                table: "StreamGroupVideoStream");

            migrationBuilder.DropIndex(
                name: "IX_StreamGroupVideoStream_VideoStreamId",
                table: "StreamGroupVideoStream");

            migrationBuilder.DropPrimaryKey(
                name: "PK_StreamGroupChannelGroup",
                table: "StreamGroupChannelGroup");

            migrationBuilder.DropIndex(
                name: "IX_StreamGroupChannelGroup_ChannelGroupId",
                table: "StreamGroupChannelGroup");

            migrationBuilder.DropColumn(
                name: "FilePosition",
                table: "M3UFiles");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "StreamGroupVideoStream");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "StreamGroupChannelGroup");

            migrationBuilder.RenameTable(
                name: "StreamGroupVideoStream",
                newName: "StreamGroupVideoStreams");

            migrationBuilder.RenameTable(
                name: "StreamGroupChannelGroup",
                newName: "StreamGroupChannelGroups");

            migrationBuilder.RenameColumn(
                name: "VideoStreamId",
                table: "StreamGroupVideoStreams",
                newName: "ChildVideoStreamId");

            migrationBuilder.RenameIndex(
                name: "IX_StreamGroupVideoStream_StreamGroupId",
                table: "StreamGroupVideoStreams",
                newName: "IX_StreamGroupVideoStreams_StreamGroupId");

            migrationBuilder.RenameIndex(
                name: "IX_StreamGroupChannelGroup_StreamGroupId",
                table: "StreamGroupChannelGroups",
                newName: "IX_StreamGroupChannelGroups_StreamGroupId");

            migrationBuilder.AddColumn<int>(
                name: "FilePosition",
                table: "VideoStreams",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_StreamGroupVideoStreams",
                table: "StreamGroupVideoStreams",
                columns: new[] { "ChildVideoStreamId", "StreamGroupId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_StreamGroupChannelGroups",
                table: "StreamGroupChannelGroups",
                columns: new[] { "ChannelGroupId", "StreamGroupId" });

            migrationBuilder.AddForeignKey(
                name: "FK_StreamGroupChannelGroups_ChannelGroups_ChannelGroupId",
                table: "StreamGroupChannelGroups",
                column: "ChannelGroupId",
                principalTable: "ChannelGroups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_StreamGroupChannelGroups_StreamGroups_StreamGroupId",
                table: "StreamGroupChannelGroups",
                column: "StreamGroupId",
                principalTable: "StreamGroups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_StreamGroupVideoStreams_StreamGroups_StreamGroupId",
                table: "StreamGroupVideoStreams",
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
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_VideoStreamLinks_VideoStreams_ChildVideoStreamId",
                table: "VideoStreamLinks",
                column: "ChildVideoStreamId",
                principalTable: "VideoStreams",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StreamGroupChannelGroups_ChannelGroups_ChannelGroupId",
                table: "StreamGroupChannelGroups");

            migrationBuilder.DropForeignKey(
                name: "FK_StreamGroupChannelGroups_StreamGroups_StreamGroupId",
                table: "StreamGroupChannelGroups");

            migrationBuilder.DropForeignKey(
                name: "FK_StreamGroupVideoStreams_StreamGroups_StreamGroupId",
                table: "StreamGroupVideoStreams");

            migrationBuilder.DropForeignKey(
                name: "FK_StreamGroupVideoStreams_VideoStreams_ChildVideoStreamId",
                table: "StreamGroupVideoStreams");

            migrationBuilder.DropForeignKey(
                name: "FK_VideoStreamLinks_VideoStreams_ChildVideoStreamId",
                table: "VideoStreamLinks");

            migrationBuilder.DropPrimaryKey(
                name: "PK_StreamGroupVideoStreams",
                table: "StreamGroupVideoStreams");

            migrationBuilder.DropPrimaryKey(
                name: "PK_StreamGroupChannelGroups",
                table: "StreamGroupChannelGroups");

            migrationBuilder.DropColumn(
                name: "FilePosition",
                table: "VideoStreams");

            migrationBuilder.RenameTable(
                name: "StreamGroupVideoStreams",
                newName: "StreamGroupVideoStream");

            migrationBuilder.RenameTable(
                name: "StreamGroupChannelGroups",
                newName: "StreamGroupChannelGroup");

            migrationBuilder.RenameColumn(
                name: "ChildVideoStreamId",
                table: "StreamGroupVideoStream",
                newName: "VideoStreamId");

            migrationBuilder.RenameIndex(
                name: "IX_StreamGroupVideoStreams_StreamGroupId",
                table: "StreamGroupVideoStream",
                newName: "IX_StreamGroupVideoStream_StreamGroupId");

            migrationBuilder.RenameIndex(
                name: "IX_StreamGroupChannelGroups_StreamGroupId",
                table: "StreamGroupChannelGroup",
                newName: "IX_StreamGroupChannelGroup_StreamGroupId");

            migrationBuilder.AddColumn<int>(
                name: "FilePosition",
                table: "M3UFiles",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "StreamGroupVideoStream",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0)
                .Annotation("Sqlite:Autoincrement", true);

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "StreamGroupChannelGroup",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0)
                .Annotation("Sqlite:Autoincrement", true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_StreamGroupVideoStream",
                table: "StreamGroupVideoStream",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_StreamGroupChannelGroup",
                table: "StreamGroupChannelGroup",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_StreamGroupVideoStream_VideoStreamId",
                table: "StreamGroupVideoStream",
                column: "VideoStreamId");

            migrationBuilder.CreateIndex(
                name: "IX_StreamGroupChannelGroup_ChannelGroupId",
                table: "StreamGroupChannelGroup",
                column: "ChannelGroupId");

            migrationBuilder.AddForeignKey(
                name: "FK_StreamGroupChannelGroup_ChannelGroups_ChannelGroupId",
                table: "StreamGroupChannelGroup",
                column: "ChannelGroupId",
                principalTable: "ChannelGroups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_StreamGroupChannelGroup_StreamGroups_StreamGroupId",
                table: "StreamGroupChannelGroup",
                column: "StreamGroupId",
                principalTable: "StreamGroups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_StreamGroupVideoStream_StreamGroups_StreamGroupId",
                table: "StreamGroupVideoStream",
                column: "StreamGroupId",
                principalTable: "StreamGroups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_StreamGroupVideoStream_VideoStreams_VideoStreamId",
                table: "StreamGroupVideoStream",
                column: "VideoStreamId",
                principalTable: "VideoStreams",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_VideoStreamLinks_VideoStreams_ChildVideoStreamId",
                table: "VideoStreamLinks",
                column: "ChildVideoStreamId",
                principalTable: "VideoStreams",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
