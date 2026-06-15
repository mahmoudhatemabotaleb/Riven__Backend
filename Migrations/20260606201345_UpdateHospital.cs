using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RivenBackend.Migrations
{
    /// <inheritdoc />
    public partial class UpdateHospital : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AvailableStrokeBeds",
                table: "Hospitals",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "EmergencyBayCleared",
                table: "Hospitals",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<double>(
                name: "Latitude",
                table: "Hospitals",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "Longitude",
                table: "Hospitals",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<bool>(
                name: "NeurologistOnStandby",
                table: "Hospitals",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "StrokeTeamNotified",
                table: "Hospitals",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "WaitTimeMinutes",
                table: "Hospitals",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AvailableStrokeBeds",
                table: "Hospitals");

            migrationBuilder.DropColumn(
                name: "EmergencyBayCleared",
                table: "Hospitals");

            migrationBuilder.DropColumn(
                name: "Latitude",
                table: "Hospitals");

            migrationBuilder.DropColumn(
                name: "Longitude",
                table: "Hospitals");

            migrationBuilder.DropColumn(
                name: "NeurologistOnStandby",
                table: "Hospitals");

            migrationBuilder.DropColumn(
                name: "StrokeTeamNotified",
                table: "Hospitals");

            migrationBuilder.DropColumn(
                name: "WaitTimeMinutes",
                table: "Hospitals");
        }
    }
}
