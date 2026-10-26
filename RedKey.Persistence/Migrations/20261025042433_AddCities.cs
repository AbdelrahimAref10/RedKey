using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RedKey.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddCities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_RK_Units_Status_Purpose_UnitTypeId_AreaId",
                table: "RK_Units");

            migrationBuilder.DropIndex(
                name: "IX_RK_Areas_IsActive_SortOrder",
                table: "RK_Areas");

            migrationBuilder.AddColumn<int>(
                name: "CityId",
                table: "RK_Units",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "CityId",
                table: "RK_Areas",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "RK_Cities",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NameAr = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    NameEn = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    SortOrder = table.Column<int>(type: "int", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RK_Cities", x => x.Id);
                });

            // Existing data: every area so far was really a city → it becomes a city with the same name,
            // the area moves under it, and its units get that city.
            migrationBuilder.Sql(@"
CREATE TABLE #AreaCity (AreaId int NOT NULL, CityId int NOT NULL);

MERGE RK_Cities AS t
USING (SELECT Id, NameAr, NameEn, IsActive, SortOrder, IsDeleted, CreatedAtUtc FROM RK_Areas) AS s
ON 1 = 0
WHEN NOT MATCHED THEN
    INSERT (NameAr, NameEn, IsActive, SortOrder, IsDeleted, CreatedAtUtc)
    VALUES (s.NameAr, s.NameEn, s.IsActive, s.SortOrder, s.IsDeleted, s.CreatedAtUtc)
OUTPUT s.Id, inserted.Id INTO #AreaCity (AreaId, CityId);

UPDATE a SET a.CityId = m.CityId FROM RK_Areas a JOIN #AreaCity m ON m.AreaId = a.Id;
UPDATE u SET u.CityId = a.CityId FROM RK_Units u JOIN RK_Areas a ON a.Id = u.AreaId;

DROP TABLE #AreaCity;");

            migrationBuilder.CreateIndex(
                name: "IX_RK_Units_CityId",
                table: "RK_Units",
                column: "CityId");

            migrationBuilder.CreateIndex(
                name: "IX_RK_Units_Status_Purpose_UnitTypeId_CityId_AreaId",
                table: "RK_Units",
                columns: new[] { "Status", "Purpose", "UnitTypeId", "CityId", "AreaId" });

            migrationBuilder.CreateIndex(
                name: "IX_RK_Areas_CityId_IsActive_SortOrder",
                table: "RK_Areas",
                columns: new[] { "CityId", "IsActive", "SortOrder" });

            migrationBuilder.CreateIndex(
                name: "IX_RK_Cities_IsActive_SortOrder",
                table: "RK_Cities",
                columns: new[] { "IsActive", "SortOrder" });

            migrationBuilder.AddForeignKey(
                name: "FK_RK_Areas_RK_Cities_CityId",
                table: "RK_Areas",
                column: "CityId",
                principalTable: "RK_Cities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_RK_Units_RK_Cities_CityId",
                table: "RK_Units",
                column: "CityId",
                principalTable: "RK_Cities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RK_Areas_RK_Cities_CityId",
                table: "RK_Areas");

            migrationBuilder.DropForeignKey(
                name: "FK_RK_Units_RK_Cities_CityId",
                table: "RK_Units");

            migrationBuilder.DropTable(
                name: "RK_Cities");

            migrationBuilder.DropIndex(
                name: "IX_RK_Units_CityId",
                table: "RK_Units");

            migrationBuilder.DropIndex(
                name: "IX_RK_Units_Status_Purpose_UnitTypeId_CityId_AreaId",
                table: "RK_Units");

            migrationBuilder.DropIndex(
                name: "IX_RK_Areas_CityId_IsActive_SortOrder",
                table: "RK_Areas");

            migrationBuilder.DropColumn(
                name: "CityId",
                table: "RK_Units");

            migrationBuilder.DropColumn(
                name: "CityId",
                table: "RK_Areas");

            migrationBuilder.CreateIndex(
                name: "IX_RK_Units_Status_Purpose_UnitTypeId_AreaId",
                table: "RK_Units",
                columns: new[] { "Status", "Purpose", "UnitTypeId", "AreaId" });

            migrationBuilder.CreateIndex(
                name: "IX_RK_Areas_IsActive_SortOrder",
                table: "RK_Areas",
                columns: new[] { "IsActive", "SortOrder" });
        }
    }
}
