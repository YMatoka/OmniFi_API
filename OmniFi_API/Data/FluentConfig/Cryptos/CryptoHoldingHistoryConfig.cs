using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OmniFi_API.Models.Cryptos;

namespace OmniFi_API.Data.FluentConfig.Cryptos
{
    public class CryptoHoldingHistoryConfig : IEntityTypeConfiguration<CryptoHoldingHistory>

    {
        public void Configure(EntityTypeBuilder<CryptoHoldingHistory> builder)
        {
            builder.HasKey(x => x.CryptoHoldingEntityId);

            builder
                .Property(x => x.CryptoHoldingEntityId)
                .ValueGeneratedOnAdd();

            builder
                .HasOne(x => x.CryptoCurrency)
                .WithMany(x => x.CryptoHoldingsHistory)
                .HasForeignKey(x => x.CryptoCurrencId)
                .IsRequired();

            builder
                .Property(x => x.Quantity)
                .IsRequired()
                .HasPrecision(27, 18);

            builder
                .HasOne(x => x.FinancialAssetHistory)
                .WithOne(x => x.CryptoHoldingHistory)
                .HasForeignKey<CryptoHoldingHistory>(x => x.FinancialAssetHistoryID)
                .OnDelete(DeleteBehavior.NoAction)
                .IsRequired();

            builder
                .HasOne(x => x.CryptoHolding)
                .WithMany(x => x.CryptoHoldingsHistory)
                .HasForeignKey(x => x.CryptoHoldingId)
                .OnDelete(DeleteBehavior.NoAction)
                .IsRequired();

            //builder.HasData(new CryptoHoldingHistory()
            //{
            //    CryptoHoldingEntityId = 1,
            //    CryptoHoldingId = 1,
            //    CryptoCurrencyName = "Bitcoin",
            //    CryptoCurrencySymbol = "BTC",
            //    Quantity = 2.33m,
            //    FinancialAssetHistoryID = 2
            //});
        }
    }
}
