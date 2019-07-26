using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace BackEnd.Migrations
{
    public partial class SampleId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_PowerUsageSamples",
                table: "PowerUsageSamples");

            migrationBuilder.AddColumn<int>(
                name: "PowerUsageSampleId",
                table: "PowerUsageSamples",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AddPrimaryKey(
                name: "PK_PowerUsageSamples",
                table: "PowerUsageSamples",
                column: "PowerUsageSampleId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_PowerUsageSamples",
                table: "PowerUsageSamples");

            migrationBuilder.DropColumn(
                name: "PowerUsageSampleId",
                table: "PowerUsageSamples");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PowerUsageSamples",
                table: "PowerUsageSamples",
                column: "SampleDate");
        }
    }
}
