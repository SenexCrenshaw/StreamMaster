using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StreamMasterInfrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddStreamGroupChannel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "StreamGroupChannelGroup",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ChannelGroupId = table.Column<int>(type: "INTEGER", nullable: false),
                    StreamGroupId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StreamGroupChannelGroup", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StreamGroupChannelGroup_ChannelGroups_ChannelGroupId",
                        column: x => x.ChannelGroupId,
                        principalTable: "ChannelGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StreamGroupChannelGroup_StreamGroups_StreamGroupId",
                        column: x => x.StreamGroupId,
                        principalTable: "StreamGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_StreamGroupChannelGroup_ChannelGroupId",
                table: "StreamGroupChannelGroup",
                column: "ChannelGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_StreamGroupChannelGroup_StreamGroupId",
                table: "StreamGroupChannelGroup",
                column: "StreamGroupId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "StreamGroupChannelGroup");
        }
    }
}
