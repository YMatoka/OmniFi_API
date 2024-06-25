using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OmniFi_API.Models.Cryptos;

namespace OmniFi_API.Data.FluentConfig.Cryptos
{
    public class CryptoApiCredentialConfig : IEntityTypeConfiguration<CryptoApiCredential>
    {
        public void Configure(EntityTypeBuilder<CryptoApiCredential> builder)
        {
            builder.HasKey(x => x.CryptoApiCredentialID);

            builder
                .Property(x => x.CryptoApiCredentialID)
                .ValueGeneratedOnAdd();

            builder.Property(x => x.ApiKey)
                .IsRequired();

            builder.Property(x => x.ApiSecret)
                .IsRequired();

            builder
                .HasOne(x => x.CryptoExchangeAccount)
                .WithOne(x => x.CryptoApiCredential)
                .HasForeignKey<CryptoApiCredential>(x => x.CryptoExchangeAccountID);
        }
    }
}
