using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StreamMasterInfrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class DBCleanup6 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "VideoStreamsChildVideoStreams");

            migrationBuilder.CreateTable(
                name: "VideoStreamLinks",
                columns: table => new
                {
                    ParentVideoStreamId = table.Column<int>(type: "INTEGER", nullable: false),
                    ChildVideoStreamId = table.Column<int>(type: "INTEGER", nullable: false),
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
                        onDelete: ReferentialAction.Restrict);
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
                name: "VideoStreamLinks");

            migrationBuilder.CreateTable(
                name: "VideoStreamsChildVideoStreams",
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
                    table.PrimaryKey("PK_VideoStreamsChildVideoStreams", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VideoStreamsChildVideoStreams_VideoStreams_ParentVideoStreamsId",
                        column: x => x.ParentVideoStreamsId,
                        principalTable: "VideoStreams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_VideoStreamsChildVideoStreams_ParentVideoStreamsId",
                table: "VideoStreamsChildVideoStreams",
                column: "ParentVideoStreamsId");
        }
    }
}
