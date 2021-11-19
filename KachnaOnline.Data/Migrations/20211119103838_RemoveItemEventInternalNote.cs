using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KachnaOnline.Data.Migrations
{
    public partial class RemoveItemEventInternalNote : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NoteInternal",
                table: "BoardGameReservationItemEvents");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "NoteInternal",
                table: "BoardGameReservationItemEvents",
                type: "character varying(1024)",
                maxLength: 1024,
                nullable: true);
        }
    }
}
