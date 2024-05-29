using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StreamMaster.Infrastructure.EF.PGSQL.Migrations.Repository
{
    /// <inheritdoc />
    public partial class Ids : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ShortId",
                table: "SMStreams",
                newName: "SMStreamId");

            migrationBuilder.AlterColumn<string>(
                name: "SMChannelId",
                table: "SMChannels",
                type: "citext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "SMStreamId",
                table: "SMStreams",
                newName: "ShortId");

            migrationBuilder.AlterColumn<string>(
                name: "SMChannelId",
                table: "SMChannels",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "citext");
        }
    }
}
