using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StreamMasterInfrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class DBCleanup : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "VideoStreamRelationships");

            migrationBuilder.DropColumn(
                name: "StreamErrorCount",
                table: "VideoStreams");

            migrationBuilder.DropColumn(
                name: "StreamLastFail",
                table: "VideoStreams");

            migrationBuilder.DropColumn(
                name: "StreamLastStream",
                table: "VideoStreams");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "StreamErrorCount",
                table: "VideoStreams",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "StreamLastFail",
                table: "VideoStreams",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "StreamLastStream",
                table: "VideoStreams",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateTable(
                name: "VideoStreamRelationships",
                columns: table => new
                {
                    ParentVideoStreamId = table.Column<int>(type: "INTEGER", nullable: false),
                    ChildVideoStreamId = table.Column<int>(type: "INTEGER", nullable: false),
                    Rank = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VideoStreamRelationships", x => new { x.ParentVideoStreamId, x.ChildVideoStreamId });
                    table.ForeignKey(
                        name: "FK_VideoStreamRelationships_VideoStreams_ChildVideoStreamId",
                        column: x => x.ChildVideoStreamId,
                        principalTable: "VideoStreams",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_VideoStreamRelationships_VideoStreams_ParentVideoStreamId",
                        column: x => x.ParentVideoStreamId,
                        principalTable: "VideoStreams",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_VideoStreamRelationships_ChildVideoStreamId",
                table: "VideoStreamRelationships",
                column: "ChildVideoStreamId");
        }
    }
}
