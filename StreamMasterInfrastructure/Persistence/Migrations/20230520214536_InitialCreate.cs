using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StreamMasterInfrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ChannelGroups",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    IsHidden = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsReadOnly = table.Column<bool>(type: "INTEGER", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Rank = table.Column<int>(type: "INTEGER", nullable: false),
                    RegexMatch = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChannelGroups", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EPGFiles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ChannelCount = table.Column<int>(type: "INTEGER", nullable: false),
                    EPGRank = table.Column<int>(type: "INTEGER", nullable: false),
                    ProgrammeCount = table.Column<int>(type: "INTEGER", nullable: false),
                    TimeShift = table.Column<float>(type: "REAL", nullable: false),
                    ContentType = table.Column<string>(type: "TEXT", nullable: false),
                    DownloadErrors = table.Column<int>(type: "INTEGER", nullable: false),
                    FileExists = table.Column<bool>(type: "INTEGER", nullable: false),
                    FileExtension = table.Column<string>(type: "TEXT", nullable: false),
                    LastDownloadAttempt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    LastDownloaded = table.Column<DateTime>(type: "TEXT", nullable: false),
                    MetaData = table.Column<string>(type: "TEXT", nullable: false),
                    MinimumMinutesBetweenDownloads = table.Column<int>(type: "INTEGER", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    OriginalSource = table.Column<string>(type: "TEXT", nullable: false),
                    SMFileType = table.Column<int>(type: "INTEGER", nullable: false),
                    Source = table.Column<string>(type: "TEXT", nullable: false),
                    Url = table.Column<string>(type: "TEXT", nullable: true),
                    AutoUpdate = table.Column<bool>(type: "INTEGER", nullable: false),
                    DaysToUpdate = table.Column<int>(type: "INTEGER", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EPGFiles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Icons",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ContentType = table.Column<string>(type: "TEXT", nullable: false),
                    DownloadErrors = table.Column<int>(type: "INTEGER", nullable: false),
                    FileExists = table.Column<bool>(type: "INTEGER", nullable: false),
                    FileExtension = table.Column<string>(type: "TEXT", nullable: false),
                    LastDownloadAttempt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    LastDownloaded = table.Column<DateTime>(type: "TEXT", nullable: false),
                    MetaData = table.Column<string>(type: "TEXT", nullable: false),
                    MinimumMinutesBetweenDownloads = table.Column<int>(type: "INTEGER", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    OriginalSource = table.Column<string>(type: "TEXT", nullable: false),
                    SMFileType = table.Column<int>(type: "INTEGER", nullable: false),
                    Source = table.Column<string>(type: "TEXT", nullable: false),
                    Url = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Icons", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "M3UFiles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    MaxStreamCount = table.Column<int>(type: "INTEGER", nullable: false),
                    StationCount = table.Column<int>(type: "INTEGER", nullable: false),
                    ContentType = table.Column<string>(type: "TEXT", nullable: false),
                    DownloadErrors = table.Column<int>(type: "INTEGER", nullable: false),
                    FileExists = table.Column<bool>(type: "INTEGER", nullable: false),
                    FileExtension = table.Column<string>(type: "TEXT", nullable: false),
                    LastDownloadAttempt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    LastDownloaded = table.Column<DateTime>(type: "TEXT", nullable: false),
                    MetaData = table.Column<string>(type: "TEXT", nullable: false),
                    MinimumMinutesBetweenDownloads = table.Column<int>(type: "INTEGER", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    OriginalSource = table.Column<string>(type: "TEXT", nullable: false),
                    SMFileType = table.Column<int>(type: "INTEGER", nullable: false),
                    Source = table.Column<string>(type: "TEXT", nullable: false),
                    Url = table.Column<string>(type: "TEXT", nullable: true),
                    AutoUpdate = table.Column<bool>(type: "INTEGER", nullable: false),
                    DaysToUpdate = table.Column<int>(type: "INTEGER", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_M3UFiles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "StreamGroups",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    StreamGroupNumber = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StreamGroups", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "VideoStreams",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CUID = table.Column<string>(type: "TEXT", nullable: false),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsHidden = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsReadOnly = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsUserCreated = table.Column<bool>(type: "INTEGER", nullable: false),
                    M3UFileId = table.Column<int>(type: "INTEGER", nullable: false),
                    StreamErrorCount = table.Column<int>(type: "INTEGER", nullable: false),
                    StreamLastFail = table.Column<DateTime>(type: "TEXT", nullable: false),
                    StreamLastStream = table.Column<DateTime>(type: "TEXT", nullable: false),
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
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Rank = table.Column<int>(type: "INTEGER", nullable: false),
                    StreamGroupId = table.Column<int>(type: "INTEGER", nullable: false),
                    VideoStreamId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StreamGroupVideoStream", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StreamGroupVideoStream_StreamGroups_StreamGroupId",
                        column: x => x.StreamGroupId,
                        principalTable: "StreamGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StreamGroupVideoStream_VideoStreams_VideoStreamId",
                        column: x => x.VideoStreamId,
                        principalTable: "VideoStreams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "VideoStreamRelationships",
                columns: table => new
                {
                    ChildVideoStreamId = table.Column<int>(type: "INTEGER", nullable: false),
                    ParentVideoStreamId = table.Column<int>(type: "INTEGER", nullable: false),
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
                name: "IX_StreamGroupVideoStream_StreamGroupId",
                table: "StreamGroupVideoStream",
                column: "StreamGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_StreamGroupVideoStream_VideoStreamId",
                table: "StreamGroupVideoStream",
                column: "VideoStreamId");

            migrationBuilder.CreateIndex(
                name: "IX_VideoStreamRelationships_ChildVideoStreamId",
                table: "VideoStreamRelationships",
                column: "ChildVideoStreamId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ChannelGroups");

            migrationBuilder.DropTable(
                name: "EPGFiles");

            migrationBuilder.DropTable(
                name: "Icons");

            migrationBuilder.DropTable(
                name: "M3UFiles");

            migrationBuilder.DropTable(
                name: "StreamGroupVideoStream");

            migrationBuilder.DropTable(
                name: "VideoStreamRelationships");

            migrationBuilder.DropTable(
                name: "StreamGroups");

            migrationBuilder.DropTable(
                name: "VideoStreams");
        }
    }
}
