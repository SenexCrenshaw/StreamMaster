﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StreamMasterInfrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class DBCleanup2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "VideoStreamChildVideoStream",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ChildVideoStreamId = table.Column<int>(type: "INTEGER", nullable: false),
                    ParentVideoStreamId = table.Column<int>(type: "INTEGER", nullable: false),
                    Rank = table.Column<int>(type: "INTEGER", nullable: false),
                    ParentRelationshipsId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VideoStreamChildVideoStream", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VideoStreamChildVideoStream_VideoStreams_ParentRelationshipsId",
                        column: x => x.ParentRelationshipsId,
                        principalTable: "VideoStreams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_VideoStreamChildVideoStream_ParentRelationshipsId",
                table: "VideoStreamChildVideoStream",
                column: "ParentRelationshipsId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "VideoStreamChildVideoStream");
        }
    }
}
