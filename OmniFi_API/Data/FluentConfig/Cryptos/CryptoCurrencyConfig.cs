using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OmniFi_API.Models.Cryptos;

namespace OmniFi_API.Data.FluentConfig.Cryptos
{
    public class CryptoCurrencyConfig : IEntityTypeConfiguration<CryptoCurrency>
    {
        public void Configure(EntityTypeBuilder<CryptoCurrency> builder)
        {
            builder.HasKey(x => x.CurrencyID);

            builder
                .Property(x => x.CurrencyID)
                .ValueGeneratedOnAdd();

            builder
                .Property(x => x.CurrencyName)
                .HasMaxLength(100)
                .IsRequired();

            builder
                .Property(x => x.CurrencySymbol)
                .HasMaxLength(10)
                .IsRequired();

        }
    }
}
