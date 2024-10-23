using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OmniFi_API.Models.Assets;

namespace OmniFi_API.Data.FluentConfig.Assets
{
    public class FinancialAssetHistoryConfig : IEntityTypeConfiguration<FinancialAssetHistory>
    {
        public void Configure(EntityTypeBuilder<FinancialAssetHistory> builder)
        {

            builder.HasKey(x => x.FinancialEntityId);

            builder
                .Property(x => x.FinancialEntityId)
                .ValueGeneratedOnAdd();

            builder
                .HasOne(x => x.User)
                .WithMany(x => x.FinancialAssetsHistory)
                .HasForeignKey(x => x.UserID);

            builder
                .HasOne(x => x.FinancialAsset)
                .WithMany(x => x.FinancialAssetsHistory)
                .HasForeignKey(x => x.FinancialAssetId)
                .OnDelete(DeleteBehavior.NoAction);

            builder
                .HasOne(x => x.AssetPlatform)
                .WithMany(x => x.FinancialAssetsHistory)
                .HasForeignKey(x => x.AssetPlatformID)
                .OnDelete(DeleteBehavior.NoAction);

            builder
                .HasOne(x => x.AssetSource)
                .WithMany(x => x.FinancialAssetsHistory)
                .HasForeignKey(x => x.AssetSourceID)
                .OnDelete(DeleteBehavior.NoAction);

            builder
                .HasOne(x => x.FiatCurrency)
                .WithMany(x => x.FinancialAssetsHistory)
                .HasForeignKey(x => x.FiatCurrencyID)
                .OnDelete(DeleteBehavior.NoAction);

            builder
                .Property(x => x.Amount)
                .IsRequired()
                .HasPrecision(21, 2);

            builder
                .Property(x => x.RecordedAt)
                .IsRequired();

            //builder.HasData(
            //    new List<FinancialAssetHistory>()
            //    {
            //        new FinancialAssetHistory()
            //        {
            //            FinancialEntityId = 1,
            //            FinancialAssetId = 1,
            //            UserID = "67769df3-3d88-437a-8be8-b1729f251b3c",
            //            AssetPlatformID = 1,
            //            AssetSourceID = 1,
            //            Value = 1000.66m,
            //            FiatCurrencyID = 2,
            //            RecordedAt = DateTime.UtcNow,
            //        },

            //        new FinancialAssetHistory()
            //        {
            //            FinancialEntityId = 2,
            //            FinancialAssetId = 2,
            //            UserID = "67769df3-3d88-437a-8be8-b1729f251b3c",
            //            AssetPlatformID = 2,
            //            AssetSourceID = 4,
            //            Value = 23000.66m,
            //            FiatCurrencyID = 2,
            //            RecordedAt = DateTime.UtcNow,
            //        },
            //    }
            //);
        }
    }
}
