using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StreamMaster.Infrastructure.EF.SQLite.Migrations.Repository
{
    /// <inheritdoc />
    public partial class CatchUp : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "User_Url",
                table: "VideoStreams",
                type: "citext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "User_Tvg_name",
                table: "VideoStreams",
                type: "citext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "User_Tvg_logo",
                table: "VideoStreams",
                type: "citext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "User_Tvg_group",
                table: "VideoStreams",
                type: "citext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "User_Tvg_ID",
                table: "VideoStreams",
                type: "citext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "Url",
                table: "VideoStreams",
                type: "citext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "Tvg_name",
                table: "VideoStreams",
                type: "citext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "Tvg_logo",
                table: "VideoStreams",
                type: "citext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "Tvg_group",
                table: "VideoStreams",
                type: "citext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "Tvg_ID",
                table: "VideoStreams",
                type: "citext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<int>(
                name: "TimeShift",
                table: "VideoStreams",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "StationId",
                table: "VideoStreams",
                type: "citext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "ShortId",
                table: "VideoStreams",
                type: "citext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "M3UFileName",
                table: "VideoStreams",
                type: "citext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "GroupTitle",
                table: "VideoStreams",
                type: "citext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "VideoStreams",
                type: "citext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "ChildVideoStreamId",
                table: "VideoStreamLinks",
                type: "citext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "ParentVideoStreamId",
                table: "VideoStreamLinks",
                type: "citext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "Value",
                table: "SystemKeyValues",
                type: "citext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "Key",
                table: "SystemKeyValues",
                type: "citext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "ChildVideoStreamId",
                table: "StreamGroupVideoStreams",
                type: "citext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "StreamGroups",
                type: "citext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AddColumn<string>(
                name: "FFMPEGProfileId",
                table: "StreamGroups",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "Url",
                table: "M3UFiles",
                type: "citext",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Source",
                table: "M3UFiles",
                type: "citext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "M3UFiles",
                type: "citext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "FileExtension",
                table: "M3UFiles",
                type: "citext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "M3UFiles",
                type: "citext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "ContentType",
                table: "M3UFiles",
                type: "citext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "Url",
                table: "EPGFiles",
                type: "citext",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "TimeShift",
                table: "EPGFiles",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(float),
                oldType: "REAL");

            migrationBuilder.AlterColumn<string>(
                name: "Source",
                table: "EPGFiles",
                type: "citext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "EPGFiles",
                type: "citext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "FileExtension",
                table: "EPGFiles",
                type: "citext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "EPGFiles",
                type: "citext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "ContentType",
                table: "EPGFiles",
                type: "citext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "Color",
                table: "EPGFiles",
                type: "citext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "RegexMatch",
                table: "ChannelGroups",
                type: "citext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "ChannelGroups",
                type: "citext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FFMPEGProfileId",
                table: "StreamGroups");

            migrationBuilder.AlterColumn<string>(
                name: "User_Url",
                table: "VideoStreams",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "citext");

            migrationBuilder.AlterColumn<string>(
                name: "User_Tvg_name",
                table: "VideoStreams",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "citext");

            migrationBuilder.AlterColumn<string>(
                name: "User_Tvg_logo",
                table: "VideoStreams",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "citext");

            migrationBuilder.AlterColumn<string>(
                name: "User_Tvg_group",
                table: "VideoStreams",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "citext");

            migrationBuilder.AlterColumn<string>(
                name: "User_Tvg_ID",
                table: "VideoStreams",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "citext");

            migrationBuilder.AlterColumn<string>(
                name: "Url",
                table: "VideoStreams",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "citext");

            migrationBuilder.AlterColumn<string>(
                name: "Tvg_name",
                table: "VideoStreams",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "citext");

            migrationBuilder.AlterColumn<string>(
                name: "Tvg_logo",
                table: "VideoStreams",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "citext");

            migrationBuilder.AlterColumn<string>(
                name: "Tvg_group",
                table: "VideoStreams",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "citext");

            migrationBuilder.AlterColumn<string>(
                name: "Tvg_ID",
                table: "VideoStreams",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "citext");

            migrationBuilder.AlterColumn<string>(
                name: "TimeShift",
                table: "VideoStreams",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AlterColumn<string>(
                name: "StationId",
                table: "VideoStreams",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "citext");

            migrationBuilder.AlterColumn<string>(
                name: "ShortId",
                table: "VideoStreams",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "citext");

            migrationBuilder.AlterColumn<string>(
                name: "M3UFileName",
                table: "VideoStreams",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "citext");

            migrationBuilder.AlterColumn<string>(
                name: "GroupTitle",
                table: "VideoStreams",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "citext");

            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "VideoStreams",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "citext");

            migrationBuilder.AlterColumn<string>(
                name: "ChildVideoStreamId",
                table: "VideoStreamLinks",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "citext");

            migrationBuilder.AlterColumn<string>(
                name: "ParentVideoStreamId",
                table: "VideoStreamLinks",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "citext");

            migrationBuilder.AlterColumn<string>(
                name: "Value",
                table: "SystemKeyValues",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "citext");

            migrationBuilder.AlterColumn<string>(
                name: "Key",
                table: "SystemKeyValues",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "citext");

            migrationBuilder.AlterColumn<string>(
                name: "ChildVideoStreamId",
                table: "StreamGroupVideoStreams",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "citext");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "StreamGroups",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "citext");

            migrationBuilder.AlterColumn<string>(
                name: "Url",
                table: "M3UFiles",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "citext",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Source",
                table: "M3UFiles",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "citext");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "M3UFiles",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "citext");

            migrationBuilder.AlterColumn<string>(
                name: "FileExtension",
                table: "M3UFiles",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "citext");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "M3UFiles",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "citext");

            migrationBuilder.AlterColumn<string>(
                name: "ContentType",
                table: "M3UFiles",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "citext");

            migrationBuilder.AlterColumn<string>(
                name: "Url",
                table: "EPGFiles",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "citext",
                oldNullable: true);

            migrationBuilder.AlterColumn<float>(
                name: "TimeShift",
                table: "EPGFiles",
                type: "REAL",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AlterColumn<string>(
                name: "Source",
                table: "EPGFiles",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "citext");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "EPGFiles",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "citext");

            migrationBuilder.AlterColumn<string>(
                name: "FileExtension",
                table: "EPGFiles",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "citext");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "EPGFiles",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "citext");

            migrationBuilder.AlterColumn<string>(
                name: "ContentType",
                table: "EPGFiles",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "citext");

            migrationBuilder.AlterColumn<string>(
                name: "Color",
                table: "EPGFiles",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "citext");

            migrationBuilder.AlterColumn<string>(
                name: "RegexMatch",
                table: "ChannelGroups",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "citext");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "ChannelGroups",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "citext");
        }
    }
}
