using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StreamMaster.Infrastructure.EF.PGSQL.Migrations.Repository
{
    /// <inheritdoc />
    public partial class Indexes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "idx_smstreams_group",
                table: "SMStreams",
                column: "Group");

            migrationBuilder.CreateIndex(
                name: "idx_smstreams_group_ishidden",
                table: "SMStreams",
                columns: new[] { "Group", "IsHidden" });

            migrationBuilder.CreateIndex(
                name: "idx_smstreams_m3ufileid",
                table: "SMStreams",
                column: "M3UFileId");

            migrationBuilder.CreateIndex(
                name: "idx_smstreams_needsdelete_m3ufileid",
                table: "SMStreams",
                columns: new[] { "NeedsDelete", "M3UFileId" });

            migrationBuilder.CreateIndex(
                name: "idx_smchannels_group",
                table: "SMChannels",
                column: "Group");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "idx_smstreams_group",
                table: "SMStreams");

            migrationBuilder.DropIndex(
                name: "idx_smstreams_group_ishidden",
                table: "SMStreams");

            migrationBuilder.DropIndex(
                name: "idx_smstreams_m3ufileid",
                table: "SMStreams");

            migrationBuilder.DropIndex(
                name: "idx_smstreams_needsdelete_m3ufileid",
                table: "SMStreams");

            migrationBuilder.DropIndex(
                name: "idx_smchannels_group",
                table: "SMChannels");
        }
    }
}
