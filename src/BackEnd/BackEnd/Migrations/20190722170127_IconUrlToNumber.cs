using Microsoft.EntityFrameworkCore.Migrations;

namespace BackEnd.Migrations
{
    public partial class IconUrlToNumber : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IconUrl",
                table: "Plugs");

            migrationBuilder.AddColumn<int>(
                name: "IconNumber",
                table: "Plugs",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IconNumber",
                table: "Plugs");

            migrationBuilder.AddColumn<string>(
                name: "IconUrl",
                table: "Plugs",
                nullable: true);
        }
    }
}
