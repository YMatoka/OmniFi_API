using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OmniFi_API.Models.Banks;

namespace OmniFi_API.Data.FluentConfig.Banks
{
    public class BankCredentialConfig : IEntityTypeConfiguration<BankAccount>
    {
        public void Configure(EntityTypeBuilder<BankAccount> builder)
        {
            builder.HasKey(x => x.BankAccountID);

            builder
                .Property(x => x.BankAccountID)
                .ValueGeneratedOnAdd();

            builder
                .Property(x => x.BankUserID)
                .IsRequired();

            builder
                .Property(x => x.RequisitionId)
                .IsRequired();

            builder
                .HasOne(x => x.BankAccount)
                .WithOne(x => x.BankCredential)
                .HasForeignKey<BankAccount>(x => x.BankAccountID);
        }
    }
}
