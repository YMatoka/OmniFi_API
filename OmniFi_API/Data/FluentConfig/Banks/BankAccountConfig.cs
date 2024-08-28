using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OmniFi_API.Models.Banks;

namespace OmniFi_API.Data.FluentConfig.Banks
{
    public class BankAccountConfig : IEntityTypeConfiguration<BankSubAccount>
    {
        public void Configure(EntityTypeBuilder<BankSubAccount> builder)
        {

            builder.HasKey(x => x.BankAccountID);

            builder
                .Property(x => x.BankAccountID)
                .ValueGeneratedOnAdd();

            builder
                .HasOne(x => x.User)
                .WithMany(x => x.BankAccounts)
                .HasForeignKey(x => x.UserID);

            builder
                .HasOne(x => x.Bank)
                .WithMany(x => x.BankAccounts)
                .HasForeignKey(x => x.BankID);
        }
    }
}
