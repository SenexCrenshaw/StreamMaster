using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StreamMaster.Infrastructure.EF.PGSQL.Migrations.Repository
{
    /// <inheritdoc />
    public partial class M3U8OutPutProfile : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "M3U8OutPutProfile",
                table: "M3UFiles",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "M3U8OutPutProfile",
                table: "M3UFiles");
        }
    }
}
