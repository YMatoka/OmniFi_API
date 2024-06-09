using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OmniFi_API.Models.Assets;
using OmniFi_API.Models.Banks;
using OmniFi_API.Models.Cryptos;
using OmniFi_API.Models.Currencies;
using OmniFi_API.Models.Identity;

namespace OmniFi_API.Data.Interfaces
{
    public interface IApplicationDbContext
    {
        DbSet<CryptoApiCredential> ApiCredentials { get; set; }
        DbSet<AssetPlatform> AssetPlatforms { get; set; }
        DbSet<AssetSource> AssetSources { get; set; }
        DbSet<AssetTracking> AssetTrackings { get; set; }
        DbSet<BankAccount> BankAccounts { get; set; }
        DbSet<BankCredential> BankCredentials { get; set; }
        DbSet<Bank> Banks { get; set; }
        DbSet<CryptoExchangeAccount> CryptoExchangeAccounts { get; set; }
        DbSet<CryptoExchange> CryptoExchanges { get; set; }
        DbSet<CryptoHolding> CryptoHoldings { get; set; }
        DbSet<FiatCurrency> FiatCurrencies { get; set; }
        DbSet<ApplicationUser> Users { get; set; }
        DbSet<ApplicationRole> Roles { get; set; }
    }
}