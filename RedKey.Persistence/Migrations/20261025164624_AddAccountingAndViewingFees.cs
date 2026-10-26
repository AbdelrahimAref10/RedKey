using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace RedKey.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddAccountingAndViewingFees : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateSequence<int>(
                name: "RK_JournalNumberSequence",
                startValue: 100001L);

            migrationBuilder.AddColumn<DateTime>(
                name: "CompletedAtUtc",
                table: "RK_ViewingRequests",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "JournalEntryId",
                table: "RK_ViewingRequests",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "ViewingFee",
                table: "RK_ViewingRequests",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "ViewingFee",
                table: "RK_Units",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.CreateTable(
                name: "RK_Accounts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    Code = table.Column<string>(type: "varchar(10)", unicode: false, maxLength: 10, nullable: false),
                    NameAr = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    NameEn = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Type = table.Column<byte>(type: "tinyint", nullable: false),
                    IsCash = table.Column<bool>(type: "bit", nullable: false),
                    IsSystem = table.Column<bool>(type: "bit", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RK_Accounts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RK_JournalEntries",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Number = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: false, defaultValueSql: "CONCAT('JE-', NEXT VALUE FOR RK_JournalNumberSequence)"),
                    EntryDateUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DescriptionAr = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    DescriptionEn = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    SourceType = table.Column<byte>(type: "tinyint", nullable: false),
                    SourceId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UnitId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ReversalOfId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsReversed = table.Column<bool>(type: "bit", nullable: false),
                    CreatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RK_JournalEntries", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RK_JournalLines",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EntryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AccountId = table.Column<int>(type: "int", nullable: false),
                    Debit = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Credit = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Memo = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RK_JournalLines", x => x.Id);
                    table.CheckConstraint("CK_RK_JournalLines_Amounts", "[Debit] >= 0 AND [Credit] >= 0");
                    table.CheckConstraint("CK_RK_JournalLines_OneSide", "([Debit] > 0 AND [Credit] = 0) OR ([Credit] > 0 AND [Debit] = 0)");
                    table.ForeignKey(
                        name: "FK_RK_JournalLines_RK_Accounts_AccountId",
                        column: x => x.AccountId,
                        principalTable: "RK_Accounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RK_JournalLines_RK_JournalEntries_EntryId",
                        column: x => x.EntryId,
                        principalTable: "RK_JournalEntries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "RK_Accounts",
                columns: new[] { "Id", "Code", "IsActive", "IsCash", "IsSystem", "NameAr", "NameEn", "Type" },
                values: new object[,]
                {
                    { 1, "1100", true, true, true, "الخزنة النقدية", "Cash box", (byte)1 },
                    { 2, "1200", true, true, true, "البنك", "Bank", (byte)1 },
                    { 3, "1300", true, false, true, "مستحقات على العملاء", "Customer receivables", (byte)1 },
                    { 4, "2100", true, false, true, "عمولات وسطاء مستحقة", "Broker commissions payable", (byte)2 },
                    { 5, "4100", true, false, true, "إيراد رسوم المعاينة", "Viewing fee revenue", (byte)4 },
                    { 6, "4200", true, false, true, "إيراد عمولة بيع", "Sale commission revenue", (byte)4 },
                    { 7, "4300", true, false, true, "إيراد عمولة إيجار", "Rent commission revenue", (byte)4 },
                    { 8, "5100", true, false, true, "مصروف عمولات الوسطاء", "Broker commission expense", (byte)5 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_RK_Accounts_Code",
                table: "RK_Accounts",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RK_JournalEntries_EntryDateUtc",
                table: "RK_JournalEntries",
                column: "EntryDateUtc");

            migrationBuilder.CreateIndex(
                name: "IX_RK_JournalEntries_Number",
                table: "RK_JournalEntries",
                column: "Number",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RK_JournalEntries_SourceType_SourceId",
                table: "RK_JournalEntries",
                columns: new[] { "SourceType", "SourceId" },
                unique: true,
                filter: "[ReversalOfId] IS NULL AND [IsReversed] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_RK_JournalEntries_UnitId",
                table: "RK_JournalEntries",
                column: "UnitId");

            migrationBuilder.CreateIndex(
                name: "IX_RK_JournalLines_AccountId_EntryId",
                table: "RK_JournalLines",
                columns: new[] { "AccountId", "EntryId" });

            migrationBuilder.CreateIndex(
                name: "IX_RK_JournalLines_EntryId",
                table: "RK_JournalLines",
                column: "EntryId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RK_JournalLines");

            migrationBuilder.DropTable(
                name: "RK_Accounts");

            migrationBuilder.DropTable(
                name: "RK_JournalEntries");

            migrationBuilder.DropColumn(
                name: "CompletedAtUtc",
                table: "RK_ViewingRequests");

            migrationBuilder.DropColumn(
                name: "JournalEntryId",
                table: "RK_ViewingRequests");

            migrationBuilder.DropColumn(
                name: "ViewingFee",
                table: "RK_ViewingRequests");

            migrationBuilder.DropColumn(
                name: "ViewingFee",
                table: "RK_Units");

            migrationBuilder.DropSequence(
                name: "RK_JournalNumberSequence");
        }
    }
}
