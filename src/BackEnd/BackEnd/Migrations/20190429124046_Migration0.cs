using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace BackEnd.Migrations
{
    public partial class Migration0 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PowerUsageSample_Plugs_PlugMac",
                table: "PowerUsageSample");

            migrationBuilder.DropForeignKey(
                name: "FK_Task_Plugs_DeviceMac",
                table: "Task");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Task",
                table: "Task");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PowerUsageSample",
                table: "PowerUsageSample");

            migrationBuilder.RenameTable(
                name: "Task",
                newName: "Tasks");

            migrationBuilder.RenameTable(
                name: "PowerUsageSample",
                newName: "PowerUsageSamples");

            migrationBuilder.RenameIndex(
                name: "IX_Task_DeviceMac",
                table: "Tasks",
                newName: "IX_Tasks_DeviceMac");

            migrationBuilder.RenameIndex(
                name: "IX_PowerUsageSample_PlugMac",
                table: "PowerUsageSamples",
                newName: "IX_PowerUsageSamples_PlugMac");

            migrationBuilder.AddColumn<DateTime>(
                name: "DateToBeExecuted",
                table: "Tasks",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Discriminator",
                table: "Tasks",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Tasks",
                table: "Tasks",
                column: "TaskId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PowerUsageSamples",
                table: "PowerUsageSamples",
                column: "SampleDate");

            migrationBuilder.AddForeignKey(
                name: "FK_PowerUsageSamples_Plugs_PlugMac",
                table: "PowerUsageSamples",
                column: "PlugMac",
                principalTable: "Plugs",
                principalColumn: "Mac",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Tasks_Plugs_DeviceMac",
                table: "Tasks",
                column: "DeviceMac",
                principalTable: "Plugs",
                principalColumn: "Mac",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PowerUsageSamples_Plugs_PlugMac",
                table: "PowerUsageSamples");

            migrationBuilder.DropForeignKey(
                name: "FK_Tasks_Plugs_DeviceMac",
                table: "Tasks");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Tasks",
                table: "Tasks");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PowerUsageSamples",
                table: "PowerUsageSamples");

            migrationBuilder.DropColumn(
                name: "DateToBeExecuted",
                table: "Tasks");

            migrationBuilder.DropColumn(
                name: "Discriminator",
                table: "Tasks");

            migrationBuilder.RenameTable(
                name: "Tasks",
                newName: "Task");

            migrationBuilder.RenameTable(
                name: "PowerUsageSamples",
                newName: "PowerUsageSample");

            migrationBuilder.RenameIndex(
                name: "IX_Tasks_DeviceMac",
                table: "Task",
                newName: "IX_Task_DeviceMac");

            migrationBuilder.RenameIndex(
                name: "IX_PowerUsageSamples_PlugMac",
                table: "PowerUsageSample",
                newName: "IX_PowerUsageSample_PlugMac");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Task",
                table: "Task",
                column: "TaskId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PowerUsageSample",
                table: "PowerUsageSample",
                column: "SampleDate");

            migrationBuilder.AddForeignKey(
                name: "FK_PowerUsageSample_Plugs_PlugMac",
                table: "PowerUsageSample",
                column: "PlugMac",
                principalTable: "Plugs",
                principalColumn: "Mac",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Task_Plugs_DeviceMac",
                table: "Task",
                column: "DeviceMac",
                principalTable: "Plugs",
                principalColumn: "Mac",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
