using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RivenBackend.Migrations
{
    /// <inheritdoc />
    public partial class AddCaseTransportFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "ArrivedTime",
                table: "Cases",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "LocationLatitude",
                table: "Cases",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "LocationLongitude",
                table: "Cases",
                type: "float",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ArrivedTime",
                table: "Cases");

            migrationBuilder.DropColumn(
                name: "LocationLatitude",
                table: "Cases");

            migrationBuilder.DropColumn(
                name: "LocationLongitude",
                table: "Cases");
        }
    }
}
