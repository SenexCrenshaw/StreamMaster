using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StreamMaster.Infrastructure.EF.PGSQL.Migrations.Repository
{
    /// <inheritdoc />
    public partial class Added_custom_to_channel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "StreamID",
                table: "SMChannels",
                newName: "BaseStreamID");

            migrationBuilder.AddColumn<string>(
                name: "CommandProfileName",
                table: "SMChannels",
                type: "citext",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CommandProfileName",
                table: "SMChannels");

            migrationBuilder.RenameColumn(
                name: "BaseStreamID",
                table: "SMChannels",
                newName: "StreamID");
        }
    }
}
