using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RedKey.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddLookupsSoftDelete : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "RK_UnitTypes",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "RK_Areas",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "RK_UnitTypes");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "RK_Areas");
        }
    }
}
