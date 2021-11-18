using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KachnaOnline.Data.Migrations
{
    public partial class NextPlannedStateDeleteBehavior : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PlannedStates_PlannedStates_NextPlannedStateId",
                table: "PlannedStates");

            migrationBuilder.AddForeignKey(
                name: "FK_PlannedStates_PlannedStates_NextPlannedStateId",
                table: "PlannedStates",
                column: "NextPlannedStateId",
                principalTable: "PlannedStates",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PlannedStates_PlannedStates_NextPlannedStateId",
                table: "PlannedStates");

            migrationBuilder.AddForeignKey(
                name: "FK_PlannedStates_PlannedStates_NextPlannedStateId",
                table: "PlannedStates",
                column: "NextPlannedStateId",
                principalTable: "PlannedStates",
                principalColumn: "Id");
        }
    }
}
