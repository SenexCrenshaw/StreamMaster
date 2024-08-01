using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StreamMaster.Infrastructure.EF.PGSQL.Migrations.Repository
{
    /// <inheritdoc />
    public partial class Added_Channel_Links : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "SMStreamId",
                table: "SMChannelStreamLinks",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "citext");

            migrationBuilder.AddColumn<int>(
                name: "ChannelType",
                table: "SMChannels",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "SMChannelChannelLinks",
                columns: table => new
                {
                    ParentSMChannelId = table.Column<int>(type: "integer", nullable: false),
                    ChildSMChannelId = table.Column<int>(type: "integer", nullable: false),
                    Rank = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SMChannelChannelLinks", x => new { x.ParentSMChannelId, x.ChildSMChannelId });
                    table.ForeignKey(
                        name: "FK_SMChannelChannelLinks_SMChannels_ChildSMChannelId",
                        column: x => x.ChildSMChannelId,
                        principalTable: "SMChannels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SMChannelChannelLinks_SMChannels_ParentSMChannelId",
                        column: x => x.ParentSMChannelId,
                        principalTable: "SMChannels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SMChannelChannelLinks_ChildSMChannelId",
                table: "SMChannelChannelLinks",
                column: "ChildSMChannelId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SMChannelChannelLinks");

            migrationBuilder.DropColumn(
                name: "ChannelType",
                table: "SMChannels");

            migrationBuilder.AlterColumn<string>(
                name: "SMStreamId",
                table: "SMChannelStreamLinks",
                type: "citext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");
        }
    }
}
