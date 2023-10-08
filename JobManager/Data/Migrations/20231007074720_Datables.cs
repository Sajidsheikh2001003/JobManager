using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JobManager.Data.Migrations
{
    public partial class Datables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PickedJobs");

            migrationBuilder.DropTable(
                name: "UnPickedJobs");

            migrationBuilder.DropColumn(
                name: "UnPickedJobs",
                table: "Jobs");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "UnPickedJobs",
                table: "Jobs",
                type: "bit",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "PickedJobs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CompletedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FilePath = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsCompleted = table.Column<byte>(type: "tinyint", nullable: false),
                    JobName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    JobsId = table.Column<int>(type: "int", nullable: false),
                    UnPicked = table.Column<bool>(type: "bit", nullable: true),
                    UserId = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PickedJobs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UnPickedJobs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CompletedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FilePath = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsCompleted = table.Column<byte>(type: "tinyint", nullable: false),
                    JobName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PeekId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UnPickedJobs", x => x.Id);
                });
        }
    }
}
