using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StreamMaster.Infrastructure.EF.PGSQL.Migrations.Repository
{
    /// <inheritdoc />
    public partial class ProfileName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Name",
                table: "StreamGroupProfiles",
                newName: "ProfileName");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ProfileName",
                table: "StreamGroupProfiles",
                newName: "Name");
        }
    }
}
