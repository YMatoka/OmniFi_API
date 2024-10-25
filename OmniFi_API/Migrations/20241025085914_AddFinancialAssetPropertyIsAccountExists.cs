using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OmniFi_API.Migrations
{
    /// <inheritdoc />
    public partial class AddFinancialAssetPropertyIsAccountExists : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsAccountExists",
                table: "FinancialAssets",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsAccountExists",
                table: "FinancialAssets");
        }
    }
}
