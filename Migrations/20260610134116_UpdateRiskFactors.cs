using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RivenBackend.Migrations
{
    /// <inheritdoc />
    public partial class UpdateRiskFactors : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "SmokingStatus",
                table: "RiskFactors",
                newName: "Smoking");

            migrationBuilder.RenameColumn(
                name: "AtrialFibrillation",
                table: "RiskFactors",
                newName: "SleepApnea");

            migrationBuilder.AddColumn<bool>(
                name: "HighCholesterol",
                table: "RiskFactors",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Hypertension",
                table: "RiskFactors",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Obesity",
                table: "RiskFactors",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "PhysicalInactive",
                table: "RiskFactors",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HighCholesterol",
                table: "RiskFactors");

            migrationBuilder.DropColumn(
                name: "Hypertension",
                table: "RiskFactors");

            migrationBuilder.DropColumn(
                name: "Obesity",
                table: "RiskFactors");

            migrationBuilder.DropColumn(
                name: "PhysicalInactive",
                table: "RiskFactors");

            migrationBuilder.RenameColumn(
                name: "Smoking",
                table: "RiskFactors",
                newName: "SmokingStatus");

            migrationBuilder.RenameColumn(
                name: "SleepApnea",
                table: "RiskFactors",
                newName: "AtrialFibrillation");
        }
    }
}
