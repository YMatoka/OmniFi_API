using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OmniFi_API.Models.Banks;

namespace OmniFi_API.Data.FluentConfig.Banks
{
    public class BankSubAccountConfig : IEntityTypeConfiguration<BankSubAccount>
    {
        public void Configure(EntityTypeBuilder<BankSubAccount> builder)
        {

            builder.HasKey(x => x.BankSubAccountID);

            builder
                .Property(x => x.BankSubAccountID)
                .ValueGeneratedOnAdd();

            builder
                .HasOne(x => x.BankAccount)
                .WithMany(x => x.BankSubAccounts)
                .HasForeignKey(x => x.BankAccountID);
        }
    }
}
