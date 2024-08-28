using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace OmniFi_API.Migrations
{
    /// <inheritdoc />
    public partial class FirstMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AssetSources",
                columns: table => new
                {
                    AssetSourceID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AssetSourceName = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AssetSources", x => x.AssetSourceID);
                });

            migrationBuilder.CreateTable(
                name: "Banks",
                columns: table => new
                {
                    BankID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BankName = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    ImageUrl = table.Column<byte[]>(type: "varbinary(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Banks", x => x.BankID);
                });

            migrationBuilder.CreateTable(
                name: "CryptoCurrencies",
                columns: table => new
                {
                    CurrencyID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CurrencyName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CurrencySymbol = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CryptoCurrencies", x => x.CurrencyID);
                });

            migrationBuilder.CreateTable(
                name: "CryptoExchanges",
                columns: table => new
                {
                    CryptoExchangeID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ExchangeName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ImageUrl = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CryptoExchanges", x => x.CryptoExchangeID);
                });

            migrationBuilder.CreateTable(
                name: "FiatCurrencies",
                columns: table => new
                {
                    FiatCurrencyID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CurrencyCode = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false),
                    CurrencyName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CurrencySymbol = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FiatCurrencies", x => x.FiatCurrencyID);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AssetPlatforms",
                columns: table => new
                {
                    AssetPlatformID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BankID = table.Column<int>(type: "int", nullable: true),
                    CryptoExchangeID = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AssetPlatforms", x => x.AssetPlatformID);
                    table.ForeignKey(
                        name: "FK_AssetPlatforms_Banks_BankID",
                        column: x => x.BankID,
                        principalTable: "Banks",
                        principalColumn: "BankID");
                    table.ForeignKey(
                        name: "FK_AssetPlatforms_CryptoExchanges_CryptoExchangeID",
                        column: x => x.CryptoExchangeID,
                        principalTable: "CryptoExchanges",
                        principalColumn: "CryptoExchangeID");
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SelectedFiatCurrencyID = table.Column<int>(type: "int", nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SecurityStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "bit", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "bit", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUsers_FiatCurrencies_SelectedFiatCurrencyID",
                        column: x => x.SelectedFiatCurrencyID,
                        principalTable: "FiatCurrencies",
                        principalColumn: "FiatCurrencyID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderKey = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BankAccounts",
                columns: table => new
                {
                    BankAccountID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserID = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    BankID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BankAccounts", x => x.BankAccountID);
                    table.ForeignKey(
                        name: "FK_BankAccounts_AspNetUsers_UserID",
                        column: x => x.UserID,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BankAccounts_Banks_BankID",
                        column: x => x.BankID,
                        principalTable: "Banks",
                        principalColumn: "BankID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CryptoExchangeAccounts",
                columns: table => new
                {
                    ExchangeAccountID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CryptoExchangeID = table.Column<int>(type: "int", nullable: false),
                    UserID = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CryptoExchangeAccounts", x => x.ExchangeAccountID);
                    table.ForeignKey(
                        name: "FK_CryptoExchangeAccounts_AspNetUsers_UserID",
                        column: x => x.UserID,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CryptoExchangeAccounts_CryptoExchanges_CryptoExchangeID",
                        column: x => x.CryptoExchangeID,
                        principalTable: "CryptoExchanges",
                        principalColumn: "CryptoExchangeID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FinancialAssets",
                columns: table => new
                {
                    FinancialEntityId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FirstRetrievedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastUpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UserID = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    AssetPlatformID = table.Column<int>(type: "int", nullable: false),
                    AssetSourceID = table.Column<int>(type: "int", nullable: false),
                    Value = table.Column<decimal>(type: "decimal(21,2)", precision: 21, scale: 2, nullable: false),
                    FiatCurrencyID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FinancialAssets", x => x.FinancialEntityId);
                    table.ForeignKey(
                        name: "FK_FinancialAssets_AspNetUsers_UserID",
                        column: x => x.UserID,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FinancialAssets_AssetPlatforms_AssetPlatformID",
                        column: x => x.AssetPlatformID,
                        principalTable: "AssetPlatforms",
                        principalColumn: "AssetPlatformID");
                    table.ForeignKey(
                        name: "FK_FinancialAssets_AssetSources_AssetSourceID",
                        column: x => x.AssetSourceID,
                        principalTable: "AssetSources",
                        principalColumn: "AssetSourceID");
                    table.ForeignKey(
                        name: "FK_FinancialAssets_FiatCurrencies_FiatCurrencyID",
                        column: x => x.FiatCurrencyID,
                        principalTable: "FiatCurrencies",
                        principalColumn: "FiatCurrencyID");
                });

            migrationBuilder.CreateTable(
                name: "BankCredentials",
                columns: table => new
                {
                    BankCredientialID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BankUserID = table.Column<int>(type: "int", nullable: false),
                    Password = table.Column<byte[]>(type: "varbinary(max)", nullable: false),
                    BankAccountID = table.Column<int>(type: "int", nullable: false),
                    BankID = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BankCredentials", x => x.BankCredientialID);
                    table.ForeignKey(
                        name: "FK_BankCredentials_BankAccounts_BankAccountID",
                        column: x => x.BankAccountID,
                        principalTable: "BankAccounts",
                        principalColumn: "BankAccountID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BankCredentials_Banks_BankID",
                        column: x => x.BankID,
                        principalTable: "Banks",
                        principalColumn: "BankID");
                });

            migrationBuilder.CreateTable(
                name: "CryptoApiCredentials",
                columns: table => new
                {
                    CryptoApiCredentialID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ApiKey = table.Column<byte[]>(type: "varbinary(max)", nullable: false),
                    ApiSecret = table.Column<byte[]>(type: "varbinary(max)", nullable: false),
                    CryptoExchangeAccountID = table.Column<int>(type: "int", nullable: false),
                    CryptoExchangeID = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CryptoApiCredentials", x => x.CryptoApiCredentialID);
                    table.ForeignKey(
                        name: "FK_CryptoApiCredentials_CryptoExchangeAccounts_CryptoExchangeAccountID",
                        column: x => x.CryptoExchangeAccountID,
                        principalTable: "CryptoExchangeAccounts",
                        principalColumn: "ExchangeAccountID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CryptoApiCredentials_CryptoExchanges_CryptoExchangeID",
                        column: x => x.CryptoExchangeID,
                        principalTable: "CryptoExchanges",
                        principalColumn: "CryptoExchangeID");
                });

            migrationBuilder.CreateTable(
                name: "CryptoHoldings",
                columns: table => new
                {
                    CryptoHoldingEntityId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FinancialAssetID = table.Column<int>(type: "int", nullable: false),
                    CryptoCurrencId = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<decimal>(type: "decimal(27,18)", precision: 27, scale: 18, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CryptoHoldings", x => x.CryptoHoldingEntityId);
                    table.ForeignKey(
                        name: "FK_CryptoHoldings_CryptoCurrencies_CryptoCurrencId",
                        column: x => x.CryptoCurrencId,
                        principalTable: "CryptoCurrencies",
                        principalColumn: "CurrencyID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CryptoHoldings_FinancialAssets_FinancialAssetID",
                        column: x => x.FinancialAssetID,
                        principalTable: "FinancialAssets",
                        principalColumn: "FinancialEntityId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FinancialAssetsHistory",
                columns: table => new
                {
                    FinancialEntityId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FinancialAssetId = table.Column<int>(type: "int", nullable: false),
                    RecordedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UserID = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    AssetPlatformID = table.Column<int>(type: "int", nullable: false),
                    AssetSourceID = table.Column<int>(type: "int", nullable: false),
                    Value = table.Column<decimal>(type: "decimal(21,2)", precision: 21, scale: 2, nullable: false),
                    FiatCurrencyID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FinancialAssetsHistory", x => x.FinancialEntityId);
                    table.ForeignKey(
                        name: "FK_FinancialAssetsHistory_AspNetUsers_UserID",
                        column: x => x.UserID,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FinancialAssetsHistory_AssetPlatforms_AssetPlatformID",
                        column: x => x.AssetPlatformID,
                        principalTable: "AssetPlatforms",
                        principalColumn: "AssetPlatformID");
                    table.ForeignKey(
                        name: "FK_FinancialAssetsHistory_AssetSources_AssetSourceID",
                        column: x => x.AssetSourceID,
                        principalTable: "AssetSources",
                        principalColumn: "AssetSourceID");
                    table.ForeignKey(
                        name: "FK_FinancialAssetsHistory_FiatCurrencies_FiatCurrencyID",
                        column: x => x.FiatCurrencyID,
                        principalTable: "FiatCurrencies",
                        principalColumn: "FiatCurrencyID");
                    table.ForeignKey(
                        name: "FK_FinancialAssetsHistory_FinancialAssets_FinancialAssetId",
                        column: x => x.FinancialAssetId,
                        principalTable: "FinancialAssets",
                        principalColumn: "FinancialEntityId");
                });

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
                        principalColumn: "BankAccountID");
                    table.ForeignKey(
                        name: "FK_AesIV_CryptoApiCredentials_CryptoApiCredentialId",
                        column: x => x.CryptoApiCredentialId,
                        principalTable: "CryptoApiCredentials",
                        principalColumn: "CryptoApiCredentialID");
                });

            migrationBuilder.CreateTable(
                name: "AesKeys",
                columns: table => new
                {
                    AesKeyId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Key = table.Column<byte[]>(type: "varbinary(max)", nullable: false),
                    BankCredentialId = table.Column<int>(type: "int", nullable: true),
                    CryptoApiCredentialId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AesKeys", x => x.AesKeyId);
                    table.ForeignKey(
                        name: "FK_AesKeys_BankCredentials_BankCredentialId",
                        column: x => x.BankCredentialId,
                        principalTable: "BankCredentials",
                        principalColumn: "BankAccountID");
                    table.ForeignKey(
                        name: "FK_AesKeys_CryptoApiCredentials_CryptoApiCredentialId",
                        column: x => x.CryptoApiCredentialId,
                        principalTable: "CryptoApiCredentials",
                        principalColumn: "CryptoApiCredentialID");
                });

            migrationBuilder.CreateTable(
                name: "CryptoHoldingsHystory",
                columns: table => new
                {
                    CryptoHoldingEntityId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FinancialAssetHistoryID = table.Column<int>(type: "int", nullable: false),
                    CryptoHoldingId = table.Column<int>(type: "int", nullable: false),
                    CryptoCurrencId = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<decimal>(type: "decimal(27,18)", precision: 27, scale: 18, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CryptoHoldingsHystory", x => x.CryptoHoldingEntityId);
                    table.ForeignKey(
                        name: "FK_CryptoHoldingsHystory_CryptoCurrencies_CryptoCurrencId",
                        column: x => x.CryptoCurrencId,
                        principalTable: "CryptoCurrencies",
                        principalColumn: "CurrencyID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CryptoHoldingsHystory_CryptoHoldings_CryptoHoldingId",
                        column: x => x.CryptoHoldingId,
                        principalTable: "CryptoHoldings",
                        principalColumn: "CryptoHoldingEntityId");
                    table.ForeignKey(
                        name: "FK_CryptoHoldingsHystory_FinancialAssetsHistory_FinancialAssetHistoryID",
                        column: x => x.FinancialAssetHistoryID,
                        principalTable: "FinancialAssetsHistory",
                        principalColumn: "FinancialEntityId");
                });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "e1f8f0f8-4e98-4d30-abf8-8488705defe3", "c833d87b-30cd-4cc0-aa19-9eb5cc2c4b13", "Admin", "ADMIN" },
                    { "ea1104de-92d7-499a-a2eb-0f707e6bb911", "bbaf7e3d-5d38-4372-bddf-beed77f83baf", "User", "USER" }
                });

            migrationBuilder.InsertData(
                table: "AssetSources",
                columns: new[] { "AssetSourceID", "AssetSourceName" },
                values: new object[,]
                {
                    { 1, "CheckingAccount" },
                    { 2, "SavingAccount" },
                    { 3, "ShareAccount" },
                    { 4, "CryptoHolding" }
                });

            migrationBuilder.InsertData(
                table: "Banks",
                columns: new[] { "BankID", "BankName", "ImageUrl" },
                values: new object[] { 1, "BoursoBank", new byte[] { 104, 116, 116, 112, 115, 58, 47, 47, 117, 112, 108, 111, 97, 100, 46, 119, 105, 107, 105, 109, 101, 100, 105, 97, 46, 111, 114, 103, 47, 119, 105, 107, 105, 112, 101, 100, 105, 97, 47, 102, 114, 47, 116, 104, 117, 109, 98, 47, 51, 47, 51, 100, 47, 76, 111, 103, 111, 45, 98, 111, 117, 114, 115, 111, 114, 97, 109, 97, 45, 98, 97, 110, 113, 117, 101, 46, 115, 118, 103, 47, 49, 48, 50, 52, 112, 120, 45, 76, 111, 103, 111, 45, 98, 111, 117, 114, 115, 111, 114, 97, 109, 97, 45, 98, 97, 110, 113, 117, 101, 46, 115, 118, 103, 46, 112, 110, 103 } });

            migrationBuilder.InsertData(
                table: "CryptoExchanges",
                columns: new[] { "CryptoExchangeID", "ExchangeName", "ImageUrl" },
                values: new object[,]
                {
                    { 1, "Binance", "https://w7.pngwing.com/pngs/696/485/png-transparent-binance-logo-cryptocurrency-exchange-coin-text-logo-computer-wallpaper.png" },
                    { 2, "Crypto.Com", "https://upload.wikimedia.org/wikipedia/commons/thumb/b/b0/Cryptos.com_logo.svg/2560px-Cryptos.com_logo.svg.png" },
                    { 3, "Kraken", "https://logo-marque.com/wp-content/uploads/2021/03/Kraken-Logo.png" }
                });

            migrationBuilder.InsertData(
                table: "FiatCurrencies",
                columns: new[] { "FiatCurrencyID", "CurrencyCode", "CurrencyName", "CurrencySymbol" },
                values: new object[,]
                {
                    { 1, "USD", "United States Dollar", "$" },
                    { 2, "EUR", "Euro", "€" },
                    { 3, "GBP", "British Pound", "£" },
                    { 4, "CHF", "Swiss Franc", "₣" },
                    { 5, "JPY", "Japanese Yen", "¥" }
                });

            migrationBuilder.InsertData(
                table: "AssetPlatforms",
                columns: new[] { "AssetPlatformID", "BankID", "CryptoExchangeID" },
                values: new object[,]
                {
                    { 1, 1, null },
                    { 2, null, 1 },
                    { 3, null, 2 },
                    { 4, null, 3 }
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

            migrationBuilder.CreateIndex(
                name: "IX_AesKeys_BankCredentialId",
                table: "AesKeys",
                column: "BankCredentialId",
                unique: true,
                filter: "[BankCredentialId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AesKeys_CryptoApiCredentialId",
                table: "AesKeys",
                column: "CryptoApiCredentialId",
                unique: true,
                filter: "[CryptoApiCredentialId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true,
                filter: "[NormalizedName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_SelectedFiatCurrencyID",
                table: "AspNetUsers",
                column: "SelectedFiatCurrencyID");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true,
                filter: "[NormalizedUserName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AssetPlatforms_BankID",
                table: "AssetPlatforms",
                column: "BankID",
                unique: true,
                filter: "[BankID] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AssetPlatforms_CryptoExchangeID",
                table: "AssetPlatforms",
                column: "CryptoExchangeID",
                unique: true,
                filter: "[CryptoExchangeID] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_BankAccounts_BankID",
                table: "BankAccounts",
                column: "BankID");

            migrationBuilder.CreateIndex(
                name: "IX_BankAccounts_UserID",
                table: "BankAccounts",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "IX_BankCredentials_BankAccountID",
                table: "BankCredentials",
                column: "BankAccountID",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BankCredentials_BankID",
                table: "BankCredentials",
                column: "BankID");

            migrationBuilder.CreateIndex(
                name: "IX_CryptoApiCredentials_CryptoExchangeAccountID",
                table: "CryptoApiCredentials",
                column: "CryptoExchangeAccountID",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CryptoApiCredentials_CryptoExchangeID",
                table: "CryptoApiCredentials",
                column: "CryptoExchangeID");

            migrationBuilder.CreateIndex(
                name: "IX_CryptoExchangeAccounts_CryptoExchangeID",
                table: "CryptoExchangeAccounts",
                column: "CryptoExchangeID");

            migrationBuilder.CreateIndex(
                name: "IX_CryptoExchangeAccounts_UserID",
                table: "CryptoExchangeAccounts",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "IX_CryptoHoldings_CryptoCurrencId",
                table: "CryptoHoldings",
                column: "CryptoCurrencId");

            migrationBuilder.CreateIndex(
                name: "IX_CryptoHoldings_FinancialAssetID",
                table: "CryptoHoldings",
                column: "FinancialAssetID",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CryptoHoldingsHystory_CryptoCurrencId",
                table: "CryptoHoldingsHystory",
                column: "CryptoCurrencId");

            migrationBuilder.CreateIndex(
                name: "IX_CryptoHoldingsHystory_CryptoHoldingId",
                table: "CryptoHoldingsHystory",
                column: "CryptoHoldingId");

            migrationBuilder.CreateIndex(
                name: "IX_CryptoHoldingsHystory_FinancialAssetHistoryID",
                table: "CryptoHoldingsHystory",
                column: "FinancialAssetHistoryID",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_FinancialAssets_AssetPlatformID",
                table: "FinancialAssets",
                column: "AssetPlatformID");

            migrationBuilder.CreateIndex(
                name: "IX_FinancialAssets_AssetSourceID",
                table: "FinancialAssets",
                column: "AssetSourceID");

            migrationBuilder.CreateIndex(
                name: "IX_FinancialAssets_FiatCurrencyID",
                table: "FinancialAssets",
                column: "FiatCurrencyID");

            migrationBuilder.CreateIndex(
                name: "IX_FinancialAssets_UserID",
                table: "FinancialAssets",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "IX_FinancialAssetsHistory_AssetPlatformID",
                table: "FinancialAssetsHistory",
                column: "AssetPlatformID");

            migrationBuilder.CreateIndex(
                name: "IX_FinancialAssetsHistory_AssetSourceID",
                table: "FinancialAssetsHistory",
                column: "AssetSourceID");

            migrationBuilder.CreateIndex(
                name: "IX_FinancialAssetsHistory_FiatCurrencyID",
                table: "FinancialAssetsHistory",
                column: "FiatCurrencyID");

            migrationBuilder.CreateIndex(
                name: "IX_FinancialAssetsHistory_FinancialAssetId",
                table: "FinancialAssetsHistory",
                column: "FinancialAssetId");

            migrationBuilder.CreateIndex(
                name: "IX_FinancialAssetsHistory_UserID",
                table: "FinancialAssetsHistory",
                column: "UserID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AesIV");

            migrationBuilder.DropTable(
                name: "AesKeys");

            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "CryptoHoldingsHystory");

            migrationBuilder.DropTable(
                name: "BankCredentials");

            migrationBuilder.DropTable(
                name: "CryptoApiCredentials");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "CryptoHoldings");

            migrationBuilder.DropTable(
                name: "FinancialAssetsHistory");

            migrationBuilder.DropTable(
                name: "BankAccounts");

            migrationBuilder.DropTable(
                name: "CryptoExchangeAccounts");

            migrationBuilder.DropTable(
                name: "CryptoCurrencies");

            migrationBuilder.DropTable(
                name: "FinancialAssets");

            migrationBuilder.DropTable(
                name: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "AssetPlatforms");

            migrationBuilder.DropTable(
                name: "AssetSources");

            migrationBuilder.DropTable(
                name: "FiatCurrencies");

            migrationBuilder.DropTable(
                name: "Banks");

            migrationBuilder.DropTable(
                name: "CryptoExchanges");
        }
    }
}
