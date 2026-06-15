using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RivenBackend.Migrations
{
    /// <inheritdoc />
    public partial class UpdateSymptoms : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ArmWeakness",
                table: "Symptoms");

            migrationBuilder.DropColumn(
                name: "BalanceLoss",
                table: "Symptoms");

            migrationBuilder.DropColumn(
                name: "FacialDroop",
                table: "Symptoms");

            migrationBuilder.DropColumn(
                name: "SevereHeadache",
                table: "Symptoms");

            migrationBuilder.DropColumn(
                name: "SpeechDifficulty",
                table: "Symptoms");

            migrationBuilder.DropColumn(
                name: "VisionLoss",
                table: "Symptoms");

            migrationBuilder.AddColumn<string>(
                name: "AdditionalNotes",
                table: "Symptoms",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SelectedSymptoms",
                table: "Symptoms",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AdditionalNotes",
                table: "Symptoms");

            migrationBuilder.DropColumn(
                name: "SelectedSymptoms",
                table: "Symptoms");

            migrationBuilder.AddColumn<bool>(
                name: "ArmWeakness",
                table: "Symptoms",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "BalanceLoss",
                table: "Symptoms",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "FacialDroop",
                table: "Symptoms",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "SevereHeadache",
                table: "Symptoms",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "SpeechDifficulty",
                table: "Symptoms",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "VisionLoss",
                table: "Symptoms",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
