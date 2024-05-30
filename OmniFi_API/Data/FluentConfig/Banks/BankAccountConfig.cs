using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OmniFi_API.Models.Banks;

namespace OmniFi_API.Data.FluentConfig.Banks
{
    public class BankAccountConfig : IEntityTypeConfiguration<BankAccount>
    {
        public void Configure(EntityTypeBuilder<BankAccount> builder)
        {

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
