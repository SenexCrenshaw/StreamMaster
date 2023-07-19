using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StreamMasterInfrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class DBCleanup4 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "VideoStreamChildVideoStream");

            migrationBuilder.CreateTable(
                name: "VideoStreamsChildVideoStream",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ChildVideoStreamId = table.Column<int>(type: "INTEGER", nullable: false),
                    ParentVideoStreamId = table.Column<int>(type: "INTEGER", nullable: false),
                    Rank = table.Column<int>(type: "INTEGER", nullable: false),
                    ParentVideoStreamsId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VideoStreamsChildVideoStream", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VideoStreamsChildVideoStream_VideoStreams_ParentVideoStreamsId",
                        column: x => x.ParentVideoStreamsId,
                        principalTable: "VideoStreams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_VideoStreamsChildVideoStream_ParentVideoStreamsId",
                table: "VideoStreamsChildVideoStream",
                column: "ParentVideoStreamsId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "VideoStreamsChildVideoStream");

            migrationBuilder.CreateTable(
                name: "VideoStreamChildVideoStream",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ChildVideoStreamId = table.Column<int>(type: "INTEGER", nullable: false),
                    ParentVideoStreamId = table.Column<int>(type: "INTEGER", nullable: false),
                    ParentVideoStreamsId = table.Column<int>(type: "INTEGER", nullable: false),
                    Rank = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VideoStreamChildVideoStream", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VideoStreamChildVideoStream_VideoStreams_ParentVideoStreamsId",
                        column: x => x.ParentVideoStreamsId,
                        principalTable: "VideoStreams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_VideoStreamChildVideoStream_ParentVideoStreamsId",
                table: "VideoStreamChildVideoStream",
                column: "ParentVideoStreamsId");
        }
    }
}
