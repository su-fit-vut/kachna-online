using Microsoft.EntityFrameworkCore.Migrations;

namespace KachnaOnline.Data.Migrations
{
    public partial class AddBoardGamesFK : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_BoardGameReservationItems_BoardGameId",
                table: "BoardGameReservationItems",
                column: "BoardGameId");

            migrationBuilder.AddForeignKey(
                name: "FK_BoardGameReservationItems_BoardGames_BoardGameId",
                table: "BoardGameReservationItems",
                column: "BoardGameId",
                principalTable: "BoardGames",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BoardGameReservationItems_BoardGames_BoardGameId",
                table: "BoardGameReservationItems");

            migrationBuilder.DropIndex(
                name: "IX_BoardGameReservationItems_BoardGameId",
                table: "BoardGameReservationItems");
        }
    }
}
