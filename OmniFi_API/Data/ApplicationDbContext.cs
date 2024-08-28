using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using OmniFi_API.Data.FluentConfig.Assets;
using OmniFi_API.Data.FluentConfig.Banks;
using OmniFi_API.Data.FluentConfig.Cryptos;
using OmniFi_API.Data.FluentConfig.Currencies;
using OmniFi_API.Data.FluentConfig.Encryption;
using OmniFi_API.Data.FluentConfig.Identity;
using OmniFi_API.Data.Interfaces;
using OmniFi_API.Models.Api;
using OmniFi_API.Models.Assets;
using OmniFi_API.Models.Banks;
using OmniFi_API.Models.Cryptos;
using OmniFi_API.Models.Currencies;
using OmniFi_API.Models.Encryption;
using OmniFi_API.Models.Identity;

namespace OmniFi_API.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, string>
    {
        public override DbSet<ApplicationUser> Users { get; set; }
        public override DbSet<ApplicationRole> Roles { get; set; }

        internal DbSet<AssetPlatform> AssetPlatforms { get; set; }
        internal DbSet<AssetSource> AssetSources { get; set; }
        internal DbSet<FinancialAsset> FinancialAssets { get; set; }
        internal DbSet<FinancialAssetHistory> FinancialAssetsHistory { get; set; }

        internal DbSet<Bank> Banks { get; set; }
        internal DbSet<BankSubAccount> BankAccounts { get; set; }
        internal DbSet<BankAccount> BankCredentials { get; set; }

        public DbSet<CryptoExchange> CryptoExchanges { get; set; }
        internal DbSet<CryptoExchangeAccount> CryptoExchangeAccounts { get; set; }
        internal DbSet<CryptoApiCredential> CryptoApiCredentials { get; set; }
        internal DbSet<CryptoCurrency> CryptoCurrencies { get; set; }
        internal DbSet<CryptoHolding> CryptoHoldings { get; set; }
        internal DbSet<CryptoHoldingHistory> CryptoHoldingsHystory { get; set; }

        internal DbSet<BankDataApiCredential> BankDataApiCredentials { get; set; }

        public DbSet<FiatCurrency> FiatCurrencies { get; set; }

        internal DbSet<AesKey> AesKeys { get; set; }
        internal DbSet<AesIV> AesIV { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
            
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            // Use the built-in method to defines the key of the entity IdentityUserLogin<string>
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfiguration(new AssetPlatformConfig());
            modelBuilder.ApplyConfiguration(new AssetSourceConfig());
            modelBuilder.ApplyConfiguration(new FinancialAssetConfig());
            modelBuilder.ApplyConfiguration(new FinancialAssetHistoryConfig());

            modelBuilder.ApplyConfiguration(new BankConfig());
            modelBuilder.ApplyConfiguration(new BankAccountConfig());
            modelBuilder.ApplyConfiguration(new BankCredentialConfig());

            modelBuilder.ApplyConfiguration(new CryptoExchangeAccountConfig());
            modelBuilder.ApplyConfiguration(new CryptoExchangeConfig());
            modelBuilder.ApplyConfiguration(new CryptoApiCredentialConfig());
            modelBuilder.ApplyConfiguration(new CryptoCurrencyConfig());
            modelBuilder.ApplyConfiguration(new CryptoHoldingConfig());
            modelBuilder.ApplyConfiguration(new CryptoHoldingHistoryConfig());

            modelBuilder.ApplyConfiguration(new FiatCurrencyConfig());

            modelBuilder.ApplyConfiguration(new ApplicationRoleConfig());
            modelBuilder.ApplyConfiguration(new ApplicationUserConfig());

            modelBuilder.ApplyConfiguration(new AesKeyConfig());
            modelBuilder.ApplyConfiguration(new AesIVConfig());
        }
    }
}
