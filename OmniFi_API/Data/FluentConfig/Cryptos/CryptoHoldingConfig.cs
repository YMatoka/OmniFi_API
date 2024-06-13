using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OmniFi_API.Models.Cryptos;

namespace OmniFi_API.Data.FluentConfig.Cryptos
{
    public class CryptoHoldingConfig : IEntityTypeConfiguration<CryptoHolding>
    {
        public void Configure(EntityTypeBuilder<CryptoHolding> builder)
        {
            builder.HasKey(x => x.CrytpoHoldingID);

            builder
                .Property(x => x.CrytpoHoldingID)
                .ValueGeneratedOnAdd();

            builder.Property(x => x.CryptoCurrencySymbol)
                .HasMaxLength(10)
                .IsRequired();

            builder.Property(x => x.CryptoCurrencyName)
                .HasMaxLength(30)
                .IsRequired();

            builder
                .Property(x => x.Amount)
                .IsRequired()
                .HasPrecision(27,18);
        }
    }
}
