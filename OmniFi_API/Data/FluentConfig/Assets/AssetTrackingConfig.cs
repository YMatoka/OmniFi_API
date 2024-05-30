using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OmniFi_API.Models.Assets;

namespace OmniFi_API.Data.FluentConfig.Assets
{
    public class AssetTrackingConfig : IEntityTypeConfiguration<AssetTracking>
    {
        public void Configure(EntityTypeBuilder<AssetTracking> builder)
        {
            builder.HasKey(x => x.AssetTrackingID);

            builder
                .HasOne(x => x.User)
                .WithMany(x => x.AssetTrackings)
                .HasForeignKey(x => x.UserID);

            builder
                .HasOne(x => x.AssetPlatform)
                .WithMany(x => x.AssetTrackings)
                .HasForeignKey(x => x.AssetPlatformID);

            builder
                .HasOne(x => x.AssetSource)
                .WithMany(x => x.AssetTrackings)
                .HasForeignKey(x => x.AssetSourceID);

            builder
                .HasOne(x => x.FiatCurrency)
                .WithMany(x => x.AssetTrackings)
                .HasForeignKey(x => x.FiatCurrencyID)
                .OnDelete(DeleteBehavior.NoAction);

            builder
                .HasOne(x => x.CryptoHolding)
                .WithOne(x => x.AssetTracking)
                .HasForeignKey<AssetTracking>(x => x.CrytpoHoldingID)
                .IsRequired(false);


            builder
                .Property(x => x.Value)
                .IsRequired()
                .HasPrecision(21,2);

            builder.Property(x => x.RetrievalDate).IsRequired();

        }
    }
}
