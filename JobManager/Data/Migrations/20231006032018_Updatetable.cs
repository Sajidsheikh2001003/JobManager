using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JobManager.Data.Migrations
{
    public partial class Updatetable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Text",
                table: "Jobs",
                newName: "JobName");

            migrationBuilder.AddColumn<DateTime>(
                name: "CompletedOn",
                table: "Jobs",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<byte>(
                name: "IsCompleted",
                table: "Jobs",
                type: "tinyint",
                nullable: false,
                defaultValue: (byte)0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CompletedOn",
                table: "Jobs");

            migrationBuilder.DropColumn(
                name: "IsCompleted",
                table: "Jobs");

            migrationBuilder.RenameColumn(
                name: "JobName",
                table: "Jobs",
                newName: "Text");
        }
    }
}
