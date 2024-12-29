using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StreamMaster.Infrastructure.EF.PGSQL.Migrations.Repository
{
    /// <inheritdoc />
    public partial class AddIndexesForOptimization : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "idx_smstreams_m3ufileid",
                table: "SMStreams",
                column: "M3UFileId");

            migrationBuilder.CreateIndex(
                name: "idx_smstreams_needsdelete_m3ufileid",
                table: "SMStreams",
                columns: new[] { "NeedsDelete", "M3UFileId" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "idx_smstreams_m3ufileid",
                table: "SMStreams");

            migrationBuilder.DropIndex(
                name: "idx_smstreams_needsdelete_m3ufileid",
                table: "SMStreams");
        }
    }
}
