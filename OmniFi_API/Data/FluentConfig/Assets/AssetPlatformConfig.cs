using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OmniFi_API.Models.Assets;

namespace OmniFi_API.Data.FluentConfig.Assets
{
    public class AssetPlatformConfig : IEntityTypeConfiguration<AssetPlatform>
    {
        public void Configure(EntityTypeBuilder<AssetPlatform> builder)
        {
            builder.HasKey(x => x.AssetPlatformID);

            builder
                .Property(x => x.AssetPlatformID)
                .ValueGeneratedOnAdd();

            builder
                .HasOne(x => x.Bank)
                .WithOne(x => x.AssetPlatform)
                .HasForeignKey<AssetPlatform>(x => x.BankID)
                .IsRequired(false);

            builder
                .HasOne(x => x.CryptoExchange)
                .WithOne(x => x.AssetPlatform)
                .HasForeignKey<AssetPlatform>(x => x.CryptoExchangeID)
                .IsRequired(false);
        }
    }
}
