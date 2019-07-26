using Microsoft.EntityFrameworkCore.Migrations;

namespace BackEnd.Migrations
{
    public partial class UsernameAndConnectionString : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Plugs_Users_UserName",
                table: "Plugs");

            migrationBuilder.RenameColumn(
                name: "UserName",
                table: "Users",
                newName: "Username");

            migrationBuilder.RenameColumn(
                name: "UserName",
                table: "Plugs",
                newName: "Username");

            migrationBuilder.RenameIndex(
                name: "IX_Plugs_UserName",
                table: "Plugs",
                newName: "IX_Plugs_Username");

            migrationBuilder.AddForeignKey(
                name: "FK_Plugs_Users_Username",
                table: "Plugs",
                column: "Username",
                principalTable: "Users",
                principalColumn: "Username",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Plugs_Users_Username",
                table: "Plugs");

            migrationBuilder.RenameColumn(
                name: "Username",
                table: "Users",
                newName: "UserName");

            migrationBuilder.RenameColumn(
                name: "Username",
                table: "Plugs",
                newName: "UserName");

            migrationBuilder.RenameIndex(
                name: "IX_Plugs_Username",
                table: "Plugs",
                newName: "IX_Plugs_UserName");

            migrationBuilder.AddForeignKey(
                name: "FK_Plugs_Users_UserName",
                table: "Plugs",
                column: "UserName",
                principalTable: "Users",
                principalColumn: "UserName",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
