using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TourApi.Migrations
{
    /// <inheritdoc />
    public partial class AddAssignedTravelAgentToTour : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AssignedTravelAgentId",
                table: "Tours",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Tours_AssignedTravelAgentId",
                table: "Tours",
                column: "AssignedTravelAgentId");

            migrationBuilder.AddForeignKey(
                name: "FK_Tours_Employees_AssignedTravelAgentId",
                table: "Tours",
                column: "AssignedTravelAgentId",
                principalTable: "Employees",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tours_Employees_AssignedTravelAgentId",
                table: "Tours");

            migrationBuilder.DropIndex(
                name: "IX_Tours_AssignedTravelAgentId",
                table: "Tours");

            migrationBuilder.DropColumn(
                name: "AssignedTravelAgentId",
                table: "Tours");
        }
    }
}