using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StreamMaster.Infrastructure.Persistence.Migrations
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
                name: "DataProtectionKeys",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    FriendlyName = table.Column<string>(type: "TEXT", nullable: true),
                    Xml = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DataProtectionKeys", x => x.Id);
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
                    MinimumMinutesBetweenDownloads = table.Column<int>(type: "INTEGER", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    SMFileType = table.Column<int>(type: "INTEGER", nullable: false),
                    Source = table.Column<string>(type: "TEXT", nullable: false),
                    AutoUpdate = table.Column<bool>(type: "INTEGER", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: false),
                    HoursToUpdate = table.Column<int>(type: "INTEGER", nullable: false),
                    Url = table.Column<string>(type: "TEXT", nullable: true)
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
                    MinimumMinutesBetweenDownloads = table.Column<int>(type: "INTEGER", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    SMFileType = table.Column<int>(type: "INTEGER", nullable: false),
                    Source = table.Column<string>(type: "TEXT", nullable: false)
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
                    StartingChannelNumber = table.Column<int>(type: "INTEGER", nullable: false),
                    StationCount = table.Column<int>(type: "INTEGER", nullable: false),
                    ContentType = table.Column<string>(type: "TEXT", nullable: false),
                    DownloadErrors = table.Column<int>(type: "INTEGER", nullable: false),
                    FileExists = table.Column<bool>(type: "INTEGER", nullable: false),
                    FileExtension = table.Column<string>(type: "TEXT", nullable: false),
                    LastDownloadAttempt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    LastDownloaded = table.Column<DateTime>(type: "TEXT", nullable: false),
                    MinimumMinutesBetweenDownloads = table.Column<int>(type: "INTEGER", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    SMFileType = table.Column<int>(type: "INTEGER", nullable: false),
                    Source = table.Column<string>(type: "TEXT", nullable: false),
                    AutoUpdate = table.Column<bool>(type: "INTEGER", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: false),
                    HoursToUpdate = table.Column<int>(type: "INTEGER", nullable: false),
                    Url = table.Column<string>(type: "TEXT", nullable: true)
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
                name: "StreamGroupChannelGroups",
                columns: table => new
                {
                    ChannelGroupId = table.Column<int>(type: "INTEGER", nullable: false),
                    StreamGroupId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StreamGroupChannelGroups", x => new { x.ChannelGroupId, x.StreamGroupId });
                    table.ForeignKey(
                        name: "FK_StreamGroupChannelGroups_ChannelGroups_ChannelGroupId",
                        column: x => x.ChannelGroupId,
                        principalTable: "ChannelGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StreamGroupChannelGroups_StreamGroups_StreamGroupId",
                        column: x => x.StreamGroupId,
                        principalTable: "StreamGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "StreamGroupVideoStreams",
                columns: table => new
                {
                    ChildVideoStreamId = table.Column<string>(type: "TEXT", nullable: false),
                    StreamGroupId = table.Column<int>(type: "INTEGER", nullable: false),
                    IsReadOnly = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StreamGroupVideoStreams", x => new { x.ChildVideoStreamId, x.StreamGroupId });
                    table.ForeignKey(
                        name: "FK_StreamGroupVideoStreams_StreamGroups_StreamGroupId",
                        column: x => x.StreamGroupId,
                        principalTable: "StreamGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StreamGroupVideoStreams_VideoStreams_ChildVideoStreamId",
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
                name: "IX_StreamGroupChannelGroups_StreamGroupId",
                table: "StreamGroupChannelGroups",
                column: "StreamGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_StreamGroupVideoStreams_StreamGroupId",
                table: "StreamGroupVideoStreams",
                column: "StreamGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_VideoStreamLinks_ChildVideoStreamId",
                table: "VideoStreamLinks",
                column: "ChildVideoStreamId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DataProtectionKeys");

            migrationBuilder.DropTable(
                name: "EPGFiles");

            migrationBuilder.DropTable(
                name: "Icons");

            migrationBuilder.DropTable(
                name: "M3UFiles");

            migrationBuilder.DropTable(
                name: "StreamGroupChannelGroups");

            migrationBuilder.DropTable(
                name: "StreamGroupVideoStreams");

            migrationBuilder.DropTable(
                name: "VideoStreamLinks");

            migrationBuilder.DropTable(
                name: "ChannelGroups");

            migrationBuilder.DropTable(
                name: "StreamGroups");

            migrationBuilder.DropTable(
                name: "VideoStreams");
        }
    }
}
