using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OmniFi_API.Migrations
{
    /// <inheritdoc />
    public partial class AddAesIVEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AesIV",
                columns: table => new
                {
                    AesIVId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IV = table.Column<byte[]>(type: "varbinary(max)", nullable: false),
                    BankCredentialId = table.Column<int>(type: "int", nullable: true),
                    CryptoApiCredentialId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AesIV", x => x.AesIVId);
                    table.ForeignKey(
                        name: "FK_AesIV_BankCredentials_BankCredentialId",
                        column: x => x.BankCredentialId,
                        principalTable: "BankCredentials",
                        principalColumn: "BankCredientialID");
                    table.ForeignKey(
                        name: "FK_AesIV_CryptoApiCredentials_CryptoApiCredentialId",
                        column: x => x.CryptoApiCredentialId,
                        principalTable: "CryptoApiCredentials",
                        principalColumn: "CryptoApiCredentialID");
                });

            migrationBuilder.CreateIndex(
                name: "IX_AesIV_BankCredentialId",
                table: "AesIV",
                column: "BankCredentialId",
                unique: true,
                filter: "[BankCredentialId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AesIV_CryptoApiCredentialId",
                table: "AesIV",
                column: "CryptoApiCredentialId",
                unique: true,
                filter: "[CryptoApiCredentialId] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AesIV");
        }
    }
}
