using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KachnaOnline.Data.Migrations
{
    public partial class PushNotifications : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PushSubscriptions",
                columns: table => new
                {
                    Endpoint = table.Column<string>(type: "text", nullable: false),
                    MadeById = table.Column<int>(type: "integer", nullable: true),
                    StateChangesEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    BoardGamesEnabled = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PushSubscriptions", x => x.Endpoint);
                    table.ForeignKey(
                        name: "FK_PushSubscriptions_Users_MadeById",
                        column: x => x.MadeById,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PushSubscriptionKey",
                columns: table => new
                {
                    Endpoint = table.Column<string>(type: "text", nullable: false),
                    KeyType = table.Column<string>(type: "text", nullable: false),
                    KeyValue = table.Column<string>(type: "text", nullable: false),
                    PushSubscriptionEndpoint = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PushSubscriptionKey", x => new { x.Endpoint, x.KeyType });
                    table.ForeignKey(
                        name: "FK_PushSubscriptionKey_PushSubscriptions_PushSubscriptionEndpo~",
                        column: x => x.PushSubscriptionEndpoint,
                        principalTable: "PushSubscriptions",
                        principalColumn: "Endpoint",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PushSubscriptionKey_PushSubscriptionEndpoint",
                table: "PushSubscriptionKey",
                column: "PushSubscriptionEndpoint");

            migrationBuilder.CreateIndex(
                name: "IX_PushSubscriptions_MadeById",
                table: "PushSubscriptions",
                column: "MadeById");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PushSubscriptionKey");

            migrationBuilder.DropTable(
                name: "PushSubscriptions");
        }
    }
}
