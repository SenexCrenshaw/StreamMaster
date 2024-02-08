using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace StreamMaster.Infrastructure.EF.PGSQL.Migrations.Repository
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
                name: "ChannelGroups",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    IsHidden = table.Column<bool>(type: "boolean", nullable: false),
                    IsReadOnly = table.Column<bool>(type: "boolean", nullable: false),
                    Name = table.Column<string>(type: "citext", nullable: false),
                    RegexMatch = table.Column<string>(type: "citext", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChannelGroups", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DataProtectionKeys",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    FriendlyName = table.Column<string>(type: "text", nullable: true),
                    Xml = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DataProtectionKeys", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EPGFiles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    EPGNumber = table.Column<int>(type: "integer", nullable: false),
                    Color = table.Column<string>(type: "citext", nullable: false),
                    ChannelCount = table.Column<int>(type: "integer", nullable: false),
                    ProgrammeCount = table.Column<int>(type: "integer", nullable: false),
                    TimeShift = table.Column<float>(type: "real", nullable: false),
                    ContentType = table.Column<string>(type: "citext", nullable: false),
                    DownloadErrors = table.Column<int>(type: "integer", nullable: false),
                    FileExists = table.Column<bool>(type: "boolean", nullable: false),
                    FileExtension = table.Column<string>(type: "citext", nullable: false),
                    LastDownloadAttempt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastDownloaded = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastUpdated = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    MinimumMinutesBetweenDownloads = table.Column<int>(type: "integer", nullable: false),
                    Name = table.Column<string>(type: "citext", nullable: false),
                    SMFileType = table.Column<int>(type: "integer", nullable: false),
                    Source = table.Column<string>(type: "citext", nullable: false),
                    AutoUpdate = table.Column<bool>(type: "boolean", nullable: false),
                    Description = table.Column<string>(type: "citext", nullable: false),
                    HoursToUpdate = table.Column<int>(type: "integer", nullable: false),
                    Url = table.Column<string>(type: "citext", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EPGFiles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "M3UFiles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    VODTags = table.Column<List<string>>(type: "text[]", nullable: false),
                    OverwriteChannelNumbers = table.Column<bool>(type: "boolean", nullable: false),
                    MaxStreamCount = table.Column<int>(type: "integer", nullable: false),
                    StartingChannelNumber = table.Column<int>(type: "integer", nullable: false),
                    StationCount = table.Column<int>(type: "integer", nullable: false),
                    ContentType = table.Column<string>(type: "citext", nullable: false),
                    DownloadErrors = table.Column<int>(type: "integer", nullable: false),
                    FileExists = table.Column<bool>(type: "boolean", nullable: false),
                    FileExtension = table.Column<string>(type: "citext", nullable: false),
                    LastDownloadAttempt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastDownloaded = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastUpdated = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    MinimumMinutesBetweenDownloads = table.Column<int>(type: "integer", nullable: false),
                    Name = table.Column<string>(type: "citext", nullable: false),
                    SMFileType = table.Column<int>(type: "integer", nullable: false),
                    Source = table.Column<string>(type: "citext", nullable: false),
                    AutoUpdate = table.Column<bool>(type: "boolean", nullable: false),
                    Description = table.Column<string>(type: "citext", nullable: false),
                    HoursToUpdate = table.Column<int>(type: "integer", nullable: false),
                    Url = table.Column<string>(type: "citext", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_M3UFiles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "StreamGroups",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    IsReadOnly = table.Column<bool>(type: "boolean", nullable: false),
                    AutoSetChannelNumbers = table.Column<bool>(type: "boolean", nullable: false),
                    Name = table.Column<string>(type: "citext", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StreamGroups", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SystemKeyValues",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Key = table.Column<string>(type: "citext", nullable: false),
                    Value = table.Column<string>(type: "citext", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SystemKeyValues", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "VideoStreams",
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
                    ShortId = table.Column<string>(type: "citext", nullable: false),
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
                    table.PrimaryKey("PK_VideoStreams", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "StreamGroupChannelGroups",
                columns: table => new
                {
                    ChannelGroupId = table.Column<int>(type: "integer", nullable: false),
                    StreamGroupId = table.Column<int>(type: "integer", nullable: false)
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
                        onDelete: ReferentialAction.Cascade);
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
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "VideoStreamLinks",
                columns: table => new
                {
                    ChildVideoStreamId = table.Column<string>(type: "citext", nullable: false),
                    ParentVideoStreamId = table.Column<string>(type: "citext", nullable: false),
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
                name: "idx_Name",
                table: "ChannelGroups",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "idx_Name_IsHidden",
                table: "ChannelGroups",
                columns: new[] { "Name", "IsHidden" });

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

            migrationBuilder.CreateIndex(
                name: "idx_User_Tvg_group",
                table: "VideoStreams",
                column: "User_Tvg_group");

            migrationBuilder.CreateIndex(
                name: "idx_User_Tvg_group_IsHidden",
                table: "VideoStreams",
                columns: new[] { "User_Tvg_group", "IsHidden" });

            migrationBuilder.CreateIndex(
                name: "IX_VideoStream_ShortId",
                table: "VideoStreams",
                column: "ShortId");

            migrationBuilder.CreateIndex(
                name: "IX_VideoStream_User_Tvg_chno",
                table: "VideoStreams",
                column: "User_Tvg_chno");

            migrationBuilder.CreateIndex(
                name: "IX_VideoStream_User_Tvg_name",
                table: "VideoStreams",
                column: "User_Tvg_name");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DataProtectionKeys");

            migrationBuilder.DropTable(
                name: "EPGFiles");

            migrationBuilder.DropTable(
                name: "M3UFiles");

            migrationBuilder.DropTable(
                name: "StreamGroupChannelGroups");

            migrationBuilder.DropTable(
                name: "StreamGroupVideoStreams");

            migrationBuilder.DropTable(
                name: "SystemKeyValues");

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
