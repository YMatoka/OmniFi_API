using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace OmniFi_API.Migrations
{
    /// <inheritdoc />
    public partial class SeedFiatCurrenciesTbale : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "FiatCurrencies",
                columns: new[] { "CurrencyID", "CurrencyCode", "CurrencyName", "CurrencySymbol" },
                values: new object[,]
                {
                    { 1, "USD", "United States Dollar", "$" },
                    { 2, "EUR", "Euro", "€" },
                    { 3, "GBP", "British Pound", "£" },
                    { 4, "CHF", "Swiss Franc", "₣" },
                    { 5, "JPY", "Japanese Yen", "¥" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "FiatCurrencies",
                keyColumn: "CurrencyID",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "FiatCurrencies",
                keyColumn: "CurrencyID",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "FiatCurrencies",
                keyColumn: "CurrencyID",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "FiatCurrencies",
                keyColumn: "CurrencyID",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "FiatCurrencies",
                keyColumn: "CurrencyID",
                keyValue: 5);
        }
    }
}
