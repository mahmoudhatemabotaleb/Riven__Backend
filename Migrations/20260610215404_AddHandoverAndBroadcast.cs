using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RivenBackend.Migrations
{
    /// <inheritdoc />
    public partial class AddHandoverAndBroadcast : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "HandoverNotes",
                table: "Cases",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "HandoverTime",
                table: "Cases",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PatientConditionOnArrival",
                table: "Cases",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ReceivingPhysician",
                table: "Cases",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HandoverNotes",
                table: "Cases");

            migrationBuilder.DropColumn(
                name: "HandoverTime",
                table: "Cases");

            migrationBuilder.DropColumn(
                name: "PatientConditionOnArrival",
                table: "Cases");

            migrationBuilder.DropColumn(
                name: "ReceivingPhysician",
                table: "Cases");
        }
    }
}
