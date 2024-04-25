using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace backend.Migrations
{
    public partial class CorrectingStableDiffusionDbColumns : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Panorama",
                table: "Images");

            migrationBuilder.AddColumn<int>(
                name: "InferenceDenoisingSteps",
                table: "Images",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Samples",
                table: "Images",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "Seed",
                table: "Images",
                type: "bigint",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "InferenceDenoisingSteps",
                table: "Images");

            migrationBuilder.DropColumn(
                name: "Samples",
                table: "Images");

            migrationBuilder.DropColumn(
                name: "Seed",
                table: "Images");

            migrationBuilder.AddColumn<bool>(
                name: "Panorama",
                table: "Images",
                type: "bit",
                nullable: true);
        }
    }
}
