using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StreamMaster.Infrastructure.EF.PGSQL.Migrations.Repository
{
    /// <inheritdoc />
    public partial class RemoveUnusedCols : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "M3UFiles");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "EPGFiles");

            migrationBuilder.RenameColumn(
                name: "Tvg_chno",
                table: "SMStreams",
                newName: "ChannelNumber");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ChannelNumber",
                table: "SMStreams",
                newName: "Tvg_chno");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "M3UFiles",
                type: "citext",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "EPGFiles",
                type: "citext",
                nullable: false,
                defaultValue: "");
        }
    }
}
