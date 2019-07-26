using Microsoft.EntityFrameworkCore.Migrations;

namespace BackEnd.Migrations
{
    public partial class AvoidCircularTaskReference : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Plugs_Users_UserName",
                table: "Plugs");

            migrationBuilder.DropForeignKey(
                name: "FK_PowerUsageSamples_Plugs_PlugMac",
                table: "PowerUsageSamples");

            migrationBuilder.DropForeignKey(
                name: "FK_Tasks_Plugs_DeviceMac",
                table: "Tasks");

            migrationBuilder.DropIndex(
                name: "IX_Tasks_DeviceMac",
                table: "Tasks");

            migrationBuilder.AlterColumn<string>(
                name: "DeviceMac",
                table: "Tasks",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PlugMac",
                table: "Tasks",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Tasks_PlugMac",
                table: "Tasks",
                column: "PlugMac");

            migrationBuilder.AddForeignKey(
                name: "FK_Plugs_Users_UserName",
                table: "Plugs",
                column: "UserName",
                principalTable: "Users",
                principalColumn: "UserName",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PowerUsageSamples_Plugs_PlugMac",
                table: "PowerUsageSamples",
                column: "PlugMac",
                principalTable: "Plugs",
                principalColumn: "Mac",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Tasks_Plugs_PlugMac",
                table: "Tasks",
                column: "PlugMac",
                principalTable: "Plugs",
                principalColumn: "Mac",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Plugs_Users_UserName",
                table: "Plugs");

            migrationBuilder.DropForeignKey(
                name: "FK_PowerUsageSamples_Plugs_PlugMac",
                table: "PowerUsageSamples");

            migrationBuilder.DropForeignKey(
                name: "FK_Tasks_Plugs_PlugMac",
                table: "Tasks");

            migrationBuilder.DropIndex(
                name: "IX_Tasks_PlugMac",
                table: "Tasks");

            migrationBuilder.DropColumn(
                name: "PlugMac",
                table: "Tasks");

            migrationBuilder.AlterColumn<string>(
                name: "DeviceMac",
                table: "Tasks",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Tasks_DeviceMac",
                table: "Tasks",
                column: "DeviceMac");

            migrationBuilder.AddForeignKey(
                name: "FK_Plugs_Users_UserName",
                table: "Plugs",
                column: "UserName",
                principalTable: "Users",
                principalColumn: "UserName",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PowerUsageSamples_Plugs_PlugMac",
                table: "PowerUsageSamples",
                column: "PlugMac",
                principalTable: "Plugs",
                principalColumn: "Mac",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Tasks_Plugs_DeviceMac",
                table: "Tasks",
                column: "DeviceMac",
                principalTable: "Plugs",
                principalColumn: "Mac",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
