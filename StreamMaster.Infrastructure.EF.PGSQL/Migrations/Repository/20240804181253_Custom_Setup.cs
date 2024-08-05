using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StreamMaster.Infrastructure.EF.PGSQL.Migrations.Repository
{
    /// <inheritdoc />
    public partial class Custom_Setup : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsCustomStream",
                table: "SMStreams");

            migrationBuilder.DropColumn(
                name: "IsCustomStream",
                table: "SMChannels");

            migrationBuilder.RenameColumn(
                name: "ChannelType",
                table: "SMChannels",
                newName: "SMChannelType");

            migrationBuilder.AddColumn<int>(
                name: "SMStreamType",
                table: "SMStreams",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SMStreamType",
                table: "SMStreams");

            migrationBuilder.RenameColumn(
                name: "SMChannelType",
                table: "SMChannels",
                newName: "ChannelType");

            migrationBuilder.AddColumn<bool>(
                name: "IsCustomStream",
                table: "SMStreams",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsCustomStream",
                table: "SMChannels",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }
    }
}
