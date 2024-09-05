using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OmniFi_API.Models.Api.Banks;

namespace OmniFi_API.Data.FluentConfig.Api.Banks
{
    public class BankAgreementConfig : IEntityTypeConfiguration<BankAgreement>
    {
        public void Configure(EntityTypeBuilder<BankAgreement> builder)
        {
            builder.HasKey(x => x.BankAgreementId);

            builder
                  .Property(x => x.BankAgreementId)
                  .ValueGeneratedOnAdd()
                  .IsRequired();

            builder
                .HasOne(x => x.Bank)
                .WithMany(x => x.BankAgreements)
                .HasForeignKey(x => x.BankId);

            builder
                .HasOne(x => x.User)
                .WithMany(x => x.BankAgreements)
                .HasForeignKey(x => x.UserId);

        }
    }
}
