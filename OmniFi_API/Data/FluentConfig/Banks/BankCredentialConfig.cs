using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OmniFi_API.Models.Banks;

namespace OmniFi_API.Data.FluentConfig.Banks
{
    public class BankCredentialConfig : IEntityTypeConfiguration<BankCredential>
    {
        public void Configure(EntityTypeBuilder<BankCredential> builder)
        {
            builder.HasKey(x => x.BankCredientialID);

            builder.Property(x => x.BankUserID).IsRequired();
            builder
                .Property(x => x.Password)
                .HasMaxLength(450)
                .IsRequired();

            builder
                .HasOne(x => x.ApplicationUser)
                .WithMany(x => x.BankCredentials)
                .HasForeignKey(x => x.UserID);

            builder
                .HasOne(x => x.Bank)
                .WithMany(x => x.BankCredentials)
                .HasForeignKey(x => x.BankID);
        }
    }
}
