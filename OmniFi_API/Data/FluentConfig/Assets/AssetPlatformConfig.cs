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

            builder.HasData(
                new List<AssetPlatform>()
                {
                    new AssetPlatform()
                    {
                        AssetPlatformID = 1,
                        BankID = 1
                    },
                    new AssetPlatform()
                    {
                        AssetPlatformID = 2,
                        CryptoExchangeID = 1
                    },
                    new AssetPlatform()
                    {
                        AssetPlatformID = 3,
                        CryptoExchangeID = 2
                    },
                    new AssetPlatform()
                    {
                        AssetPlatformID = 4,
                        CryptoExchangeID = 3
                    },
                    new AssetPlatform()
                    {
                        AssetPlatformID = 5,
                        BankID = 2
                    },
                }
            );

        }
    }
}
