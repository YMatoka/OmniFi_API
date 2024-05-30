﻿using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using OmniFi_API.Data.FluentConfig.Assets;
using OmniFi_API.Data.FluentConfig.Banks;
using OmniFi_API.Data.FluentConfig.Cryptos;
using OmniFi_API.Data.FluentConfig.Currencies;
using OmniFi_API.Data.FluentConfig.Identity;
using OmniFi_API.Models.Assets;
using OmniFi_API.Models.Banks;
using OmniFi_API.Models.Cryptos;
using OmniFi_API.Models.Currencies;
using OmniFi_API.Models.Identity;

namespace OmniFi_API.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, string>
    {
        public DbSet<AssetPlatform> AssetPlatforms {get;set;}
        public DbSet<AssetSource> AssetSources{get;set;}
        public DbSet<AssetTracking> AssetTrackings {get;set;}

        public DbSet<Bank> Banks { get; set; }
        public DbSet<BankAccount> BankAccounts { get;set;}
        public DbSet<BankCredential> BankCredentials { get; set; }

        public DbSet<CryptoExchange> CryptoExchanges { get;set;}
        public DbSet<CryptoExchangeAccount> CryptoExchangeAccounts { get;set;}
        public DbSet<CryptoApiCredential> ApiCredentials { get; set; }
        public DbSet<CryptoHolding> CryptoHoldings { get; set; }

        public DbSet<FiatCurrency> FiatCurrencies { get;set;}

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
            
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            // Use the built-in method to defines the key of the entity IdentityUserLogin<string>
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfiguration(new AssetPlatformConfig());
            modelBuilder.ApplyConfiguration(new AssetSourceConfig());
            modelBuilder.ApplyConfiguration(new AssetTrackingConfig());

            modelBuilder.ApplyConfiguration(new BankConfig());
            modelBuilder.ApplyConfiguration(new BankAccountConfig());
            modelBuilder.ApplyConfiguration(new BankCredentialConfig());

            modelBuilder.ApplyConfiguration(new CryptoExchangeAccountConfig());
            modelBuilder.ApplyConfiguration(new CryptoExchangeConfig());
            modelBuilder.ApplyConfiguration(new CryptoApiCredentialConfig());
            modelBuilder.ApplyConfiguration(new CryptoHoldingConfig());

            modelBuilder.ApplyConfiguration(new FiatCurrencyConfig());

        }


    }
}
