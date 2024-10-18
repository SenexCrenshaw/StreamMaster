using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace StreamMaster.Infrastructure.EF.PGSQL.Migrations.Repository
{
    /// <inheritdoc />
    public partial class Initial : Migration
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
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn),
                    IsSystem = table.Column<bool>(type: "boolean", nullable: false),
                    IsHidden = table.Column<bool>(type: "boolean", nullable: false),
                    IsReadOnly = table.Column<bool>(type: "boolean", nullable: false),
                    Name = table.Column<string>(type: "citext", nullable: false),
                    RegexMatch = table.Column<string>(type: "citext", nullable: false),
                    ActiveCount = table.Column<int>(type: "integer", nullable: false),
                    TotalCount = table.Column<int>(type: "integer", nullable: false),
                    HiddenCount = table.Column<int>(type: "integer", nullable: false)
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
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn),
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
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn),
                    EPGNumber = table.Column<int>(type: "integer", nullable: false),
                    Color = table.Column<string>(type: "citext", nullable: false),
                    ChannelCount = table.Column<int>(type: "integer", nullable: false),
                    ProgrammeCount = table.Column<int>(type: "integer", nullable: false),
                    TimeShift = table.Column<int>(type: "integer", nullable: false),
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
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn),
                    M3UKey = table.Column<int>(type: "integer", nullable: false),
                    M3UName = table.Column<int>(type: "integer", nullable: false),
                    VODTags = table.Column<List<string>>(type: "text[]", nullable: false),
                    M3U8OutPutProfile = table.Column<string>(type: "text", nullable: true),
                    MaxStreamCount = table.Column<int>(type: "integer", nullable: false),
                    StreamCount = table.Column<int>(type: "integer", nullable: false),
                    SyncChannels = table.Column<bool>(type: "boolean", nullable: false),
                    StartingChannelNumber = table.Column<int>(type: "integer", nullable: false),
                    AutoSetChannelNumbers = table.Column<bool>(type: "boolean", nullable: false),
                    DefaultStreamGroupName = table.Column<string>(type: "text", nullable: true),
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
                    HoursToUpdate = table.Column<int>(type: "integer", nullable: false),
                    Url = table.Column<string>(type: "citext", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_M3UFiles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SMChannels",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn),
                    CommandProfileName = table.Column<string>(type: "citext", nullable: false),
                    IsHidden = table.Column<bool>(type: "boolean", nullable: false),
                    BaseStreamID = table.Column<string>(type: "text", nullable: false),
                    M3UFileId = table.Column<int>(type: "integer", nullable: false),
                    ChannelNumber = table.Column<int>(type: "integer", nullable: false),
                    ChannelId = table.Column<string>(type: "text", nullable: false),
                    TVGName = table.Column<string>(type: "citext", nullable: false),
                    ChannelName = table.Column<string>(type: "citext", nullable: false),
                    TimeShift = table.Column<int>(type: "integer", nullable: false),
                    Group = table.Column<string>(type: "citext", nullable: false),
                    EPGId = table.Column<string>(type: "citext", nullable: false),
                    Logo = table.Column<string>(type: "citext", nullable: false),
                    Name = table.Column<string>(type: "citext", nullable: false),
                    ClientUserAgent = table.Column<string>(type: "citext", nullable: true),
                    StationId = table.Column<string>(type: "citext", nullable: false),
                    GroupTitle = table.Column<string>(type: "citext", nullable: false),
                    IsSystem = table.Column<bool>(type: "boolean", nullable: false),
                    SMChannelType = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SMChannels", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SMStreams",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    ClientUserAgent = table.Column<string>(type: "citext", nullable: true),
                    CommandProfileName = table.Column<string>(type: "citext", nullable: true),
                    FilePosition = table.Column<int>(type: "integer", nullable: false),
                    IsHidden = table.Column<bool>(type: "boolean", nullable: false),
                    IsUserCreated = table.Column<bool>(type: "boolean", nullable: false),
                    M3UFileId = table.Column<int>(type: "integer", nullable: false),
                    ChannelNumber = table.Column<int>(type: "integer", nullable: false),
                    M3UFileName = table.Column<string>(type: "citext", nullable: false),
                    Group = table.Column<string>(type: "citext", nullable: false),
                    EPGID = table.Column<string>(type: "citext", nullable: false),
                    Logo = table.Column<string>(type: "citext", nullable: false),
                    Name = table.Column<string>(type: "citext", nullable: false),
                    Url = table.Column<string>(type: "citext", nullable: false),
                    StationId = table.Column<string>(type: "citext", nullable: false),
                    IsSystem = table.Column<bool>(type: "boolean", nullable: false),
                    NeedsDelete = table.Column<bool>(type: "boolean", nullable: false),
                    ChannelName = table.Column<string>(type: "citext", nullable: false),
                    TVGName = table.Column<string>(type: "citext", nullable: false),
                    CUID = table.Column<string>(type: "citext", nullable: false),
                    ChannelId = table.Column<string>(type: "citext", nullable: false),
                    SMStreamType = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SMStreams", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "StreamGroups",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn),
                    DeviceID = table.Column<string>(type: "text", nullable: false),
                    IsReadOnly = table.Column<bool>(type: "boolean", nullable: false),
                    ShowIntros = table.Column<int>(type: "integer", nullable: false),
                    IsSystem = table.Column<bool>(type: "boolean", nullable: false),
                    Name = table.Column<string>(type: "citext", nullable: false),
                    GroupKey = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StreamGroups", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SystemKeyValues",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn),
                    Key = table.Column<string>(type: "citext", nullable: false),
                    Value = table.Column<string>(type: "citext", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SystemKeyValues", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SMChannelChannelLinks",
                columns: table => new
                {
                    ParentSMChannelId = table.Column<int>(type: "integer", nullable: false),
                    SMChannelId = table.Column<int>(type: "integer", nullable: false),
                    Rank = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SMChannelChannelLinks", x => new { x.ParentSMChannelId, x.SMChannelId });
                    table.ForeignKey(
                        name: "FK_SMChannelChannelLinks_SMChannels_ParentSMChannelId",
                        column: x => x.ParentSMChannelId,
                        principalTable: "SMChannels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SMChannelChannelLinks_SMChannels_SMChannelId",
                        column: x => x.SMChannelId,
                        principalTable: "SMChannels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SMChannelStreamLinks",
                columns: table => new
                {
                    SMChannelId = table.Column<int>(type: "integer", nullable: false),
                    SMStreamId = table.Column<string>(type: "text", nullable: false),
                    Rank = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SMChannelStreamLinks", x => new { x.SMChannelId, x.SMStreamId });
                    table.ForeignKey(
                        name: "FK_SMChannelStreamLinks_SMChannels_SMChannelId",
                        column: x => x.SMChannelId,
                        principalTable: "SMChannels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SMChannelStreamLinks_SMStreams_SMStreamId",
                        column: x => x.SMStreamId,
                        principalTable: "SMStreams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
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
                name: "StreamGroupProfiles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn),
                    StreamGroupId = table.Column<int>(type: "integer", nullable: false),
                    ProfileName = table.Column<string>(type: "text", nullable: false),
                    OutputProfileName = table.Column<string>(type: "text", nullable: false),
                    CommandProfileName = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StreamGroupProfiles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StreamGroupProfiles_StreamGroups_StreamGroupId",
                        column: x => x.StreamGroupId,
                        principalTable: "StreamGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StreamGroupSMChannelLink",
                columns: table => new
                {
                    SMChannelId = table.Column<int>(type: "integer", nullable: false),
                    StreamGroupId = table.Column<int>(type: "integer", nullable: false),
                    IsReadOnly = table.Column<bool>(type: "boolean", nullable: false),
                    Rank = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StreamGroupSMChannelLink", x => new { x.StreamGroupId, x.SMChannelId });
                    table.ForeignKey(
                        name: "FK_StreamGroupSMChannelLink_SMChannels_SMChannelId",
                        column: x => x.SMChannelId,
                        principalTable: "SMChannels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StreamGroupSMChannelLink_StreamGroups_StreamGroupId",
                        column: x => x.StreamGroupId,
                        principalTable: "StreamGroups",
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
                name: "idx_epgfiles_url",
                table: "EPGFiles",
                column: "Url");

            migrationBuilder.CreateIndex(
                name: "IX_SMChannelChannelLinks_SMChannelId",
                table: "SMChannelChannelLinks",
                column: "SMChannelId");

            migrationBuilder.CreateIndex(
                name: "idx_SMChannelName",
                table: "SMChannels",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "idx_smchannels_basestreamid",
                table: "SMChannels",
                column: "BaseStreamID");

            migrationBuilder.CreateIndex(
                name: "idx_smchannels_channelnumber_id",
                table: "SMChannels",
                columns: new[] { "ChannelNumber", "Id" });

            migrationBuilder.CreateIndex(
                name: "idx_smchannels_id",
                table: "SMChannels",
                column: "Id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "idx_smchannelstreamlinks_smchannelid_rank",
                table: "SMChannelStreamLinks",
                columns: new[] { "SMChannelId", "Rank" });

            migrationBuilder.CreateIndex(
                name: "idx_smchannelstreamlinks_smchannelid_smstreamid",
                table: "SMChannelStreamLinks",
                columns: new[] { "SMChannelId", "SMStreamId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SMChannelStreamLinks_SMStreamId",
                table: "SMChannelStreamLinks",
                column: "SMStreamId");

            migrationBuilder.CreateIndex(
                name: "idx_SMStreamName",
                table: "SMStreams",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "idx_smstreams_id",
                table: "SMStreams",
                column: "Id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_StreamGroupChannelGroups_StreamGroupId",
                table: "StreamGroupChannelGroups",
                column: "StreamGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_StreamGroupProfiles_StreamGroupId",
                table: "StreamGroupProfiles",
                column: "StreamGroupId");

            migrationBuilder.CreateIndex(
                name: "idx_streamgroups_name_id",
                table: "StreamGroups",
                columns: new[] { "Name", "Id" });

            migrationBuilder.CreateIndex(
                name: "idx_streamgroupsmchannellink_smchannelid_streamgroupid",
                table: "StreamGroupSMChannelLink",
                columns: new[] { "SMChannelId", "StreamGroupId" },
                unique: true);
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
                name: "SMChannelChannelLinks");

            migrationBuilder.DropTable(
                name: "SMChannelStreamLinks");

            migrationBuilder.DropTable(
                name: "StreamGroupChannelGroups");

            migrationBuilder.DropTable(
                name: "StreamGroupProfiles");

            migrationBuilder.DropTable(
                name: "StreamGroupSMChannelLink");

            migrationBuilder.DropTable(
                name: "SystemKeyValues");

            migrationBuilder.DropTable(
                name: "SMStreams");

            migrationBuilder.DropTable(
                name: "ChannelGroups");

            migrationBuilder.DropTable(
                name: "SMChannels");

            migrationBuilder.DropTable(
                name: "StreamGroups");
        }
    }
}
