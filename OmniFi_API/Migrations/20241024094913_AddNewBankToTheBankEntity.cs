using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OmniFi_API.Migrations
{
    /// <inheritdoc />
    public partial class AddNewBankToTheBankEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Banks",
                columns: new[] { "BankID", "BankName", "ImageUrl" },
                values: new object[] { 2, "CaisseDepargneIleDeFrance", new byte[] { 104, 116, 116, 112, 115, 58, 47, 47, 115, 116, 111, 114, 97, 103, 101, 46, 103, 111, 111, 103, 108, 101, 97, 112, 105, 115, 46, 99, 111, 109, 47, 101, 110, 100, 117, 114, 97, 110, 99, 101, 45, 97, 112, 112, 115, 45, 108, 105, 105, 112, 47, 109, 101, 100, 105, 97, 47, 99, 97, 99, 104, 101, 47, 116, 104, 101, 109, 105, 110, 103, 95, 110, 111, 95, 102, 105, 108, 116, 101, 114, 95, 103, 114, 105, 100, 95, 102, 115, 47, 53, 56, 52, 53, 55, 54, 55, 49, 100, 101, 54, 48, 99, 52, 54, 51, 48, 48, 56, 98, 52, 54, 56, 56 } });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Banks",
                keyColumn: "BankID",
                keyValue: 2);
        }
    }
}
