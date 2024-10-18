using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StreamMaster.Infrastructure.EF.PGSQL.Migrations.Repository
{
    /// <inheritdoc />
    public partial class AddedTVGName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TVGName",
                table: "SMStreams",
                type: "citext",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "ChannelName",
                table: "SMChannels",
                type: "citext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<string>(
                name: "TVGName",
                table: "SMChannels",
                type: "citext",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TVGName",
                table: "SMStreams");

            migrationBuilder.DropColumn(
                name: "TVGName",
                table: "SMChannels");

            migrationBuilder.AlterColumn<string>(
                name: "ChannelName",
                table: "SMChannels",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "citext");
        }
    }
}
