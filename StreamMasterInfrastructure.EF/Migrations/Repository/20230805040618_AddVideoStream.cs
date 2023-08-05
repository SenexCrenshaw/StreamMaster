using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StreamMasterInfrastructure.EF.Migrations.Repository
{
    /// <inheritdoc />
    public partial class AddVideoStream : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "VideoStreams",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    FilePosition = table.Column<int>(type: "INTEGER", nullable: false),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsHidden = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsReadOnly = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsUserCreated = table.Column<bool>(type: "INTEGER", nullable: false),
                    M3UFileId = table.Column<int>(type: "INTEGER", nullable: false),
                    StreamProxyType = table.Column<int>(type: "INTEGER", nullable: false),
                    Tvg_chno = table.Column<int>(type: "INTEGER", nullable: false),
                    Tvg_group = table.Column<string>(type: "TEXT", nullable: false),
                    Tvg_ID = table.Column<string>(type: "TEXT", nullable: false),
                    Tvg_logo = table.Column<string>(type: "TEXT", nullable: false),
                    Tvg_name = table.Column<string>(type: "TEXT", nullable: false),
                    Url = table.Column<string>(type: "TEXT", nullable: false),
                    User_Tvg_chno = table.Column<int>(type: "INTEGER", nullable: false),
                    User_Tvg_group = table.Column<string>(type: "TEXT", nullable: false),
                    User_Tvg_ID = table.Column<string>(type: "TEXT", nullable: false),
                    User_Tvg_logo = table.Column<string>(type: "TEXT", nullable: false),
                    User_Tvg_name = table.Column<string>(type: "TEXT", nullable: false),
                    User_Url = table.Column<string>(type: "TEXT", nullable: false),
                    VideoStreamHandler = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VideoStreams", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "StreamGroupVideoStream",
                columns: table => new
                {
                    ChildVideoStreamId = table.Column<string>(type: "TEXT", nullable: false),
                    StreamGroupId = table.Column<int>(type: "INTEGER", nullable: false),
                    IsReadOnly = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StreamGroupVideoStream", x => new { x.ChildVideoStreamId, x.StreamGroupId });
                    table.ForeignKey(
                        name: "FK_StreamGroupVideoStream_VideoStreams_ChildVideoStreamId",
                        column: x => x.ChildVideoStreamId,
                        principalTable: "VideoStreams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "VideoStreamLinks",
                columns: table => new
                {
                    ChildVideoStreamId = table.Column<string>(type: "TEXT", nullable: false),
                    ParentVideoStreamId = table.Column<string>(type: "TEXT", nullable: false),
                    Rank = table.Column<int>(type: "INTEGER", nullable: false)
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
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_VideoStreamLinks_ChildVideoStreamId",
                table: "VideoStreamLinks",
                column: "ChildVideoStreamId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "StreamGroupVideoStream");

            migrationBuilder.DropTable(
                name: "VideoStreamLinks");

            migrationBuilder.DropTable(
                name: "VideoStreams");
        }
    }
}
