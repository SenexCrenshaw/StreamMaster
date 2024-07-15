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
                    IsHidden = table.Column<bool>(type: "boolean", nullable: false),
                    IsReadOnly = table.Column<bool>(type: "boolean", nullable: false),
                    Name = table.Column<string>(type: "citext", nullable: false),
                    RegexMatch = table.Column<string>(type: "citext", nullable: false),
                    ActiveCount = table.Column<int>(type: "integer", nullable: false),
                    TotalCount = table.Column<int>(type: "integer", nullable: false),
                    HiddenCount = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table => table.PrimaryKey("PK_ChannelGroups", x => x.Id));

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
                    VODTags = table.Column<List<string>>(type: "text[]", nullable: false),
                    MaxStreamCount = table.Column<int>(type: "integer", nullable: false),
                    StreamCount = table.Column<int>(type: "integer", nullable: false),
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
                    StreamingProxyType = table.Column<string>(type: "text", nullable: false),
                    IsHidden = table.Column<bool>(type: "boolean", nullable: false),
                    ChannelNumber = table.Column<int>(type: "integer", nullable: false),
                    TimeShift = table.Column<int>(type: "integer", nullable: false),
                    Group = table.Column<string>(type: "citext", nullable: false),
                    EPGId = table.Column<string>(type: "citext", nullable: false),
                    Logo = table.Column<string>(type: "citext", nullable: false),
                    Name = table.Column<string>(type: "citext", nullable: false),
                    StationId = table.Column<string>(type: "citext", nullable: false),
                    GroupTitle = table.Column<string>(type: "citext", nullable: false),
                    VideoStreamHandler = table.Column<int>(type: "integer", nullable: false),
                    ShortSMChannelId = table.Column<string>(type: "citext", nullable: false)
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
                    FilePosition = table.Column<int>(type: "integer", nullable: false),
                    IsHidden = table.Column<bool>(type: "boolean", nullable: false),
                    IsUserCreated = table.Column<bool>(type: "boolean", nullable: false),
                    M3UFileId = table.Column<int>(type: "integer", nullable: false),
                    ChannelNumber = table.Column<int>(type: "integer", nullable: false),
                    M3UFileName = table.Column<string>(type: "citext", nullable: false),
                    ShortSMStreamId = table.Column<string>(type: "citext", nullable: false),
                    Group = table.Column<string>(type: "citext", nullable: false),
                    EPGID = table.Column<string>(type: "citext", nullable: false),
                    Logo = table.Column<string>(type: "citext", nullable: false),
                    Name = table.Column<string>(type: "citext", nullable: false),
                    Url = table.Column<string>(type: "citext", nullable: false),
                    StationId = table.Column<string>(type: "citext", nullable: false)
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
                    IgnoreExistingChannelNumbers = table.Column<bool>(type: "boolean", nullable: false),
                    AutoSetChannelNumbers = table.Column<bool>(type: "boolean", nullable: false),
                    StartingChannelNumber = table.Column<int>(type: "integer", nullable: false),
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
                name: "SMChannelStreamLinks",
                columns: table => new
                {
                    SMChannelId = table.Column<int>(type: "integer", nullable: false),
                    SMStreamId = table.Column<string>(type: "citext", nullable: false),
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
                    Name = table.Column<string>(type: "text", nullable: false),
                    OutputProfileName = table.Column<string>(type: "text", nullable: false),
                    VideoProfileName = table.Column<string>(type: "text", nullable: false)
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
                name: "idx_SMChannelName",
                table: "SMChannels",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_SMChannelStreamLinks_SMStreamId",
                table: "SMChannelStreamLinks",
                column: "SMStreamId");

            migrationBuilder.CreateIndex(
                name: "idx_SMStreamName",
                table: "SMStreams",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_StreamGroupChannelGroups_StreamGroupId",
                table: "StreamGroupChannelGroups",
                column: "StreamGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_StreamGroupProfiles_StreamGroupId",
                table: "StreamGroupProfiles",
                column: "StreamGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_StreamGroupSMChannelLink_SMChannelId",
                table: "StreamGroupSMChannelLink",
                column: "SMChannelId");
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
