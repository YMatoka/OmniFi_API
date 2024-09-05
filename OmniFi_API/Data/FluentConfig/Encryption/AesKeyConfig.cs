using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OmniFi_API.Models.Encryption;

namespace OmniFi_API.Data.FluentConfig.Encryption
{
    public class AesKeyConfig : IEntityTypeConfiguration<AesKey>
    {
        public void Configure(EntityTypeBuilder<AesKey> builder)
        {
            builder.HasKey(builder => builder.AesKeyId);

            builder
                .Property(x =>  x.AesKeyId)
                .ValueGeneratedOnAdd();

            builder
                .Property(x => x.Key)
                .IsRequired();

            builder
                .HasOne(x => x.BankDataApiCredential)
                .WithOne(x => x.AesKey)
                .HasForeignKey<AesKey>(x => x.BankDataApiCredentialId)
                .IsRequired(false);

            builder
                .HasOne(x => x.CryptoApiCredential)
                .WithOne(x => x.AesKey)
                .HasForeignKey<AesKey>(x => x.CryptoApiCredentialId)
                .IsRequired(false);
        }
    }
}
