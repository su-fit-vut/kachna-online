using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace KachnaOnline.Data.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BoardGameCategories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    ColourHex = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BoardGameCategories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false),
                    Email = table.Column<string>(type: "character varying(320)", maxLength: 320, nullable: false),
                    Name = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    Nickname = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    DiscordId = table.Column<decimal>(type: "numeric(20,0)", nullable: true),
                    Disabled = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BoardGameReservations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    MadeById = table.Column<int>(type: "integer", nullable: false),
                    MadeOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    NoteUser = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: true),
                    NoteInternal = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BoardGameReservations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BoardGameReservations_Users_MadeById",
                        column: x => x.MadeById,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BoardGames",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    ImageUrl = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: true),
                    PlayersMin = table.Column<int>(type: "integer", nullable: true),
                    PlayersMax = table.Column<int>(type: "integer", nullable: true),
                    CategoryId = table.Column<int>(type: "integer", nullable: false),
                    NoteInternal = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: true),
                    OwnerId = table.Column<int>(type: "integer", nullable: true),
                    InStock = table.Column<int>(type: "integer", nullable: false),
                    Unavailable = table.Column<int>(type: "integer", nullable: false),
                    Visible = table.Column<bool>(type: "boolean", nullable: false),
                    DefaultReservationTime = table.Column<TimeSpan>(type: "interval", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BoardGames", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BoardGames_BoardGameCategories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "BoardGameCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BoardGames_Users_OwnerId",
                        column: x => x.OwnerId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "Events",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    MadeById = table.Column<int>(type: "integer", nullable: false),
                    Name = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    Place = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    PlaceUrl = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: true),
                    ImageUrl = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: true),
                    ShortDescription = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: false),
                    FullDescription = table.Column<string>(type: "text", nullable: true),
                    Url = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: true),
                    From = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    To = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Events", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Events_Users_MadeById",
                        column: x => x.MadeById,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RepeatingStates",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    MadeById = table.Column<int>(type: "integer", nullable: false),
                    State = table.Column<int>(type: "integer", nullable: false),
                    DayOfWeek = table.Column<int>(type: "integer", nullable: false),
                    EffectiveFrom = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    EffectiveTo = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    TimeFrom = table.Column<TimeSpan>(type: "interval", nullable: false),
                    TimeTo = table.Column<TimeSpan>(type: "interval", nullable: false),
                    NoteInternal = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: true),
                    NotePublic = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RepeatingStates", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RepeatingStates_Users_MadeById",
                        column: x => x.MadeById,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UserRole",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    RoleId = table.Column<int>(type: "integer", nullable: false),
                    ForceDisable = table.Column<bool>(type: "boolean", nullable: false),
                    AssignedByUserId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserRole", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_UserRole_Roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserRole_Users_AssignedByUserId",
                        column: x => x.AssignedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserRole_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BoardGameReservationItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ReservationId = table.Column<int>(type: "integer", nullable: false),
                    BoardGameId = table.Column<int>(type: "integer", nullable: false),
                    ExpiresOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BoardGameReservationItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BoardGameReservationItems_BoardGameReservations_Reservation~",
                        column: x => x.ReservationId,
                        principalTable: "BoardGameReservations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BoardGameReservationItems_BoardGames_BoardGameId",
                        column: x => x.BoardGameId,
                        principalTable: "BoardGames",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PlannedStates",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    MadeById = table.Column<int>(type: "integer", nullable: false),
                    Start = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    PlannedEnd = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    State = table.Column<int>(type: "integer", nullable: false),
                    Ended = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    ClosedById = table.Column<int>(type: "integer", nullable: true),
                    NoteInternal = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: true),
                    NotePublic = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: true),
                    NextPlannedStateId = table.Column<int>(type: "integer", nullable: true),
                    RepeatingStateId = table.Column<int>(type: "integer", nullable: true),
                    AssociatedEventId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlannedStates", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PlannedStates_Events_AssociatedEventId",
                        column: x => x.AssociatedEventId,
                        principalTable: "Events",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_PlannedStates_PlannedStates_NextPlannedStateId",
                        column: x => x.NextPlannedStateId,
                        principalTable: "PlannedStates",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_PlannedStates_RepeatingStates_RepeatingStateId",
                        column: x => x.RepeatingStateId,
                        principalTable: "RepeatingStates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PlannedStates_Users_ClosedById",
                        column: x => x.ClosedById,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PlannedStates_Users_MadeById",
                        column: x => x.MadeById,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "BoardGameReservationItemEvents",
                columns: table => new
                {
                    ReservationItemId = table.Column<int>(type: "integer", nullable: false),
                    MadeOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    MadeById = table.Column<int>(type: "integer", nullable: false),
                    NewState = table.Column<int>(type: "integer", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    NewExpiryDateTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    NoteInternal = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BoardGameReservationItemEvents", x => new { x.ReservationItemId, x.MadeOn });
                    table.ForeignKey(
                        name: "FK_BoardGameReservationItemEvents_BoardGameReservationItems_Re~",
                        column: x => x.ReservationItemId,
                        principalTable: "BoardGameReservationItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BoardGameReservationItemEvents_Users_MadeById",
                        column: x => x.MadeById,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BoardGameReservationItemEvents_MadeById",
                table: "BoardGameReservationItemEvents",
                column: "MadeById");

            migrationBuilder.CreateIndex(
                name: "IX_BoardGameReservationItems_BoardGameId",
                table: "BoardGameReservationItems",
                column: "BoardGameId");

            migrationBuilder.CreateIndex(
                name: "IX_BoardGameReservationItems_ReservationId",
                table: "BoardGameReservationItems",
                column: "ReservationId");

            migrationBuilder.CreateIndex(
                name: "IX_BoardGameReservations_MadeById",
                table: "BoardGameReservations",
                column: "MadeById");

            migrationBuilder.CreateIndex(
                name: "IX_BoardGames_CategoryId",
                table: "BoardGames",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_BoardGames_OwnerId",
                table: "BoardGames",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_Events_MadeById",
                table: "Events",
                column: "MadeById");

            migrationBuilder.CreateIndex(
                name: "IX_PlannedStates_AssociatedEventId",
                table: "PlannedStates",
                column: "AssociatedEventId");

            migrationBuilder.CreateIndex(
                name: "IX_PlannedStates_ClosedById",
                table: "PlannedStates",
                column: "ClosedById");

            migrationBuilder.CreateIndex(
                name: "IX_PlannedStates_MadeById",
                table: "PlannedStates",
                column: "MadeById");

            migrationBuilder.CreateIndex(
                name: "IX_PlannedStates_NextPlannedStateId",
                table: "PlannedStates",
                column: "NextPlannedStateId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PlannedStates_RepeatingStateId",
                table: "PlannedStates",
                column: "RepeatingStateId");

            migrationBuilder.CreateIndex(
                name: "IX_RepeatingStates_MadeById",
                table: "RepeatingStates",
                column: "MadeById");

            migrationBuilder.CreateIndex(
                name: "IX_UserRole_AssignedByUserId",
                table: "UserRole",
                column: "AssignedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserRole_RoleId",
                table: "UserRole",
                column: "RoleId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BoardGameReservationItemEvents");

            migrationBuilder.DropTable(
                name: "PlannedStates");

            migrationBuilder.DropTable(
                name: "UserRole");

            migrationBuilder.DropTable(
                name: "BoardGameReservationItems");

            migrationBuilder.DropTable(
                name: "Events");

            migrationBuilder.DropTable(
                name: "RepeatingStates");

            migrationBuilder.DropTable(
                name: "Roles");

            migrationBuilder.DropTable(
                name: "BoardGameReservations");

            migrationBuilder.DropTable(
                name: "BoardGames");

            migrationBuilder.DropTable(
                name: "BoardGameCategories");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
