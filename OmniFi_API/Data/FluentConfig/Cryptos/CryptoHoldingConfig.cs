using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OmniFi_API.Models.Assets;
using OmniFi_API.Models.Cryptos;

namespace OmniFi_API.Data.FluentConfig.Cryptos
{
    public class CryptoHoldingConfig : IEntityTypeConfiguration<CryptoHolding>
    {
        public void Configure(EntityTypeBuilder<CryptoHolding> builder)
        {
            builder.HasKey(x => x.CryptoHoldingEntityId);

            builder
                .Property(x => x.CryptoHoldingEntityId)
                .ValueGeneratedOnAdd();

            builder
                .HasOne(x => x.CryptoCurrency)
                .WithMany(x => x.CryptoHoldings)
                .HasForeignKey(x => x.CryptoCurrencId)
                .IsRequired();

            builder
                .Property(x => x.Quantity)
                .IsRequired()
                .HasPrecision(27,18);

            builder
                .HasOne(x => x.FinancialAsset)
                .WithOne(x => x.CryptoHolding)
                .HasForeignKey<CryptoHolding>(x => x.FinancialAssetID)
                .IsRequired();

            //builder.HasData(new CryptoHolding()
            //{
            //    CryptoHoldingEntityId = 1,
            //    CryptoCurrencyName = "Bitcoin",
            //    CryptoCurrencySymbol = "BTC",
            //    Quantity = 2.33m,
            //    FinancialAssetID = 2
            //});
        }
    }
}
