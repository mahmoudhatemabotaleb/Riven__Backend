using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RivenBackend.Migrations
{
    /// <inheritdoc />
    public partial class UpdateVitalSigns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BloodPressure",
                table: "VitalSigns");

            migrationBuilder.AddColumn<int>(
                name: "DiastolicBP",
                table: "VitalSigns",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<double>(
                name: "RespiratoryRate",
                table: "VitalSigns",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<int>(
                name: "SystolicBP",
                table: "VitalSigns",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "TemperatureUnit",
                table: "VitalSigns",
                type: "nvarchar(1)",
                maxLength: 1,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DiastolicBP",
                table: "VitalSigns");

            migrationBuilder.DropColumn(
                name: "RespiratoryRate",
                table: "VitalSigns");

            migrationBuilder.DropColumn(
                name: "SystolicBP",
                table: "VitalSigns");

            migrationBuilder.DropColumn(
                name: "TemperatureUnit",
                table: "VitalSigns");

            migrationBuilder.AddColumn<string>(
                name: "BloodPressure",
                table: "VitalSigns",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");
        }
    }
}
