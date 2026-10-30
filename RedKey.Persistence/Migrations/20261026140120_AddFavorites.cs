using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RedKey.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddFavorites : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RK_Favorites",
                columns: table => new
                {
                    CustomerUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UnitId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RK_Favorites", x => new { x.CustomerUserId, x.UnitId });
                    table.ForeignKey(
                        name: "FK_RK_Favorites_RK_Units_UnitId",
                        column: x => x.UnitId,
                        principalTable: "RK_Units",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RK_Favorites_RK_Users_CustomerUserId",
                        column: x => x.CustomerUserId,
                        principalTable: "RK_Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RK_Favorites_CustomerUserId_CreatedAtUtc",
                table: "RK_Favorites",
                columns: new[] { "CustomerUserId", "CreatedAtUtc" });

            migrationBuilder.CreateIndex(
                name: "IX_RK_Favorites_UnitId",
                table: "RK_Favorites",
                column: "UnitId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RK_Favorites");
        }
    }
}
