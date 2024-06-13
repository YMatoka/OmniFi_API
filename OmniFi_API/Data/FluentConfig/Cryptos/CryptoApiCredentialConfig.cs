using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OmniFi_API.Models.Cryptos;

namespace OmniFi_API.Data.FluentConfig.Cryptos
{
    public class CryptoApiCredentialConfig : IEntityTypeConfiguration<CryptoApiCredential>
    {
        public void Configure(EntityTypeBuilder<CryptoApiCredential> builder)
        {
            builder.HasKey(x => x.ApiCrendentialsID);

            builder
                .Property(x => x.ApiCrendentialsID)
                .ValueGeneratedOnAdd();

            builder.Property(x => x.ApiKey)
                .HasMaxLength(128)
                .IsRequired();

            builder.Property(x => x.ApiSecret)
                .HasMaxLength(128)
                .IsRequired();

            builder
                .HasOne(x => x.ApplicationUser)
                .WithMany(x => x.ApiCredentials)
                .HasForeignKey(x => x.UserId);

            builder
                .HasOne(x => x.CryptoExchange)
                .WithMany(x => x.ApiCredentials)
                .HasForeignKey(x => x.CryptoExchangeID);
        }
    }
}
