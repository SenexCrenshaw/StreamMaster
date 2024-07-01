using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StreamMaster.Infrastructure.EF.PGSQL.Migrations.Repository
{
    /// <inheritdoc />
    public partial class CleanUp : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "SMStreamId",
                table: "SMStreams",
                newName: "ShortSMStreamId");

            migrationBuilder.AddColumn<string>(
                name: "DeviceID",
                table: "StreamGroups",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DeviceID",
                table: "StreamGroups");

            migrationBuilder.RenameColumn(
                name: "ShortSMStreamId",
                table: "SMStreams",
                newName: "SMStreamId");
        }
    }
}
