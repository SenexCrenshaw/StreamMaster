using Microsoft.EntityFrameworkCore.Migrations;

using StreamMaster.Domain.Enums;


#nullable disable

namespace StreamMaster.Infrastructure.EF.Migrations.Repository
{
    /// <inheritdoc />
    public partial class AddVideoStreamProxyType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase(
                collation: "NOCASE_UTF8",
                oldCollation: "NOCASE");

            migrationBuilder.AddColumn<int>(
                name: "StreamingProxyType",
                table: "VideoStreams",
                type: "INTEGER",
                nullable: false,
                defaultValue: StreamingProxyTypes.SystemDefault);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "StreamingProxyType",
                table: "VideoStreams");

            migrationBuilder.AlterDatabase(
                collation: "NOCASE",
                oldCollation: "NOCASE_UTF8");
        }
    }
}
