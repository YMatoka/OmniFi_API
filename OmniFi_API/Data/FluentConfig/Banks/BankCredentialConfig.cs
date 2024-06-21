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

            builder
                .Property(x => x.BankCredientialID)
                .ValueGeneratedOnAdd();

            builder
                .Property(x => x.BankUserID)
                .IsRequired();

            builder
                .Property(x => x.Password)
                .IsRequired();

            builder
                .HasOne(x => x.BankAccount)
                .WithOne(x => x.BankCredential)
                .HasForeignKey<BankCredential>(x => x.BankAccountID);
        }
    }
}
