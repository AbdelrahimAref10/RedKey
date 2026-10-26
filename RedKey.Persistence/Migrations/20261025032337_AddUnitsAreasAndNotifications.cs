using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RedKey.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddUnitsAreasAndNotifications : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateSequence<int>(
                name: "RK_UnitCodeSequence",
                startValue: 1000L);

            migrationBuilder.CreateTable(
                name: "RK_Areas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NameAr = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    NameEn = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    SortOrder = table.Column<int>(type: "int", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RK_Areas", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RK_Notifications",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RecipientUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Type = table.Column<byte>(type: "tinyint", nullable: false),
                    TitleAr = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    TitleEn = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    BodyAr = table.Column<string>(type: "nvarchar(1500)", maxLength: 1500, nullable: false),
                    BodyEn = table.Column<string>(type: "nvarchar(1500)", maxLength: 1500, nullable: false),
                    LinkUrl = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    EntityId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsRead = table.Column<bool>(type: "bit", nullable: false),
                    ReadAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RK_Notifications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RK_Notifications_RK_Users_RecipientUserId",
                        column: x => x.RecipientUserId,
                        principalTable: "RK_Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RK_UnitTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NameAr = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    NameEn = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    IconUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    SortOrder = table.Column<int>(type: "int", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RK_UnitTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RK_Units",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false, defaultValueSql: "CONCAT('RK-', NEXT VALUE FOR RK_UnitCodeSequence)"),
                    UnitTypeId = table.Column<int>(type: "int", nullable: false),
                    AreaId = table.Column<int>(type: "int", nullable: false),
                    Purpose = table.Column<byte>(type: "tinyint", nullable: false),
                    TitleAr = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    TitleEn = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    CompoundAr = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    CompoundEn = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Bedrooms = table.Column<int>(type: "int", nullable: false),
                    Bathrooms = table.Column<int>(type: "int", nullable: false),
                    Size = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: false),
                    Finishing = table.Column<byte>(type: "tinyint", nullable: false),
                    CoverImageUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    IsFeatured = table.Column<bool>(type: "bit", nullable: false),
                    BrokerCommission = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: false),
                    Status = table.Column<byte>(type: "tinyint", nullable: false),
                    ReviewNote = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    CreatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedByRole = table.Column<int>(type: "int", nullable: false),
                    ReviewedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ReviewedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RK_Units", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RK_Units_RK_Areas_AreaId",
                        column: x => x.AreaId,
                        principalTable: "RK_Areas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RK_Units_RK_UnitTypes_UnitTypeId",
                        column: x => x.UnitTypeId,
                        principalTable: "RK_UnitTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RK_Units_RK_Users_CreatedByUserId",
                        column: x => x.CreatedByUserId,
                        principalTable: "RK_Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RK_UnitDetails",
                columns: table => new
                {
                    UnitId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DescriptionAr = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
                    DescriptionEn = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
                    DeliveryAr = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    DeliveryEn = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RK_UnitDetails", x => x.UnitId);
                    table.ForeignKey(
                        name: "FK_RK_UnitDetails_RK_Units_UnitId",
                        column: x => x.UnitId,
                        principalTable: "RK_Units",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RK_UnitFeatures",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UnitId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    NameAr = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    NameEn = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    SortOrder = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RK_UnitFeatures", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RK_UnitFeatures_RK_Units_UnitId",
                        column: x => x.UnitId,
                        principalTable: "RK_Units",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RK_UnitImages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UnitId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Url = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    SortOrder = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RK_UnitImages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RK_UnitImages_RK_Units_UnitId",
                        column: x => x.UnitId,
                        principalTable: "RK_Units",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RK_UnitPaymentPlans",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UnitId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LabelAr = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    LabelEn = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    ValueAr = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    ValueEn = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    SortOrder = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RK_UnitPaymentPlans", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RK_UnitPaymentPlans_RK_Units_UnitId",
                        column: x => x.UnitId,
                        principalTable: "RK_Units",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RK_ViewingRequests",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UnitId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CustomerUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    PhoneNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    PreferredTime = table.Column<byte>(type: "tinyint", nullable: false),
                    Status = table.Column<byte>(type: "tinyint", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RK_ViewingRequests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RK_ViewingRequests_RK_Units_UnitId",
                        column: x => x.UnitId,
                        principalTable: "RK_Units",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RK_ViewingRequests_RK_Users_CustomerUserId",
                        column: x => x.CustomerUserId,
                        principalTable: "RK_Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RK_Areas_IsActive_SortOrder",
                table: "RK_Areas",
                columns: new[] { "IsActive", "SortOrder" });

            migrationBuilder.CreateIndex(
                name: "IX_RK_Notifications_RecipientUserId_IsRead_CreatedAtUtc",
                table: "RK_Notifications",
                columns: new[] { "RecipientUserId", "IsRead", "CreatedAtUtc" });

            migrationBuilder.CreateIndex(
                name: "IX_RK_UnitFeatures_UnitId_SortOrder",
                table: "RK_UnitFeatures",
                columns: new[] { "UnitId", "SortOrder" });

            migrationBuilder.CreateIndex(
                name: "IX_RK_UnitImages_UnitId_SortOrder",
                table: "RK_UnitImages",
                columns: new[] { "UnitId", "SortOrder" });

            migrationBuilder.CreateIndex(
                name: "IX_RK_UnitPaymentPlans_UnitId_SortOrder",
                table: "RK_UnitPaymentPlans",
                columns: new[] { "UnitId", "SortOrder" });

            migrationBuilder.CreateIndex(
                name: "IX_RK_Units_AreaId",
                table: "RK_Units",
                column: "AreaId");

            migrationBuilder.CreateIndex(
                name: "IX_RK_Units_Code",
                table: "RK_Units",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RK_Units_CreatedByUserId",
                table: "RK_Units",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_RK_Units_Status_Purpose_UnitTypeId_AreaId",
                table: "RK_Units",
                columns: new[] { "Status", "Purpose", "UnitTypeId", "AreaId" });

            migrationBuilder.CreateIndex(
                name: "IX_RK_Units_UnitTypeId",
                table: "RK_Units",
                column: "UnitTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_RK_UnitTypes_IsActive_SortOrder",
                table: "RK_UnitTypes",
                columns: new[] { "IsActive", "SortOrder" });

            migrationBuilder.CreateIndex(
                name: "IX_RK_ViewingRequests_CustomerUserId",
                table: "RK_ViewingRequests",
                column: "CustomerUserId");

            migrationBuilder.CreateIndex(
                name: "IX_RK_ViewingRequests_Status_CreatedAtUtc",
                table: "RK_ViewingRequests",
                columns: new[] { "Status", "CreatedAtUtc" });

            migrationBuilder.CreateIndex(
                name: "IX_RK_ViewingRequests_UnitId",
                table: "RK_ViewingRequests",
                column: "UnitId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RK_Notifications");

            migrationBuilder.DropTable(
                name: "RK_UnitDetails");

            migrationBuilder.DropTable(
                name: "RK_UnitFeatures");

            migrationBuilder.DropTable(
                name: "RK_UnitImages");

            migrationBuilder.DropTable(
                name: "RK_UnitPaymentPlans");

            migrationBuilder.DropTable(
                name: "RK_ViewingRequests");

            migrationBuilder.DropTable(
                name: "RK_Units");

            migrationBuilder.DropTable(
                name: "RK_Areas");

            migrationBuilder.DropTable(
                name: "RK_UnitTypes");

            migrationBuilder.DropSequence(
                name: "RK_UnitCodeSequence");
        }
    }
}
