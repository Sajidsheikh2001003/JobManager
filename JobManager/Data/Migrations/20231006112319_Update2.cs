using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JobManager.Data.Migrations
{
    public partial class Update2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PeekJobs_Jobs_JobsId",
                table: "PeekJobs");

            migrationBuilder.DropForeignKey(
                name: "FK_UnPeekJobs_Jobs_JobId",
                table: "UnPeekJobs");

            migrationBuilder.DropIndex(
                name: "IX_UnPeekJobs_JobId",
                table: "UnPeekJobs");

            migrationBuilder.DropIndex(
                name: "IX_PeekJobs_JobsId",
                table: "PeekJobs");

            migrationBuilder.RenameColumn(
                name: "JobId",
                table: "UnPeekJobs",
                newName: "PeekId");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "UnPeekJobs",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<DateTime>(
                name: "CompletedOn",
                table: "UnPeekJobs",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "FilePath",
                table: "UnPeekJobs",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<byte>(
                name: "IsCompleted",
                table: "UnPeekJobs",
                type: "tinyint",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.AddColumn<string>(
                name: "JobName",
                table: "UnPeekJobs",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "PeekJobs",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<DateTime>(
                name: "CompletedOn",
                table: "PeekJobs",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "FilePath",
                table: "PeekJobs",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<byte>(
                name: "IsCompleted",
                table: "PeekJobs",
                type: "tinyint",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.AddColumn<string>(
                name: "JobName",
                table: "PeekJobs",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CompletedOn",
                table: "UnPeekJobs");

            migrationBuilder.DropColumn(
                name: "FilePath",
                table: "UnPeekJobs");

            migrationBuilder.DropColumn(
                name: "IsCompleted",
                table: "UnPeekJobs");

            migrationBuilder.DropColumn(
                name: "JobName",
                table: "UnPeekJobs");

            migrationBuilder.DropColumn(
                name: "CompletedOn",
                table: "PeekJobs");

            migrationBuilder.DropColumn(
                name: "FilePath",
                table: "PeekJobs");

            migrationBuilder.DropColumn(
                name: "IsCompleted",
                table: "PeekJobs");

            migrationBuilder.DropColumn(
                name: "JobName",
                table: "PeekJobs");

            migrationBuilder.RenameColumn(
                name: "PeekId",
                table: "UnPeekJobs",
                newName: "JobId");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "UnPeekJobs",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "PeekJobs",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_UnPeekJobs_JobId",
                table: "UnPeekJobs",
                column: "JobId");

            migrationBuilder.CreateIndex(
                name: "IX_PeekJobs_JobsId",
                table: "PeekJobs",
                column: "JobsId");

            migrationBuilder.AddForeignKey(
                name: "FK_PeekJobs_Jobs_JobsId",
                table: "PeekJobs",
                column: "JobsId",
                principalTable: "Jobs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UnPeekJobs_Jobs_JobId",
                table: "UnPeekJobs",
                column: "JobId",
                principalTable: "Jobs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
