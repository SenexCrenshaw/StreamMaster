using Microsoft.EntityFrameworkCore.Migrations;

using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

using StreamMaster.Infrastructure.EF.PGSQL.Extenstions;

#nullable disable

namespace StreamMaster.Infrastructure.EF.PGSQL.Migrations.Repository
{
    /// <inheritdoc />
    public partial class ConvertToSMChannels : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTableIfExists("SystemKeyValues");

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
                name: "SMChannels",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn),
                    StreamingProxyType = table.Column<int>(type: "integer", nullable: false),
                    IsHidden = table.Column<bool>(type: "boolean", nullable: false),
                    ChannelNumber = table.Column<string>(type: "citext", nullable: false),
                    TimeShift = table.Column<int>(type: "integer", nullable: false),
                    Group = table.Column<string>(type: "citext", nullable: false),
                    EPGId = table.Column<string>(type: "citext", nullable: false),
                    Logo = table.Column<string>(type: "citext", nullable: false),
                    Name = table.Column<string>(type: "citext", nullable: false),
                    StationId = table.Column<string>(type: "citext", nullable: false),
                    GroupTitle = table.Column<string>(type: "citext", nullable: false),
                    VideoStreamHandler = table.Column<int>(type: "integer", nullable: false),
                    VideoStreamId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SMChannels", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SMStreams",
                columns: table => new
                {
                    Id = table.Column<string>(type: "citext", nullable: false),
                    FilePosition = table.Column<int>(type: "integer", nullable: false),
                    IsHidden = table.Column<bool>(type: "boolean", nullable: false),
                    IsUserCreated = table.Column<bool>(type: "boolean", nullable: false),
                    M3UFileId = table.Column<int>(type: "integer", nullable: false),
                    Tvg_chno = table.Column<int>(type: "integer", nullable: false),
                    M3UFileName = table.Column<string>(type: "citext", nullable: false),
                    SMChannelId = table.Column<string>(type: "citext", nullable: false),
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
                name: "StreamGroupSMChannels",
                columns: table => new
                {
                    SMChannelId = table.Column<int>(type: "integer", nullable: false),
                    StreamGroupId = table.Column<int>(type: "integer", nullable: false),
                    IsReadOnly = table.Column<bool>(type: "boolean", nullable: false),
                    Rank = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StreamGroupSMChannels", x => new { x.SMChannelId, x.StreamGroupId });
                    table.ForeignKey(
                        name: "FK_StreamGroupSMChannels_SMChannels_SMChannelId",
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
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SMChannelStreamLinks");

            migrationBuilder.DropTable(
                name: "StreamGroupSMChannels");

            migrationBuilder.DropTable(
                name: "SMStreams");

            migrationBuilder.DropTable(
                name: "SMChannels");

            migrationBuilder.DropTable(
                name: "SystemKeyValues");

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
        }
    }
}
