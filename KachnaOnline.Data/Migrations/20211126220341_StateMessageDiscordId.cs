using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KachnaOnline.Data.Migrations
{
    public partial class StateMessageDiscordId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "DiscordNotificationId",
                table: "PlannedStates",
                type: "numeric(20,0)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DiscordNotificationId",
                table: "PlannedStates");
        }
    }
}
