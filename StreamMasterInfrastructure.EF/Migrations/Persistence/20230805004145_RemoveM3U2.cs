using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StreamMasterInfrastructure.EF.Migrations.Persistence
{
    /// <inheritdoc />
    public partial class RemoveM3U2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "M3UFiles");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "M3UFiles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    AutoUpdate = table.Column<bool>(type: "INTEGER", nullable: false),
                    ContentType = table.Column<string>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: false),
                    DownloadErrors = table.Column<int>(type: "INTEGER", nullable: false),
                    FileExists = table.Column<bool>(type: "INTEGER", nullable: false),
                    FileExtension = table.Column<string>(type: "TEXT", nullable: false),
                    HoursToUpdate = table.Column<int>(type: "INTEGER", nullable: false),
                    LastDownloadAttempt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    LastDownloaded = table.Column<DateTime>(type: "TEXT", nullable: false),
                    LastUpdated = table.Column<DateTime>(type: "TEXT", nullable: false),
                    MaxStreamCount = table.Column<int>(type: "INTEGER", nullable: false),
                    MinimumMinutesBetweenDownloads = table.Column<int>(type: "INTEGER", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    SMFileType = table.Column<int>(type: "INTEGER", nullable: false),
                    Source = table.Column<string>(type: "TEXT", nullable: false),
                    StartingChannelNumber = table.Column<int>(type: "INTEGER", nullable: false),
                    StationCount = table.Column<int>(type: "INTEGER", nullable: false),
                    Url = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_M3UFiles", x => x.Id);
                });
        }
    }
}
