using Microsoft.EntityFrameworkCore.Migrations;

namespace KachnaOnline.Data.Migrations
{
    public partial class AddForceDisableUserRole : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "ForceDisable",
                table: "UserRole",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ForceDisable",
                table: "UserRole");
        }
    }
}
