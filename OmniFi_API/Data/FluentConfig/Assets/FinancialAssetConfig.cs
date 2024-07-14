using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OmniFi_API.Models.Assets;

namespace OmniFi_API.Data.FluentConfig.Assets
{
    public class FinancialAssetConfig : IEntityTypeConfiguration<FinancialAsset>
    {
        public void Configure(EntityTypeBuilder<FinancialAsset> builder)
        {
            builder.HasKey(x => x.FinancialEntityId);

            builder
                .Property(x => x.FinancialEntityId)
                .ValueGeneratedOnAdd();

            builder
                .HasOne(x => x.User)
                .WithMany(x => x.FinancialAssets)
                .HasForeignKey(x => x.UserID);

            builder
                .HasOne(x => x.AssetPlatform)
                .WithMany(x => x.FinancialAssets)
                .HasForeignKey(x => x.AssetPlatformID)
                .OnDelete(DeleteBehavior.NoAction);

            builder
                .HasOne(x => x.AssetSource)
                .WithMany(x => x.FinancialAssets)
                .HasForeignKey(x => x.AssetSourceID)
                .OnDelete(DeleteBehavior.NoAction);

            builder
                .HasOne(x => x.FiatCurrency)
                .WithMany(x => x.FinancialAssets)
                .HasForeignKey(x => x.FiatCurrencyID)
                .OnDelete(DeleteBehavior.NoAction);

            builder
                .Property(x => x.Value)
                .IsRequired()
                .HasPrecision(21,2);

            builder
                .Property(x => x.FirstRetrievedAt)
                .IsRequired();

            builder
                .Property(x => x.LastUpdatedAt)
                .IsRequired();

            //builder.HasData(
            //    new List<FinancialAsset>()
            //    {
            //        new FinancialAsset()
            //        {
            //            FinancialEntityId = 1,
            //            UserID = "67769df3-3d88-437a-8be8-b1729f251b3c",
            //            AssetPlatformID = 1,
            //            AssetSourceID = 1,
            //            Value = 1000.66m,
            //            FiatCurrencyID = 2,
            //            FirstRetrievedAt = DateTime.UtcNow,
            //            LastUpdatedAt = DateTime.UtcNow
            //        },

            //        new FinancialAsset()
            //        {
            //            FinancialEntityId = 2,
            //            UserID = "67769df3-3d88-437a-8be8-b1729f251b3c",
            //            AssetPlatformID = 2,
            //            AssetSourceID = 4,
            //            Value = 23000.66m,
            //            FiatCurrencyID = 2,
            //            FirstRetrievedAt = DateTime.UtcNow,
            //            LastUpdatedAt = DateTime.UtcNow
            //        },
            //    }
            //);

        }
    }
}
