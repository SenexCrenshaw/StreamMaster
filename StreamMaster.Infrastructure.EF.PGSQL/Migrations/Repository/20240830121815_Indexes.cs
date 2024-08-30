using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StreamMaster.Infrastructure.EF.PGSQL.Migrations.Repository
{
    /// <inheritdoc />
    public partial class Indexes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_StreamGroupSMChannelLink_SMChannelId",
                table: "StreamGroupSMChannelLink");

            migrationBuilder.CreateIndex(
                name: "idx_streamgroupsmchannellink_smchannelid_streamgroupid",
                table: "StreamGroupSMChannelLink",
                columns: new[] { "SMChannelId", "StreamGroupId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "idx_smstreams_id",
                table: "SMStreams",
                column: "Id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "idx_smchannelstreamlinks_smchannelid_smstreamid",
                table: "SMChannelStreamLinks",
                columns: new[] { "SMChannelId", "SMStreamId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "idx_smchannels_basestreamid",
                table: "SMChannels",
                column: "BaseStreamID");

            migrationBuilder.CreateIndex(
                name: "idx_smchannels_id",
                table: "SMChannels",
                column: "Id",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "idx_streamgroupsmchannellink_smchannelid_streamgroupid",
                table: "StreamGroupSMChannelLink");

            migrationBuilder.DropIndex(
                name: "idx_smstreams_id",
                table: "SMStreams");

            migrationBuilder.DropIndex(
                name: "idx_smchannelstreamlinks_smchannelid_smstreamid",
                table: "SMChannelStreamLinks");

            migrationBuilder.DropIndex(
                name: "idx_smchannels_basestreamid",
                table: "SMChannels");

            migrationBuilder.DropIndex(
                name: "idx_smchannels_id",
                table: "SMChannels");

            migrationBuilder.CreateIndex(
                name: "IX_StreamGroupSMChannelLink_SMChannelId",
                table: "StreamGroupSMChannelLink",
                column: "SMChannelId");
        }
    }
}
