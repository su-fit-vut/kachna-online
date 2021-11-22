using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KachnaOnline.Data.Migrations
{
    public partial class AddNotificationControls : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "NotifiedBeforeExpiration",
                table: "BoardGameReservationItems",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "NotifiedOnExpiration",
                table: "BoardGameReservationItems",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NotifiedBeforeExpiration",
                table: "BoardGameReservationItems");

            migrationBuilder.DropColumn(
                name: "NotifiedOnExpiration",
                table: "BoardGameReservationItems");
        }
    }
}
