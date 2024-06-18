using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace StreamMaster.Infrastructure.EF.PGSQL.Migrations.Repository
{
    /// <inheritdoc />
    public partial class Profiles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "StreamGroupProfiles",
                table: "StreamGroups");

            migrationBuilder.CreateTable(
                name: "StreamGroupProfiles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn),
                    StreamGroupId = table.Column<int>(type: "integer", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    FileProfileName = table.Column<string>(type: "text", nullable: false),
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

            migrationBuilder.CreateIndex(
                name: "IX_StreamGroupProfiles_StreamGroupId",
                table: "StreamGroupProfiles",
                column: "StreamGroupId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "StreamGroupProfiles");

            migrationBuilder.AddColumn<string[]>(
                name: "StreamGroupProfiles",
                table: "StreamGroups",
                type: "text[]",
                nullable: false,
                defaultValue: new string[0]);
        }
    }
}
