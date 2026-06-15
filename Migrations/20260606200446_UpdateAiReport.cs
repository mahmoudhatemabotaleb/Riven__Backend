using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RivenBackend.Migrations
{
    /// <inheritdoc />
    public partial class UpdateAiReport : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AdditionalNotes",
                table: "AiReports",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CtScanResult",
                table: "AiReports",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EcgImageResult",
                table: "AiReports",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EcgSignalResult",
                table: "AiReports",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NihssScore",
                table: "AiReports",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AdditionalNotes",
                table: "AiReports");

            migrationBuilder.DropColumn(
                name: "CtScanResult",
                table: "AiReports");

            migrationBuilder.DropColumn(
                name: "EcgImageResult",
                table: "AiReports");

            migrationBuilder.DropColumn(
                name: "EcgSignalResult",
                table: "AiReports");

            migrationBuilder.DropColumn(
                name: "NihssScore",
                table: "AiReports");
        }
    }
}
