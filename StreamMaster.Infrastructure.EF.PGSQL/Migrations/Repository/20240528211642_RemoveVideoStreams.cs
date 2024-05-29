using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StreamMaster.Infrastructure.EF.PGSQL.Migrations.Repository
{
    /// <inheritdoc />
    public partial class RemoveVideoStreams : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "StreamGroupVideoStreams");

            migrationBuilder.DropTable(
                name: "VideoStreamLinks");

            migrationBuilder.DropTable(
                name: "VideoStreams");

            migrationBuilder.RenameColumn(
                name: "VideoStreamId",
                table: "SMChannels",
                newName: "SMChannelId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "SMChannelId",
                table: "SMChannels",
                newName: "VideoStreamId");

            migrationBuilder.CreateTable(
                name: "VideoStreams",
                columns: table => new
                {
                    Id = table.Column<string>(type: "citext", nullable: false),
                    FilePosition = table.Column<int>(type: "integer", nullable: false),
                    GroupTitle = table.Column<string>(type: "citext", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    IsHidden = table.Column<bool>(type: "boolean", nullable: false),
                    IsReadOnly = table.Column<bool>(type: "boolean", nullable: false),
                    IsUserCreated = table.Column<bool>(type: "boolean", nullable: false),
                    M3UFileId = table.Column<int>(type: "integer", nullable: false),
                    M3UFileName = table.Column<string>(type: "citext", nullable: false),
                    SMChannelId = table.Column<string>(type: "citext", nullable: false),
                    StationId = table.Column<string>(type: "citext", nullable: false),
                    StreamProxyType = table.Column<int>(type: "integer", nullable: false),
                    StreamingProxyType = table.Column<int>(type: "integer", nullable: false),
                    TimeShift = table.Column<int>(type: "integer", nullable: false),
                    Tvg_ID = table.Column<string>(type: "citext", nullable: false),
                    Tvg_chno = table.Column<int>(type: "integer", nullable: false),
                    Tvg_group = table.Column<string>(type: "citext", nullable: false),
                    Tvg_logo = table.Column<string>(type: "citext", nullable: false),
                    Tvg_name = table.Column<string>(type: "citext", nullable: false),
                    Url = table.Column<string>(type: "citext", nullable: false),
                    User_Tvg_ID = table.Column<string>(type: "citext", nullable: false),
                    User_Tvg_chno = table.Column<int>(type: "integer", nullable: false),
                    User_Tvg_group = table.Column<string>(type: "citext", nullable: false),
                    User_Tvg_logo = table.Column<string>(type: "citext", nullable: false),
                    User_Tvg_name = table.Column<string>(type: "citext", nullable: false),
                    User_Url = table.Column<string>(type: "citext", nullable: false),
                    VideoStreamHandler = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VideoStreams", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "StreamGroupVideoStreams",
                columns: table => new
                {
                    ChildVideoStreamId = table.Column<string>(type: "citext", nullable: false),
                    StreamGroupId = table.Column<int>(type: "integer", nullable: false),
                    IsReadOnly = table.Column<bool>(type: "boolean", nullable: false),
                    Rank = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StreamGroupVideoStreams", x => new { x.ChildVideoStreamId, x.StreamGroupId });
                    table.ForeignKey(
                        name: "FK_StreamGroupVideoStreams_VideoStreams_ChildVideoStreamId",
                        column: x => x.ChildVideoStreamId,
                        principalTable: "VideoStreams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "VideoStreamLinks",
                columns: table => new
                {
                    ParentVideoStreamId = table.Column<string>(type: "citext", nullable: false),
                    ChildVideoStreamId = table.Column<string>(type: "citext", nullable: false),
                    Rank = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VideoStreamLinks", x => new { x.ParentVideoStreamId, x.ChildVideoStreamId });
                    table.ForeignKey(
                        name: "FK_VideoStreamLinks_VideoStreams_ChildVideoStreamId",
                        column: x => x.ChildVideoStreamId,
                        principalTable: "VideoStreams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_VideoStreamLinks_VideoStreams_ParentVideoStreamId",
                        column: x => x.ParentVideoStreamId,
                        principalTable: "VideoStreams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_VideoStreamLinks_ChildVideoStreamId",
                table: "VideoStreamLinks",
                column: "ChildVideoStreamId");

            migrationBuilder.CreateIndex(
                name: "idx_User_Tvg_group",
                table: "VideoStreams",
                column: "User_Tvg_group");

            migrationBuilder.CreateIndex(
                name: "idx_User_Tvg_group_IsHidden",
                table: "VideoStreams",
                columns: new[] { "User_Tvg_group", "IsHidden" });

            migrationBuilder.CreateIndex(
                name: "IX_VideoStream_SMChannelId",
                table: "VideoStreams",
                column: "SMChannelId");

            migrationBuilder.CreateIndex(
                name: "IX_VideoStream_User_Tvg_chno",
                table: "VideoStreams",
                column: "User_Tvg_chno");

            migrationBuilder.CreateIndex(
                name: "IX_VideoStream_User_Tvg_name",
                table: "VideoStreams",
                column: "User_Tvg_name");
        }
    }
}
