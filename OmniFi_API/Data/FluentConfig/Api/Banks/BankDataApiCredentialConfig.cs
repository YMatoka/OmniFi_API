using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OmniFi_API.Models.Api;
using OmniFi_API.Models.Api.Banks;

namespace OmniFi_API.Data.FluentConfig.Api.Banks
{
    public class BankDataApiCredentialConfig : IEntityTypeConfiguration<BankDataApiCredential>
    {
        public void Configure(EntityTypeBuilder<BankDataApiCredential> builder)
        {
            builder.HasKey(x => x.BankDataApiId);

            builder
                .Property(x => x.BankDataApiId)
                .ValueGeneratedOnAdd()
                .IsRequired();
        }
    }
}
