using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StreamMaster.Infrastructure.EF.PGSQL.Migrations.Repository
{
    /// <inheritdoc />
    public partial class Links2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
               name: "StreamingProxyType",
               table: "SMChannels");

            migrationBuilder.AddColumn<string>(
                name: "StreamingProxyType",
                table: "SMChannels",
                type: "text",
                defaultValue: "SystemDefault",
                nullable: false);

            migrationBuilder.DropColumn(
              name: "ChannelNumber",
              table: "SMChannels");

            migrationBuilder.AddColumn<string>(
                name: "ChannelNumber",
                table: "SMChannels",
                type: "integer",
                defaultValue: 0,
                nullable: false);


        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
              name: "StreamingProxyType",
              table: "SMChannels");

            migrationBuilder.AddColumn<string>(
                name: "StreamingProxyType",
                table: "SMChannels",
                type: "integer",
                nullable: false);

            migrationBuilder.DropColumn(
                         name: "ChannelNumber",
                         table: "SMChannels");

            migrationBuilder.AddColumn<string>(
                name: "ChannelNumber",
                table: "SMChannels",
                type: "string",
                defaultValue: "0",
                nullable: false);
        }
    }
}
