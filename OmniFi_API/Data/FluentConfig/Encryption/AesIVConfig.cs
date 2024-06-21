using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OmniFi_API.Models.Encryption;

namespace OmniFi_API.Data.FluentConfig.Encryption
{
    public class AesIVConfig : IEntityTypeConfiguration<AesIV>
    {
        public void Configure(EntityTypeBuilder<AesIV> builder)
        {
            builder.HasKey(x => x.AesIVId);

            builder
                .Property(x => x.AesIVId)
                .ValueGeneratedOnAdd();

            builder
                .HasOne(x => x.CryptoApiCredential)
                .WithOne(x => x.AesIV)
                .HasForeignKey<AesIV>(x => x.CryptoApiCredentialId)
                .IsRequired(false);

            builder
                .HasOne(x => x.BankCredential)
                .WithOne(x => x.AesIV)
                .HasForeignKey<AesIV>(x => x.BankCredentialId)
                .IsRequired(false);
           
        }
    }
}
