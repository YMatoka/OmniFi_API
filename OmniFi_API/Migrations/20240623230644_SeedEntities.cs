using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace OmniFi_API.Migrations
{
    /// <inheritdoc />
    public partial class SeedEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "FinancialAssets",
                columns: new[] { "FinancialEntityId", "AssetPlatformID", "AssetSourceID", "FiatCurrencyID", "FirstRetrievedAt", "LastUpdatedAt", "UserID", "Value" },
                values: new object[,]
                {
                    { 1, 1, 1, 2, new DateTime(2024, 6, 23, 23, 6, 43, 378, DateTimeKind.Utc).AddTicks(7567), new DateTime(2024, 6, 23, 23, 6, 43, 378, DateTimeKind.Utc).AddTicks(7569), "67769df3-3d88-437a-8be8-b1729f251b3c", 1000.66m },
                    { 2, 2, 4, 2, new DateTime(2024, 6, 23, 23, 6, 43, 378, DateTimeKind.Utc).AddTicks(7571), new DateTime(2024, 6, 23, 23, 6, 43, 378, DateTimeKind.Utc).AddTicks(7572), "67769df3-3d88-437a-8be8-b1729f251b3c", 23000.66m }
                });

            migrationBuilder.InsertData(
                table: "CryptoHoldings",
                columns: new[] { "CryptoHoldingEntityId", "Amount", "CryptoCurrencyName", "CryptoCurrencySymbol", "FinancialAssetID" },
                values: new object[] { 1, 2.33m, "Bitcoin", "BTC", 2 });

            migrationBuilder.InsertData(
                table: "FinancialAssetsHistory",
                columns: new[] { "FinancialEntityId", "AssetPlatformID", "AssetSourceID", "FiatCurrencyID", "FinancialAssetId", "RecordedAt", "UserID", "Value" },
                values: new object[,]
                {
                    { 1, 1, 1, 2, 1, new DateTime(2024, 6, 23, 23, 6, 43, 379, DateTimeKind.Utc).AddTicks(5803), "67769df3-3d88-437a-8be8-b1729f251b3c", 1000.66m },
                    { 2, 2, 4, 2, 2, new DateTime(2024, 6, 23, 23, 6, 43, 379, DateTimeKind.Utc).AddTicks(5806), "67769df3-3d88-437a-8be8-b1729f251b3c", 23000.66m }
                });

            migrationBuilder.InsertData(
                table: "CryptoHoldingsHystory",
                columns: new[] { "CryptoHoldingEntityId", "Amount", "CryptoCurrencyName", "CryptoCurrencySymbol", "CryptoHoldingId", "FinancialAssetHistoryID" },
                values: new object[] { 1, 2.33m, "Bitcoin", "BTC", 1, 2 });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "CryptoHoldingsHystory",
                keyColumn: "CryptoHoldingEntityId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "FinancialAssetsHistory",
                keyColumn: "FinancialEntityId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "CryptoHoldings",
                keyColumn: "CryptoHoldingEntityId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "FinancialAssets",
                keyColumn: "FinancialEntityId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "FinancialAssetsHistory",
                keyColumn: "FinancialEntityId",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "FinancialAssets",
                keyColumn: "FinancialEntityId",
                keyValue: 2);
        }
    }
}
