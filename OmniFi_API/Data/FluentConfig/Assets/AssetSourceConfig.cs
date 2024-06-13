using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OmniFi_API.Models.Assets;
using OmniFi_API.Utilities;

namespace OmniFi_API.Data.FluentConfig.Assets
{
    public class AssetSourceConfig : IEntityTypeConfiguration<AssetSource>
    {
        public void Configure(EntityTypeBuilder<AssetSource> builder)
        {
            builder.HasKey(x => x.AssetSourceID);

            builder
                .Property(x => x.AssetSourceID)
                .ValueGeneratedOnAdd();

            builder
                .Property(x => x.AssetSourceName)
                .HasMaxLength(30)
                .IsRequired();

            var data = new List<AssetSource>()
            {
                new AssetSource()
                {
                    AssetSourceID = 1,
                    AssetSourceName = AssetSourceNames.CheckingAccount
                },
                new AssetSource()
                {
                    AssetSourceID = 2,
                    AssetSourceName = AssetSourceNames.SavingAccount
                },
                new AssetSource()
                {
                    AssetSourceID = 3,
                    AssetSourceName = AssetSourceNames.ShareAccount
                },
                new AssetSource()
                {
                    AssetSourceID = 4,
                    AssetSourceName = AssetSourceNames.CryptoHolding
                }
            };

            builder.HasData(data);

        }
    }
}
