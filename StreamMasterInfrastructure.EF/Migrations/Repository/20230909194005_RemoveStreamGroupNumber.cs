using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StreamMasterInfrastructureEF.Migrations.Repository
{
    /// <inheritdoc />
    public partial class RemoveStreamGroupNumber : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_StreamGroup_StreamGroupNumber",
                table: "StreamGroups");

            migrationBuilder.DropColumn(
                name: "StreamGroupNumber",
                table: "StreamGroups");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "StreamGroupNumber",
                table: "StreamGroups",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_StreamGroup_StreamGroupNumber",
                table: "StreamGroups",
                column: "StreamGroupNumber");
        }
    }
}
