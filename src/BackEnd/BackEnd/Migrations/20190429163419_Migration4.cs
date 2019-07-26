using Microsoft.EntityFrameworkCore.Migrations;

namespace BackEnd.Migrations
{
    public partial class Migration4 : Migration
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

            migrationBuilder.AddForeignKey(
                name: "FK_Plugs_Users_UserName",
                table: "Plugs",
                column: "UserName",
                principalTable: "Users",
                principalColumn: "UserName",
                onDelete: ReferentialAction.Restrict);

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
                name: "FK_Plugs_Users_UserName",
                table: "Plugs");

            migrationBuilder.DropForeignKey(
                name: "FK_PowerUsageSamples_Plugs_PlugMac",
                table: "PowerUsageSamples");

            migrationBuilder.DropForeignKey(
                name: "FK_Tasks_Plugs_DeviceMac",
                table: "Tasks");

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
