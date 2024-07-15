using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace StreamMaster.Infrastructure.EF.PGSQL.Migrations.Logging
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:PostgresExtension:citext", ",,");

            migrationBuilder.CreateTable(
                name: "ChannelGroup",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn),
                    IsHidden = table.Column<bool>(type: "boolean", nullable: false),
                    IsReadOnly = table.Column<bool>(type: "boolean", nullable: false),
                    Name = table.Column<string>(type: "citext", nullable: false),
                    RegexMatch = table.Column<string>(type: "citext", nullable: false)
                },
                constraints: table => table.PrimaryKey("PK_ChannelGroup", x => x.Id));

            migrationBuilder.CreateTable(
                name: "LogEntries",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn),
                    LogLevel = table.Column<int>(type: "integer", nullable: false),
                    Message = table.Column<string>(type: "citext", nullable: false),
                    TimeStamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LogEntries", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "StreamGroup",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn),
                    IsReadOnly = table.Column<bool>(type: "boolean", nullable: false),
                    AutoSetChannelNumbers = table.Column<bool>(type: "boolean", nullable: false),
                    Name = table.Column<string>(type: "citext", nullable: false)
                },
                constraints: table => table.PrimaryKey("PK_StreamGroup", x => x.Id));

            migrationBuilder.CreateTable(
                name: "VideoStream",
                columns: table => new
                {
                    Id = table.Column<string>(type: "citext", nullable: false),
                    StreamingProxyType = table.Column<int>(type: "integer", nullable: false),
                    FilePosition = table.Column<int>(type: "integer", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    IsHidden = table.Column<bool>(type: "boolean", nullable: false),
                    IsReadOnly = table.Column<bool>(type: "boolean", nullable: false),
                    IsUserCreated = table.Column<bool>(type: "boolean", nullable: false),
                    M3UFileId = table.Column<int>(type: "integer", nullable: false),
                    StreamProxyType = table.Column<int>(type: "integer", nullable: false),
                    M3UFileName = table.Column<string>(type: "citext", nullable: false),
                    Tvg_chno = table.Column<int>(type: "integer", nullable: false),
                    SMChannelId = table.Column<string>(type: "citext", nullable: false),
                    TimeShift = table.Column<string>(type: "text", nullable: false),
                    Tvg_group = table.Column<string>(type: "citext", nullable: false),
                    Tvg_ID = table.Column<string>(type: "citext", nullable: false),
                    Tvg_logo = table.Column<string>(type: "citext", nullable: false),
                    Tvg_name = table.Column<string>(type: "citext", nullable: false),
                    StationId = table.Column<string>(type: "citext", nullable: false),
                    Url = table.Column<string>(type: "citext", nullable: false),
                    GroupTitle = table.Column<string>(type: "citext", nullable: false),
                    User_Tvg_chno = table.Column<int>(type: "integer", nullable: false),
                    User_Tvg_group = table.Column<string>(type: "citext", nullable: false),
                    User_Tvg_ID = table.Column<string>(type: "citext", nullable: false),
                    User_Tvg_logo = table.Column<string>(type: "citext", nullable: false),
                    User_Tvg_name = table.Column<string>(type: "citext", nullable: false),
                    User_Url = table.Column<string>(type: "citext", nullable: false),
                    VideoStreamHandler = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VideoStream", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "StreamGroupChannelGroup",
                columns: table => new
                {
                    ChannelGroupId = table.Column<int>(type: "integer", nullable: false),
                    StreamGroupId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StreamGroupChannelGroup", x => new { x.ChannelGroupId, x.StreamGroupId });
                    table.ForeignKey(
                        name: "FK_StreamGroupChannelGroup_ChannelGroup_ChannelGroupId",
                        column: x => x.ChannelGroupId,
                        principalTable: "ChannelGroup",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StreamGroupChannelGroup_StreamGroup_StreamGroupId",
                        column: x => x.StreamGroupId,
                        principalTable: "StreamGroup",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StreamGroupVideoStream",
                columns: table => new
                {
                    ChildVideoStreamId = table.Column<string>(type: "citext", nullable: false),
                    StreamGroupId = table.Column<int>(type: "integer", nullable: false),
                    IsReadOnly = table.Column<bool>(type: "boolean", nullable: false),
                    Rank = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StreamGroupVideoStream", x => new { x.ChildVideoStreamId, x.StreamGroupId });
                    table.ForeignKey(
                        name: "FK_StreamGroupVideoStream_StreamGroup_StreamGroupId",
                        column: x => x.StreamGroupId,
                        principalTable: "StreamGroup",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StreamGroupVideoStream_VideoStream_ChildVideoStreamId",
                        column: x => x.ChildVideoStreamId,
                        principalTable: "VideoStream",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "VideoStreamLink",
                columns: table => new
                {
                    ChildVideoStreamId = table.Column<string>(type: "citext", nullable: false),
                    ParentVideoStreamId = table.Column<string>(type: "citext", nullable: false),
                    Rank = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VideoStreamLink", x => new { x.ParentVideoStreamId, x.ChildVideoStreamId });
                    table.ForeignKey(
                        name: "FK_VideoStreamLink_VideoStream_ChildVideoStreamId",
                        column: x => x.ChildVideoStreamId,
                        principalTable: "VideoStream",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_VideoStreamLink_VideoStream_ParentVideoStreamId",
                        column: x => x.ParentVideoStreamId,
                        principalTable: "VideoStream",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_StreamGroupChannelGroup_StreamGroupId",
                table: "StreamGroupChannelGroup",
                column: "StreamGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_StreamGroupVideoStream_StreamGroupId",
                table: "StreamGroupVideoStream",
                column: "StreamGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_VideoStreamLink_ChildVideoStreamId",
                table: "VideoStreamLink",
                column: "ChildVideoStreamId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LogEntries");

            migrationBuilder.DropTable(
                name: "StreamGroupChannelGroup");

            migrationBuilder.DropTable(
                name: "StreamGroupVideoStream");

            migrationBuilder.DropTable(
                name: "VideoStreamLink");

            migrationBuilder.DropTable(
                name: "ChannelGroup");

            migrationBuilder.DropTable(
                name: "StreamGroup");

            migrationBuilder.DropTable(
                name: "VideoStream");
        }
    }
}
