using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RivenBackend.Migrations
{
    /// <inheritdoc />
    public partial class UpdateAmbulance : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "CurrentLatitude",
                table: "Ambulances",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "CurrentLongitude",
                table: "Ambulances",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "DistanceMiles",
                table: "Ambulances",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "EtaMinutes",
                table: "Ambulances",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CurrentLatitude",
                table: "Ambulances");

            migrationBuilder.DropColumn(
                name: "CurrentLongitude",
                table: "Ambulances");

            migrationBuilder.DropColumn(
                name: "DistanceMiles",
                table: "Ambulances");

            migrationBuilder.DropColumn(
                name: "EtaMinutes",
                table: "Ambulances");
        }
    }
}
