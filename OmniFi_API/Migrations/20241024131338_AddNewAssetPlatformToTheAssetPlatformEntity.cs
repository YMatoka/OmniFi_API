using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OmniFi_API.Migrations
{
    /// <inheritdoc />
    public partial class AddNewAssetPlatformToTheAssetPlatformEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AssetPlatforms",
                columns: new[] { "AssetPlatformID", "BankID", "CryptoExchangeID" },
                values: new object[] { 5, 2, null });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AssetPlatforms",
                keyColumn: "AssetPlatformID",
                keyValue: 5);
        }
    }
}
