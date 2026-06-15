using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RivenBackend.Migrations
{
    /// <inheritdoc />
    public partial class UpdateEcgResult : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserId",
                table: "EcgResults");

            migrationBuilder.AddColumn<int>(
                name: "CaseId",
                table: "EcgResults",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_EcgResults_CaseId",
                table: "EcgResults",
                column: "CaseId");

            migrationBuilder.AddForeignKey(
                name: "FK_EcgResults_Cases_CaseId",
                table: "EcgResults",
                column: "CaseId",
                principalTable: "Cases",
                principalColumn: "CaseId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EcgResults_Cases_CaseId",
                table: "EcgResults");

            migrationBuilder.DropIndex(
                name: "IX_EcgResults_CaseId",
                table: "EcgResults");

            migrationBuilder.DropColumn(
                name: "CaseId",
                table: "EcgResults");

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "EcgResults",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
