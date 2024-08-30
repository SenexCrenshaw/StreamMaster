using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StreamMaster.Infrastructure.EF.PGSQL.Migrations.Repository
{
    /// <inheritdoc />
    public partial class More_Indexes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "idx_streamgroups_name_id",
                table: "StreamGroups",
                columns: new[] { "Name", "Id" });

            migrationBuilder.CreateIndex(
                name: "idx_smchannelstreamlinks_smchannelid_rank",
                table: "SMChannelStreamLinks",
                columns: new[] { "SMChannelId", "Rank" });

            migrationBuilder.CreateIndex(
                name: "idx_smchannels_channelnumber_id",
                table: "SMChannels",
                columns: new[] { "ChannelNumber", "Id" });

            migrationBuilder.CreateIndex(
                name: "idx_epgfiles_url",
                table: "EPGFiles",
                column: "Url");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "idx_streamgroups_name_id",
                table: "StreamGroups");

            migrationBuilder.DropIndex(
                name: "idx_smchannelstreamlinks_smchannelid_rank",
                table: "SMChannelStreamLinks");

            migrationBuilder.DropIndex(
                name: "idx_smchannels_channelnumber_id",
                table: "SMChannels");

            migrationBuilder.DropIndex(
                name: "idx_epgfiles_url",
                table: "EPGFiles");
        }
    }
}
