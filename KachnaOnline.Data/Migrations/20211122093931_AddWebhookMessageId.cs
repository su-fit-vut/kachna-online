using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KachnaOnline.Data.Migrations
{
    public partial class AddWebhookMessageId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "WebhookMessageId",
                table: "BoardGameReservations",
                type: "numeric(20,0)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "WebhookMessageId",
                table: "BoardGameReservations");
        }
    }
}
